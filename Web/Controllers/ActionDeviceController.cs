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
    public class ActionDeviceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;

        public ActionDeviceController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ActionDeviceController> logger, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ActionDeviceViewModel>))]
        public async Task<IActionResult> GetAll()
        {
            return await GetActionDevices(-1, -1);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ActionDeviceViewModel>))]
        public async Task<IActionResult> GetActionDevices(int pageNumber, int pageSize)
        {
            IEnumerable<ActionDevice> actionDevices = await _unitOfWork.ActionDevices.GetAllAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<IEnumerable<ActionDeviceViewModel>>(actionDevices));
        }

        [HttpGet("{actionDeviceId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(ActionDeviceViewModel))]
        public async Task<IActionResult> GetByActionDeviceId(int actionDeviceId)
        {
            ActionDevice actionDevice = await _unitOfWork.ActionDevices.GetAsync(actionDeviceId);
            return Ok(_mapper.Map<ActionDeviceViewModel>(actionDevice));
        }


        [HttpDelete("{actionDeviceId:int}")]
        [ProducesResponseType(200, Type = typeof(ActionDeviceViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int actionDeviceId)
        {
            ActionDevice actionDevice = _unitOfWork.ActionDevices.Get(actionDeviceId);
            if (actionDevice == null)
                return NotFound(actionDeviceId);
            ActionDeviceViewModel actionDeviceVM = _mapper.Map<ActionDeviceViewModel>(actionDevice);
            _unitOfWork.ActionDevices.Remove(actionDevice);
            await _unitOfWork.SaveChangesAsync();
            return Ok(actionDeviceVM);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ActionDeviceViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]ActionDeviceViewModel actionDeviceVM)
        {
            if (ModelState.IsValid)
            {
                if (actionDeviceVM == null)
                    return BadRequest($"{nameof(actionDeviceVM)} cannot be null");
                ActionDevice actionDevice = _mapper.Map<ActionDevice>(actionDeviceVM);
                EntityEntry<ActionDevice> addedActionDevice = await _unitOfWork.ActionDevices.AddAsync(actionDevice);
                await _unitOfWork.SaveChangesAsync();
                actionDeviceVM = _mapper.Map<ActionDeviceViewModel>(addedActionDevice.Entity);
                return CreatedAtAction("GetByActionDeviceId", new { actionDeviceId = actionDeviceVM.ActionDeviceId }, actionDeviceVM);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{actionDeviceId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put(int actionDeviceId, [FromBody]ActionDeviceViewModel actionDeviceVM)
        {
            if (ModelState.IsValid)
            {
                if (actionDeviceVM == null)
                    return BadRequest($"{nameof(actionDeviceVM)} cannot be null");

                if (actionDeviceId != actionDeviceVM.ActionDeviceId)
                    return BadRequest("Conflicting ActionDevice ActionDeviceId in parameter and model data");
                _unitOfWork.ActionDevices.Update(_mapper.Map<ActionDevice>(actionDeviceVM));
                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPatch("{actionDeviceId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Patch(int actionDeviceId, [FromBody]JsonPatchDocument<ActionDeviceViewModel> patch)
        {
            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");

                ActionDeviceViewModel actionDeviceVM = _mapper.Map<ActionDeviceViewModel>(await _unitOfWork.ActionDevices.GetAsync(actionDeviceId));
                patch.ApplyTo(actionDeviceVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    _unitOfWork.ActionDevices.Update(_mapper.Map<ActionDevice>(actionDeviceVM));
                    await _unitOfWork.SaveChangesAsync();
                    return NoContent();
                }
            }

            return BadRequest(ModelState);
        }

    }
}