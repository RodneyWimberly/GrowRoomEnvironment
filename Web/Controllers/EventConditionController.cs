using Arch.EntityFrameworkCore.UnitOfWork;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.Web.ViewModels;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.Web.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class EventConditionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<EventCondition> _repository;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;

        public EventConditionController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<EventConditionController> logger, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<EventCondition>();
            _logger = logger;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventConditionViewModel>))]
        public async Task<IActionResult> GetAll()
        {
            return await GetAllPaged(0, 1000);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventConditionViewModel>))]
        public async Task<IActionResult> GetAllPaged(int pageNumber, int pageSize)
        {
            IPagedList<EventCondition> eventConditions = await _repository.GetPagedListAsync(pageIndex: pageNumber, pageSize: pageSize);
            return Ok(_mapper.Map<IEnumerable<EventConditionViewModel>>(eventConditions.Items));
        }

        [HttpGet("{eventConditionId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(EventConditionViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int eventConditionId)
        {
            EventCondition eventCondition = await _repository.FindAsync(eventConditionId);
            if (eventCondition == null)
                return NotFound(eventConditionId);
            else
                return Ok(_mapper.Map<EventConditionViewModel>(eventCondition));
        }

        [HttpGet("{eventId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventConditionViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByEventId(int eventId)
        {
            return await GetByEventIdPaged(eventId, 0, 1000);
        }

        [HttpGet("{eventId:int}/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventConditionViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByEventIdPaged(int eventId, int pageNumber, int pageSize)
        {
            IPagedList<EventCondition> eventConditions = await _repository.GetPagedListAsync(e => e.EventId == eventId, pageIndex: pageNumber, pageSize: pageSize);
            if (eventConditions.Items.Count > 0)
                return Ok(_mapper.Map<IEnumerable<EventConditionViewModel>>(eventConditions.Items));
            else
                return NotFound(eventId);
        }

        [HttpGet("{dataPointId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventConditionViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByDataPointId(int dataPointId)
        {
            return await GetByDataPointIdPaged(dataPointId, 0, 1000);
        }

        [HttpGet("{dataPointId:int}/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventConditionViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByDataPointIdPaged(int dataPointId, int pageNumber, int pageSize)
        {
            IPagedList<EventCondition> eventConditions = await _repository.GetPagedListAsync(e => e.DataPointId == dataPointId, pageIndex: pageNumber, pageSize: pageSize);
            if (eventConditions.Items.Count > 0)
                return Ok(_mapper.Map<IEnumerable<EventConditionViewModel>>(eventConditions.Items));
            else
                return NotFound(dataPointId);
        }

        [HttpDelete("{eventConditionId:int}")]
        [ProducesResponseType(200, Type = typeof(EventConditionViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int eventConditionId)
        {
            EventCondition eventCondition = await _repository.FindAsync(eventConditionId);
            if (eventCondition == null)
                return NotFound(eventConditionId);

            _repository.Delete(eventCondition);
            await _unitOfWork.SaveChangesAsync();
            EventConditionViewModel eventConditionVM = _mapper.Map<EventConditionViewModel>(eventCondition);
            return Ok(eventConditionVM);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(EventConditionViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]EventConditionViewModel eventConditionVM)
        {
            if (ModelState.IsValid)
            {
                if (eventConditionVM == null)
                    return BadRequest($"{nameof(eventConditionVM)} cannot be null");
                if (eventConditionVM?.DataPoint?.DataPointId > 0)
                    eventConditionVM.DataPoint = null;
                if (eventConditionVM.Event != null)
                    eventConditionVM.Event = null;
                EventCondition eventCondition = _mapper.Map<EventCondition>(eventConditionVM);
                EntityEntry<EventCondition> addedEventCondition = await _repository.InsertAsync(eventCondition);
                await _unitOfWork.SaveChangesAsync();
                eventConditionVM = _mapper.Map<EventConditionViewModel>(addedEventCondition.Entity);
                return CreatedAtAction("GetByEventConditionId", new { eventConditionId = eventConditionVM.EventConditionId }, eventConditionVM);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{eventConditionId:int}")]
        [ProducesResponseType(200, Type = typeof(EventConditionViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put(int eventConditionId, [FromBody]EventConditionViewModel eventConditionVM)
        {
            if (ModelState.IsValid)
            {
                if (eventConditionVM == null)
                    return BadRequest($"{nameof(eventConditionVM)} cannot be null");

                if (eventConditionId != eventConditionVM.EventConditionId)
                    return BadRequest("Conflicting EventCondition EventConditionId in parameter and model data");

                _repository.Update(_mapper.Map<EventCondition>(eventConditionVM));
                await _unitOfWork.SaveChangesAsync();
                EventCondition updatedEventCondition = await _repository.FindAsync(eventConditionId);
                eventConditionVM = _mapper.Map<EventConditionViewModel>(updatedEventCondition);
                return Ok(eventConditionVM);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPatch("{eventConditionId:int}")]
        [ProducesResponseType(200, Type = typeof(EventConditionViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Patch(int eventConditionId, [FromBody]JsonPatchDocument<EventConditionViewModel> patch)
        {
            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");

                EventConditionViewModel eventConditionVM = _mapper.Map<EventConditionViewModel>(await _repository.FindAsync(eventConditionId));
                patch.ApplyTo(eventConditionVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    _repository.Update(_mapper.Map<EventCondition>(eventConditionVM));
                    await _unitOfWork.SaveChangesAsync();
                    EventCondition updatedEventCondition = await _repository.FindAsync(eventConditionId);
                    eventConditionVM = _mapper.Map<EventConditionViewModel>(updatedEventCondition);
                    return Ok(eventConditionVM);
                }
            }

            return BadRequest(ModelState);
        }

    }
}