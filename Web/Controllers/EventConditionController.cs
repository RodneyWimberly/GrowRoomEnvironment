using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace GrowRoomEnvironment.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventConditionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;

        public EventConditionController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<EventConditionController> logger, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventConditionViewModel>))]
        public async Task<IActionResult> GetAllEventConditions()
        {
            return await GetEventConditions(-1, -1);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<EventConditionViewModel>))]
        public async Task<IActionResult> GetEventConditions(int pageNumber, int pageSize)
        {
            IEnumerable<EventCondition> eventConditions = await _unitOfWork.EventConditions.GetAllAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<IEnumerable<EventConditionViewModel>>(eventConditions));
        }

        [HttpGet("{eventConditionId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(EventConditionViewModel))]
        public async Task<IActionResult> GetByEventConditionId(int eventConditionId)
        {
            EventCondition eventCondition = await _unitOfWork.EventConditions.GetAsync(eventConditionId);
            return Ok(_mapper.Map<EventConditionViewModel>(eventCondition));
        }


        [HttpDelete("{eventConditionId:int}")]
        [ProducesResponseType(200, Type = typeof(EventConditionViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int eventConditionId)
        {
            EventCondition eventCondition = _unitOfWork.EventConditions.Get(eventConditionId);
            if (eventCondition == null)
                return NotFound(eventConditionId);
            EventConditionViewModel eventConditionVM = _mapper.Map<EventConditionViewModel>(eventCondition);
            _unitOfWork.EventConditions.Remove(eventCondition);
            await _unitOfWork.SaveChangesAsync();
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
                EventCondition eventCondition = _mapper.Map<EventCondition>(eventConditionVM);
                EntityEntry<EventCondition> addedEventCondition = await _unitOfWork.EventConditions.AddAsync(eventCondition);
                await _unitOfWork.SaveChangesAsync();
                eventConditionVM = _mapper.Map<EventConditionViewModel>(addedEventCondition.Entity);
                return CreatedAtAction("GetByEventConditionId", new { eventConditionId = eventConditionVM.EventConditionId }, eventConditionVM);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{eventConditionId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put(int eventConditionId, [FromBody]EventConditionViewModel eventConditionVM)
        {
            if (ModelState.IsValid)
            {
                if (eventConditionVM == null)
                    return BadRequest($"{nameof(eventConditionVM)} cannot be null");

                if (eventConditionId != eventConditionVM.EventConditionId)
                    return BadRequest("Conflicting EventCondition EventConditionId in parameter and model data");
                _unitOfWork.EventConditions.Update(_mapper.Map<EventCondition>(eventConditionVM));
                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPatch("{eventConditionId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Patch(int eventConditionId, [FromBody]JsonPatchDocument<EventConditionViewModel> patch)
        {
            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");

                EventConditionViewModel eventConditionVM = _mapper.Map<EventConditionViewModel>(await _unitOfWork.EventConditions.GetAsync(eventConditionId));
                patch.ApplyTo(eventConditionVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    _unitOfWork.EventConditions.Update(_mapper.Map<EventCondition>(eventConditionVM));
                    await _unitOfWork.SaveChangesAsync();
                    return NoContent();
                }
            }

            return BadRequest(ModelState);
        }

    }
}