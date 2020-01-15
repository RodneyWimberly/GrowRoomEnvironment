using Arch.EntityFrameworkCore.UnitOfWork;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using GrowRoomEnvironment.DataAccess.Core.Enums;
using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.Web.ViewModels;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.Web.Controllers
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ActionDeviceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ActionDevice> _repository;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;

        public ActionDeviceController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ActionDeviceController> logger, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GetRepository<ActionDevice>();
            _logger = logger;
            _authorizationService = authorizationService;
        }

        [HttpGet("{getDisabled:bool?}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ActionDeviceViewModel>))]
        public async Task<IActionResult> GetAll(bool getDisabled = true)
        {
            return await GetAllPaged(0, 1000, getDisabled);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}/{getDisabled:bool?}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ActionDeviceViewModel>))]
        public async Task<IActionResult> GetAllPaged(int pageNumber, int pageSize, bool getDisabled = true)
        {
            IPagedList<ActionDevice> actionDevices = await _repository.GetPagedListAsync(predicate: a => getDisabled ? true : a.IsEnabled, pageIndex: pageNumber, pageSize: pageSize);
            return Ok(_mapper.Map<IEnumerable<ActionDeviceViewModel>>(actionDevices.Items));
        }

        [HttpGet("{actionDeviceId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(ActionDeviceViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int actionDeviceId)
        {
            ActionDevice actionDevice = await _repository.FindAsync(actionDeviceId);
            if (actionDevice == null)
                return NotFound(actionDeviceId);
            else
                return Ok(_mapper.Map<ActionDeviceViewModel>(actionDevice));
        }


        [HttpDelete("{actionDeviceId:int}")]
        [ProducesResponseType(200, Type = typeof(ActionDeviceViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int actionDeviceId)
        {
            ActionDevice actionDevice = await _repository.FindAsync(actionDeviceId);
            if (actionDevice == null)
                return NotFound(actionDeviceId);
            ActionDeviceViewModel actionDeviceVM = _mapper.Map<ActionDeviceViewModel>(actionDevice);
            _repository.Delete(actionDevice);
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
                EntityEntry<ActionDevice> addedActionDevice = await _repository.InsertAsync(actionDevice);
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
                _repository.Update(_mapper.Map<ActionDevice>(actionDeviceVM));
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

                ActionDeviceViewModel actionDeviceVM = _mapper.Map<ActionDeviceViewModel>(await _repository.FindAsync(actionDeviceId));
                patch.ApplyTo(actionDeviceVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    _repository.Update(_mapper.Map<ActionDevice>(actionDeviceVM));
                    await _unitOfWork.SaveChangesAsync();
                    return NoContent();
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPut("SetState/{actionDeviceId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutState(int actionDeviceId, [FromBody]ActionDeviceStates state)
        {
            ActionDevice actionDevice = await _repository.FindAsync(actionDeviceId);
            if (actionDevice == null)
                return NotFound(actionDeviceId);
            dynamic parameters = JObject.Parse(actionDevice.Parameters);
            switch (actionDevice.Type)
            {
                case ActionDeviceTypes.iHome:

                    string id = parameters.id;
                    using (HttpClient client = new HttpClient())
                    {
                        using HttpRequestMessage request = new HttpRequestMessage();
                        string json = "[{\"value\":\"" + state + "\"}]";
                        request.RequestUri = new Uri($"https://api.evrythng.com/thngs/{id}/properties/targetpowerstate1", UriKind.RelativeOrAbsolute);
                        request.Method = HttpMethod.Put;
                        request.Content = new StringContent(json);
                        request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                        request.Content.Headers.ContentLength = json.Length;
                        request.Content.Headers.Add("Authorization", "z8RdfbcZfXDiIxxilVmiqg4fMr154fVoxKTor2V7CDWkOP7iiSjf7NPb9Qq8siLaXmxtJOmbeVbQw1GZ");
                        request.Content.Headers.Add("Accept", "application/json");
                        await client.SendAsync(request);
                    }
                    break;
                case ActionDeviceTypes.WiFi:
                    string url = parameters.url + state;
                    using (HttpClient client = new HttpClient())
                    {
                        using HttpRequestMessage request = new HttpRequestMessage();
                        request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);
                        request.Method = HttpMethod.Get;
                        await client.SendAsync(request);
                    }
                    break;
            }

            return NoContent();
        }
    }
}