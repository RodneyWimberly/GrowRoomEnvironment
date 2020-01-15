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
    public class DataPointController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DataPoint> _repository;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;

        public DataPointController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<DataPointController> logger, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<DataPoint>();
            _logger = logger;
            _authorizationService = authorizationService;
        }

        [HttpGet("{getDisabled:bool?}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<DataPointViewModel>))]
        public async Task<IActionResult> GetAll(bool getDisabled = true)
        {
            return await GetAllPaged(0, 1000, getDisabled);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}/{getDisabled:bool?}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<DataPointViewModel>))]
        public async Task<IActionResult> GetAllPaged(int pageNumber, int pageSize, bool getDisabled = true)
        {
            IPagedList<DataPoint> dataPoints = await _repository.GetPagedListAsync(predicate: d => getDisabled ? true : d.IsEnabled, pageIndex: pageNumber, pageSize: pageSize);
            return Ok(_mapper.Map<IEnumerable<DataPointViewModel>>(dataPoints.Items));
        }

        [HttpGet("{dataPointId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(DataPointViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int dataPointId)
        {
            DataPoint dataPoint = await _repository.FindAsync(dataPointId);
            if (dataPoint == null)
                return NotFound(dataPointId);
            else
                return Ok(_mapper.Map<DataPointViewModel>(dataPoint));
        }


        [HttpDelete("{dataPointId:int}")]
        [ProducesResponseType(200, Type = typeof(DataPointViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int dataPointId)
        {
            DataPoint dataPoint = await _repository.FindAsync(dataPointId);
            if (dataPoint == null)
                return NotFound(dataPointId);
            DataPointViewModel dataPointVM = _mapper.Map<DataPointViewModel>(dataPoint);
            _repository.Delete(dataPoint);
            await _unitOfWork.SaveChangesAsync();
            return Ok(dataPointVM);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(DataPointViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]DataPointViewModel dataPointVM)
        {
            if (ModelState.IsValid)
            {
                if (dataPointVM == null)
                    return BadRequest($"{nameof(dataPointVM)} cannot be null");
                DataPoint dataPoint = _mapper.Map<DataPoint>(dataPointVM);
                EntityEntry<DataPoint> addedDataPoint = await _repository.InsertAsync(dataPoint);
                await _unitOfWork.SaveChangesAsync();
                dataPointVM = _mapper.Map<DataPointViewModel>(addedDataPoint.Entity);
                return CreatedAtAction("GetByDataPointId", new { dataPointId = dataPointVM.DataPointId }, dataPointVM);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{dataPointId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put(int dataPointId, [FromBody]DataPointViewModel dataPointVM)
        {
            if (ModelState.IsValid)
            {
                if (dataPointVM == null)
                    return BadRequest($"{nameof(dataPointVM)} cannot be null");

                if (dataPointId != dataPointVM.DataPointId)
                    return BadRequest("Conflicting DataPoint DataPointId in parameter and model data");
                _repository.Update(_mapper.Map<DataPoint>(dataPointVM));
                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPatch("{dataPointId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Patch(int dataPointId, [FromBody]JsonPatchDocument<DataPointViewModel> patch)
        {
            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");

                DataPointViewModel dataPointVM = _mapper.Map<DataPointViewModel>(await _repository.FindAsync(dataPointId));
                patch.ApplyTo(dataPointVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    _repository.Update(_mapper.Map<DataPoint>(dataPointVM));
                    await _unitOfWork.SaveChangesAsync();
                    return NoContent();
                }
            }

            return BadRequest(ModelState);
        }
    }
}