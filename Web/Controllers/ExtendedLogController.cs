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
    public class ExtendedLogController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ExtendedLog> _repository;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;

        public ExtendedLogController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ExtendedLogController> logger, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<ExtendedLog>();
            _logger = logger;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Authorization.Policies.ViewLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExtendedLogViewModel>))]
        public async Task<IActionResult> GetAll()
        {
            return await GetAllPaged(0, 1000);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        [Authorize(Authorization.Policies.ViewLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExtendedLogViewModel>))]
        public async Task<IActionResult> GetAllPaged(int pageNumber, int pageSize)
        {
            IPagedList<ExtendedLog> extendedLogs = await _repository.GetPagedListAsync(pageIndex: pageNumber, pageSize: pageSize);
            return Ok(_mapper.Map<IEnumerable<ExtendedLogViewModel>>(extendedLogs.Items));
        }

        [HttpGet("level/{level}")]
        [Authorize(Authorization.Policies.ViewLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExtendedLogViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByLevel(int level)
        {
            return await GetByLevelPaged(level, 0, 1000);
        }

        [HttpGet("level/{level}/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Authorization.Policies.ViewLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ExtendedLogViewModel>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByLevelPaged(int level, int pageNumber, int pageSize)
        {
            IPagedList<ExtendedLog> extendedLogs = await _repository.GetPagedListAsync(l => l.Level == level, pageIndex: pageNumber, pageSize: pageSize);
            if (extendedLogs.Items.Count > 0)
                return Ok(_mapper.Map<IEnumerable<ExtendedLogViewModel>>(extendedLogs.Items));
            else
                return NotFound(level);
        }

        [HttpGet("{id:int}")]
        [Authorize(Authorization.Policies.ViewLogsPolicy)]
        [ProducesResponseType(200, Type = typeof(ExtendedLogViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id)
        {
            ExtendedLog extendedLog = await _repository.FindAsync(id);
            if (extendedLog == null)
                return NotFound(id);
            else
                return Ok(_mapper.Map<ExtendedLogViewModel>(extendedLog));
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        [Authorize(Authorization.Policies.ManageLogsPolicy)]
        public async Task<IActionResult> DeleteAll()
        {
            _unitOfWork.ExecuteSqlCommand("Delete from AppExtendedLogs");
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(200, Type = typeof(ExtendedLogViewModel))]
        [ProducesResponseType(404)]
        [Authorize(Authorization.Policies.ManageLogsPolicy)]
        public async Task<IActionResult> Delete(int id)
        {
            ExtendedLog extendedLog = await _repository.FindAsync(id);
            if (extendedLog == null)
                return NotFound(id);
            ExtendedLogViewModel extendedLogVM = _mapper.Map<ExtendedLogViewModel>(extendedLog);
            _repository.Delete(extendedLog);
            await _unitOfWork.SaveChangesAsync();
            return Ok(extendedLogVM);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ExtendedLogViewModel))]
        [ProducesResponseType(400)]
        [Authorize(Authorization.Policies.ManageLogsPolicy)]
        public async Task<IActionResult> Post([FromBody]ExtendedLogViewModel extendedLogVM)
        {
            if (ModelState.IsValid)
            {
                if (extendedLogVM == null)
                    return BadRequest($"{nameof(extendedLogVM)} cannot be null");
                ExtendedLog extendedLog = _mapper.Map<ExtendedLog>(extendedLogVM);
                EntityEntry<ExtendedLog> addedExtendedLog = await _repository.InsertAsync(extendedLog);
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
        [Authorize(Authorization.Policies.ManageLogsPolicy)]
        public async Task<IActionResult> Put(int id, [FromBody]ExtendedLogViewModel extendedLogVM)
        {
            if (ModelState.IsValid)
            {
                if (extendedLogVM == null)
                    return BadRequest($"{nameof(extendedLogVM)} cannot be null");

                if (id != extendedLogVM.Id)
                    return BadRequest("Conflicting ExtendedLog id in parameter and model data");
                _repository.Update(_mapper.Map<ExtendedLog>(extendedLogVM));
                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [Authorize(Authorization.Policies.ManageLogsPolicy)]
        public async Task<IActionResult> Patch(int id, [FromBody]JsonPatchDocument<ExtendedLogViewModel> patch)
        {
            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");

                ExtendedLogViewModel extendedLogVM = _mapper.Map<ExtendedLogViewModel>(await _repository.FindAsync(id));
                patch.ApplyTo(extendedLogVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    _repository.Update(_mapper.Map<ExtendedLog>(extendedLogVM));
                    await _unitOfWork.SaveChangesAsync();
                    return NoContent();
                }
            }

            return BadRequest(ModelState);
        }

    }
}