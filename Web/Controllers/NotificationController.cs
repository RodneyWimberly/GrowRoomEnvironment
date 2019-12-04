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
    public class NotificationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;

        public NotificationController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<NotificationController> logger, IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NotificationViewModel>))]
        public async Task<IActionResult> GetAllNotifications()
        {
            return await GetNotifications(-1, -1);
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NotificationViewModel>))]
        public async Task<IActionResult> GetNotifications(int pageNumber, int pageSize)
        {
            IEnumerable<Notification> notifications = await _unitOfWork.Notifications.GetAllAsync(pageNumber, pageSize);
            return Ok(_mapper.Map<IEnumerable<NotificationViewModel>>(notifications));
        }

        [HttpGet("{notificationId:int}")]
        //[Authorize(Authorization.Policies.)]
        [ProducesResponseType(200, Type = typeof(NotificationViewModel))]
        public async Task<IActionResult> GetByNotificationId(int notificationId)
        {
            Notification notification = await _unitOfWork.Notifications.GetAsync(notificationId);
            return Ok(_mapper.Map<NotificationViewModel>(notification));
        }


        [HttpDelete("{notificationId:int}")]
        [ProducesResponseType(200, Type = typeof(NotificationViewModel))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int notificationId)
        {
            Notification notification = await _unitOfWork.Notifications.GetAsync(notificationId);
            if (notification == null)
                return NotFound(notificationId);
            NotificationViewModel notificationVM = _mapper.Map<NotificationViewModel>(notification);
            _unitOfWork.Notifications.Remove(notification);
            await _unitOfWork.SaveChangesAsync();
            return Ok(notificationVM);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NotificationViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]NotificationViewModel notificationVM)
        {
            if (ModelState.IsValid)
            {
                if (notificationVM == null)
                    return BadRequest($"{nameof(notificationVM)} cannot be null");
                Notification notification = _mapper.Map<Notification>(notificationVM);
                EntityEntry<Notification> addedNotification = await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();
                notificationVM = _mapper.Map<NotificationViewModel>(addedNotification.Entity);
                return CreatedAtAction("GetByNotificationId", new { notificationId = notificationVM.NotificationId }, notificationVM);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{notificationId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put(int notificationId, [FromBody]NotificationViewModel notificationVM)
        {
            if (ModelState.IsValid)
            {
                if (notificationVM == null)
                    return BadRequest($"{nameof(notificationVM)} cannot be null");

                if (notificationId != notificationVM.NotificationId)
                    return BadRequest("Conflicting Notification NotificationId in parameter and model data");
                _unitOfWork.Notifications.Update(_mapper.Map<Notification>(notificationVM));
                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPatch("{notificationId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Patch(int notificationId, [FromBody]JsonPatchDocument<NotificationViewModel> patch)
        {
            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");

                NotificationViewModel notificationVM = _mapper.Map<NotificationViewModel>(await _unitOfWork.Notifications.GetAsync(notificationId));
                patch.ApplyTo(notificationVM, e => ModelState.AddModelError("", e.ErrorMessage));
                if (ModelState.IsValid)
                {
                    _unitOfWork.Notifications.Update(_mapper.Map<Notification>(notificationVM));
                    await _unitOfWork.SaveChangesAsync();
                    return NoContent();
                }
            }

            return BadRequest(ModelState);
        }
    }
}