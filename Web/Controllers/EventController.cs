using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.Web.ViewModels;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace GrowRoomEnvironment.Web.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;

        public EventController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<EventController> logger, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorizationService = authorizationService;
        }
        

        [HttpGet]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventViewModel>))]
        public async Task<IActionResult> GetAll()
        {
            return await GetAllPaged(-1, -1);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventViewModel>))]
        public async Task<IActionResult> GetAllPaged(int pageNumber, int pageSize)
        {
            IEnumerable<Event> events = await _unitOfWork.Events.GetAllAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<IEnumerable<EventViewModel>>(events));
        }

        [HttpGet("{actionDeviceId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByActionDeviceId(int actionDeviceId)
        {
            return await GetByActionDeviceIdPaged(actionDeviceId, -1, -1);
        }

        [HttpGet("{actionDeviceId:int}/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByActionDeviceIdPaged(int actionDeviceId, int pageNumber, int pageSize)
        {
            List<Event> events = (await _unitOfWork.Events.FindAsync(e => e.ActionDeviceId == actionDeviceId, pageNumber, pageSize)).ToList();
            if (events?.Count > 0)
                return Ok(_mapper.Map<IEnumerable<EventViewModel>>(events));
            else
                return NotFound(actionDeviceId);
        }


        [HttpGet("{eventId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(EventViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int eventId)
        {
            Event @event = await _unitOfWork.Events.GetAsync(eventId);
            if (@event == null)
                return NotFound(eventId);
            else
                return Ok(_mapper.Map<EventViewModel>(@event));
        }


        [HttpDelete("{eventId:int}")]
        [ProducesResponseType(200, Type = typeof(EventViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int eventId)
        {
            Event @event = await _unitOfWork.Events.GetAsync(eventId);
            if (@event == null)
                return NotFound(eventId);
            EventViewModel eventVM = _mapper.Map<EventViewModel>(@event);
            _unitOfWork.Events.Remove(@event);
            await _unitOfWork.SaveChangesAsync();
            return Ok(eventVM);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(EventViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]EventViewModel eventVM)
        {
            if (ModelState.IsValid)
            {
                if (eventVM == null)
                    return BadRequest($"{nameof(eventVM)} cannot be null");
                Event @event = _mapper.Map<Event>(eventVM);
                EntityEntry<Event> addedEvent = await _unitOfWork.Events.AddAsync(@event);
                await _unitOfWork.SaveChangesAsync();
                eventVM = _mapper.Map<EventViewModel>(addedEvent.Entity);
                return CreatedAtAction("GetByEventId", new { eventId = eventVM.EventId }, eventVM);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{eventId:int}")]
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

                Event @event = await _unitOfWork.Events.GetAsync(eventId);
                if (@event == null)
                    return BadRequest($"Cannot find Event with EventId: {eventId}");

               Event eventToUpdate = _mapper.Map(eventVM, @event);
                EntityEntry<Event> updatedEvent = _unitOfWork.Events.Update(eventToUpdate);
                await _unitOfWork.SaveChangesAsync();
                return Ok(_mapper.Map<EventViewModel>(updatedEvent.Entity));
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPatch("{eventId:int}")]
        [ProducesResponseType(200, Type = typeof(EventViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Patch(int eventId, [FromBody]JsonPatchDocument<EventViewModel> patch)
        {
            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");

                Event @event = await _unitOfWork.Events.GetAsync(eventId);
                EventViewModel eventVM = _mapper.Map<EventViewModel>(@event);
                patch.ApplyTo(eventVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    Event eventToUpdate = _mapper.Map(eventVM, @event);
                    EntityEntry<Event> updatedEvent = _unitOfWork.Events.Update(eventToUpdate);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok(updatedEvent.Entity);
                }
            }

            return BadRequest(ModelState);
        }

    }
}
