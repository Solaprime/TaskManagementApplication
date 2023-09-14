using AppShared.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskApplication.Contracts;
using TaskApplication.Models;
using TaskDomain.Entities;

namespace TaskManagemantApi.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        //Crud Notfication
        //Mark Notification as read Or Unread
        private readonly INotificationRepository _notifcationRepository;
        private readonly IMapper _mapper;
        public NotificationController(INotificationRepository notifcationRepository, IMapper mapper)
        {
            _notifcationRepository = notifcationRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Notfication
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Notification>>> GetAllNotification()
        {
            var allNotification = await _notifcationRepository.ListAllAsync();
            return Ok(allNotification);
        }
        /// <summary>
        /// Get notficationById
        /// </summary>
        /// <param name="Id">The Id of the notification in Guid</param>
        /// <returns></returns>
        [HttpGet("{Id}", Name = "GetNotificationById")]
        public async Task<ActionResult<Notification>> GetNotificationById(Guid Id)
        {
            var notification = await _notifcationRepository.GetByIdAsync(Id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }
        /// <summary>
        /// Sot delete Notfication
        /// </summary>
        /// <param name="Id">The Id of the notification in Guid</param>
        /// <returns></returns>
        [HttpDelete("{Id}")]
        public async Task<IActionResult> SoftDeleteNotification(Guid Id)
        {
            var notification = await _notifcationRepository.GetByIdAsync(Id);
            if (notification == null)
            {
                return NotFound();
            }
            await _notifcationRepository.SoftDeleteAsync(Id);
            return NoContent();

        }
        /// <summary>
        /// partilayy Update Notfication
        /// </summary>
        /// <param name="Id">>The Id of the notification in Guid</param>
        /// <param name="patchDocument">Json Patch Document</param>
        /// <returns></returns>
        [HttpPatch("{Id}")]
        public async Task<ActionResult> UpdateNotification(Guid Id,
           JsonPatchDocument<NotificationModel> patchDocument)
        {
            //Check if Product Exist 
            if (!await _notifcationRepository.ExistAsync(Id))
            {
                return NotFound();
            }
            var notificationFromRepo = await _notifcationRepository.GetByIdAsync(Id);
            var notificationToPatch = _mapper.Map<NotificationModel>(notificationFromRepo);
            // add validation, Since we dont have Validation on our Models
            // patchDocument.ApplyTo(taskToPatch, ModelState);
            patchDocument.ApplyTo(notificationToPatch);
            //if (!TryValidateModel(taskToPatch))
            //{
            //    return ValidationProblem(ModelState);
            //}
            _mapper.Map(notificationToPatch, notificationFromRepo);
            await _notifcationRepository.UpdateAsync(notificationFromRepo);
            return NoContent();
        }
        /// <summary>
        /// Create a new Notification
        /// </summary>
        /// <param name="modelDto">We pass in a model we use to create notfication</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Tasks>> CreateNewNotification(NotificationModel modelDto)
        {
            var notificationEntity = _mapper.Map<Notification>(modelDto);
            await _notifcationRepository.AddAsync(notificationEntity);
            var taskToReturn = _mapper.Map<NotificationModel>(notificationEntity);
            return CreatedAtRoute("GetNotificationById", new { Id = notificationEntity.Id }, notificationEntity);
           
        }
        /// <summary>
        /// Change the status of the Notfication
        /// </summary>
        /// <param name="notificationId">Tthe Id of the notification</param>
        /// <param name="notifyStatus">The status of the notification in int </param>
        /// <returns></returns>
        [HttpPost("ChangeNotificationStatus/{notificationId}/{notifyStatus}")]
        //Enum Flow
        public async Task<ActionResult<BaseResponse>> ChangeNotificationStatus(Guid notificationId, NotificationStatus notifyStatus)
        {
            var result = await _notifcationRepository.ChangeNotficationStatus(notificationId, notifyStatus);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
//U need to create the HangFIreDbNotificatiom