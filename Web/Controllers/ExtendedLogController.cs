using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GrowRoomEnvironment.DataAccess.Core.Enums;
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
    //[Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ExtendedLogController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;

        public ExtendedLogController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ExtendedLogController> logger,  IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExtendedLogViewModel>))]
        public async Task<IActionResult> GetAll()
        {
            return await GetExtendedLogs(-1, -1);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExtendedLogViewModel>))]
        public async Task<IActionResult> GetExtendedLogs(int pageNumber, int pageSize)
        {
            IEnumerable<ExtendedLog> extendedLogs = await _unitOfWork.ExtendedLogs.GetAllAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<IEnumerable<ExtendedLogViewModel>>(extendedLogs));
        }

        [HttpGet("level/{level}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExtendedLogViewModel>))]
        public async Task<IActionResult> GetAllByLevel(int level)
        {
            return await GetExtendedLogsByLevel(level, -1, -1);
        }

        [HttpGet("level/{level}/{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExtendedLogViewModel>))]
        public async Task<IActionResult> GetExtendedLogsByLevel(int level, int pageNumber, int pageSize)
        {
            IEnumerable<ExtendedLog> extendedLogs = await _unitOfWork.ExtendedLogs.FindAsync(l => l.Level == level, pageNumber, pageSize);
            return Ok(_mapper.Map<IEnumerable<ExtendedLogViewModel>>(extendedLogs));
        }

        [HttpGet("{id:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(ExtendedLogViewModel))]
        public async Task<IActionResult> GetById(int id)
        {
            ExtendedLog extendedLog = await _unitOfWork.ExtendedLogs.GetAsync(id);
            return Ok(_mapper.Map<ExtendedLogViewModel>(extendedLog));
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteAll()
        {
            await _unitOfWork.ExtendedLogs.ClearAllAsync();
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ExtendedLogViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            ExtendedLog extendedLog = _unitOfWork.ExtendedLogs.Get(id);
            if(extendedLog == null)
                return NotFound(id);
            ExtendedLogViewModel extendedLogVM = _mapper.Map<ExtendedLogViewModel>(extendedLog);
            _unitOfWork.ExtendedLogs.Remove(extendedLog);
            await _unitOfWork.SaveChangesAsync();
            return Ok(extendedLogVM);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ExtendedLogViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]ExtendedLogViewModel extendedLogVM)
        {
            if (ModelState.IsValid)
            {
                if (extendedLogVM == null)
                    return BadRequest($"{nameof(extendedLogVM)} cannot be null");
                ExtendedLog extendedLog = _mapper.Map<ExtendedLog>(extendedLogVM);
                EntityEntry<ExtendedLog> addedExtendedLog = await _unitOfWork.ExtendedLogs.AddAsync(extendedLog);
                await _unitOfWork.SaveChangesAsync();
                extendedLogVM = _mapper.Map<ExtendedLogViewModel>(addedExtendedLog.Entity);
                return CreatedAtAction("GetById", new { id = extendedLogVM.Id }, extendedLogVM);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put(int id, [FromBody]ExtendedLogViewModel extendedLogVM)
        {
            if (ModelState.IsValid)
            {
                if (extendedLogVM == null)
                    return BadRequest($"{nameof(extendedLogVM)} cannot be null");

                if (id != extendedLogVM.Id)
                    return BadRequest("Conflicting ExtendedLog id in parameter and model data");
                _unitOfWork.ExtendedLogs.Update(_mapper.Map<ExtendedLog>(extendedLogVM));
                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Patch(int id, [FromBody]JsonPatchDocument<ExtendedLogViewModel> patch)
        {
            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");

                ExtendedLogViewModel extendedLogVM = _mapper.Map<ExtendedLogViewModel>(await _unitOfWork.ExtendedLogs.GetAsync(id));
                patch.ApplyTo(extendedLogVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    _unitOfWork.ExtendedLogs.Update(_mapper.Map<ExtendedLog>(extendedLogVM));
                    await _unitOfWork.SaveChangesAsync();
                    return NoContent();
                }
            }
            
            return BadRequest(ModelState);
        }

    }
}