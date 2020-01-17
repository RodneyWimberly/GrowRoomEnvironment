using Arch.EntityFrameworkCore.UnitOfWork;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using GrowRoomEnvironment.DataAccess;
using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.Web.ViewModels;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.Web.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<ApplicationDbContext> _unitOfWork;
        private readonly IRepository<Event> _repository;
        private readonly ILogger _logger;

        public EventController(IMapper mapper, IUnitOfWork<ApplicationDbContext> unitOfWork, ILogger<EventController> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<Event>();
            _logger = logger;
        }

        [HttpGet("{getDisabled:bool?}")]
        [Authorize(Authorization.Policies.ViewEventsPolicy)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventViewModel>))]
        public async Task<IActionResult> GetAll(bool getDisabled = true)
        {
            return await GetAllPaged(0, 1000, getDisabled);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}/{getDisabled:bool?}")]
        [Authorize(Authorization.Policies.ViewEventsPolicy)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventViewModel>))]
        public async Task<IActionResult> GetAllPaged(int pageNumber, int pageSize, bool getDisabled = true)
        {
            IPagedList<Event> events = await _repository.GetPagedListAsync(predicate: e => getDisabled ? true : e.IsEnabled, pageIndex: pageNumber, pageSize: pageSize,
                include: e => GetInclude(e));
            return Ok(_mapper.Map<IEnumerable<EventViewModel>>(events.Items));
        }

        [HttpGet("{actionDeviceId:int}")]
        [Authorize(Authorization.Policies.ViewEventsPolicy)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByActionDeviceId(int actionDeviceId)
        {
            return await GetByActionDeviceIdPaged(actionDeviceId, 0, 1000);
        }

        [HttpGet("{actionDeviceId:int}/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Authorization.Policies.ViewEventsPolicy)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByActionDeviceIdPaged(int actionDeviceId, int pageNumber, int pageSize)
        {
            IPagedList<Event> events = await _repository.GetPagedListAsync(e => e.ActionDeviceId == actionDeviceId, pageIndex: pageNumber, pageSize: pageSize,
                include: e => GetInclude(e));
            if (events.Items.Count > 0)
                return Ok(_mapper.Map<IEnumerable<EventViewModel>>(events.Items));
            else
                return NotFound(actionDeviceId);
        }


        [HttpGet("{eventId:int}")]
        [Authorize(Authorization.Policies.ViewEventsPolicy)]
        [ProducesResponseType(200, Type = typeof(EventViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int eventId)
        {
            Event @event = await GetByIdAsync(eventId);
            if (@event == null)
                return NotFound(eventId);
            else
                return Ok(_mapper.Map<EventViewModel>(@event));
        }


        [HttpDelete("{eventId:int}")]
        [Authorize(Authorization.Policies.ManageEventsPolicy)]
        [ProducesResponseType(200, Type = typeof(EventViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int eventId)
        {
            Event @event = await GetByIdAsync(eventId);
            if (@event == null)
                return NotFound(eventId);
            EventViewModel eventVM = _mapper.Map<EventViewModel>(@event);
            @event = MegreViewModelToEntity(eventVM, @event);
            _repository.Delete(@event);
            await _unitOfWork.SaveChangesAsync();
            return Ok(eventVM);
        }


        [HttpPost]
        [Authorize(Authorization.Policies.ManageEventsPolicy)]
        [ProducesResponseType(201, Type = typeof(EventViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                if (eventVM == null)
                    return BadRequest($"{nameof(eventVM)} cannot be null");
                Event @event = _mapper.Map<Event>(eventVM);
                @event = MegreViewModelToEntity(eventVM, @event);
                EntityEntry<Event> addedEvent = await _repository.InsertAsync(@event);
                await _unitOfWork.SaveChangesAsync();
                eventVM = _mapper.Map<EventViewModel>(addedEvent.Entity);
                return CreatedAtAction("GetByEventId", new { eventId = eventVM.EventId }, eventVM);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{eventId:int}")]
        [Authorize(Authorization.Policies.ManageEventsPolicy)]
        [ProducesResponseType(200, Type = typeof(EventViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put(int eventId, [FromBody]EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                if (eventVM == null)
                    return BadRequest($"{nameof(eventVM)} cannot be null");

                if (eventId != eventVM.EventId)
                    return BadRequest("Conflicting Event EventId in parameter and model data");

                Event @event = await GetByIdAsync(eventId);
                if (@event == null)
                    return BadRequest($"Cannot find Event with EventId: {eventId}");

                @event = MegreViewModelToEntity(eventVM, @event);

                Event eventToUpdate = _mapper.Map(eventVM, @event);

                _repository.Update(eventToUpdate);
                await _unitOfWork.SaveChangesAsync();
                Event updatedEvent = await GetByIdAsync(eventId);
                return Ok(_mapper.Map<EventViewModel>(updatedEvent));
            }
            else
                return BadRequest(ModelState);
        }


        [HttpPatch("{eventId:int}")]
        [Authorize(Authorization.Policies.ManageEventsPolicy)]
        [ProducesResponseType(200, Type = typeof(EventViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Patch(int eventId, [FromBody]JsonPatchDocument<EventViewModel> patch)
        {
            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");

                Event @event = await GetByIdAsync(eventId);
                EventViewModel eventVM = _mapper.Map<EventViewModel>(@event);
                @event = MegreViewModelToEntity(eventVM, @event);
                patch.ApplyTo(eventVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    Event eventToUpdate = _mapper.Map(eventVM, @event);
                    _repository.Update(eventToUpdate);
                    await _unitOfWork.SaveChangesAsync();
                    Event updatedEvent = await GetByIdAsync(eventId);
                    return Ok(updatedEvent);
                }
            }

            return BadRequest(ModelState);
        }

        private async Task<Event> GetByIdAsync(int eventId)
        {
            return await _repository.GetFirstOrDefaultAsync(
                predicate: e => e.EventId == eventId,
                include: e => GetInclude(e));
        }

        private IIncludableQueryable<Event, object> GetInclude(IQueryable<Event> e)
        {
            IIncludableQueryable<Event, object> include = e
                .Include(e => e.ActionDevice)
                .Include(e => e.EventConditions)
                    .ThenInclude(ec => ec.DataPoint);

            return include;
        }

        private Event MegreViewModelToEntity(EventViewModel eventVM, Event @event)
        {
            @event.ActionDevice = null;
            foreach (EventCondition ec in @event.EventConditions)
                ec.DataPoint = null;

            List<EventCondition> deleteList = new List<EventCondition>();
            foreach (EventCondition eventCondition in @event.EventConditions)
            {
                if (!eventVM.EventConditions.Any(ecVM => ecVM.EventConditionId == eventCondition.EventConditionId))
                    deleteList.Add(eventCondition);
            }
            _unitOfWork.DbContext.RemoveRange(deleteList);
            return @event;
        }

    }
}
