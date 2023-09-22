using AppShared.ResourceParameters;
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
using TaskManagemantApi.Repositories;

namespace TaskManagemantApi.Controllers.V1
{
  /// <summary>
  /// Task Controller to CRUD Task  amd do some task related operation
  /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public TasksController( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Get All Task in our Db
        /// </summary>
        /// <returns></returns>
        [HttpGet]  
        public async Task<ActionResult<List<Tasks>>> GetAllTask()
        {
            var allTask = await _unitOfWork._taskRepository.ListAllAsync();
            return Ok(allTask);
        }
        /// <summary>
        /// Get task by Id, Id is a guid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
       [HttpGet("{Id}" , Name ="GetTaskById")]
       public async Task<ActionResult<Tasks>> GetTaskById(Guid Id)
        {
            var task = await _unitOfWork._taskRepository.GetByIdAsync(Id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }
        /// <summary>
        /// Delete a task from our Db, These function soft-Deletes a Task
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("{Id}")]
        public async Task<IActionResult> SoftDeleteTask(Guid Id)
        {
            var task = await _unitOfWork._taskRepository.GetByIdAsync(Id);
            if (task == null)
            {
                return NotFound();
            }
            await _unitOfWork._taskRepository.SoftDeleteAsync(Id);
            return NoContent();
        }
        /// <summary>
        /// Partially Updates a Task, it takes a taskId of type guid, and Json PatchDocument
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{taskId}")]
        public async Task<ActionResult> UpdateTask(Guid taskId,
           JsonPatchDocument<CreateTasksModel> patchDocument)
        {
            if (!await _unitOfWork._taskRepository.ExistAsync(taskId))
            {
                return NotFound();
            }
            var taskFromRepo = await _unitOfWork._taskRepository.GetByIdAsync(taskId);
            var taskToPatch = _unitOfWork._mapper.Map<CreateTasksModel>(taskFromRepo);
            // add validation, Since we dont have Validation on our Models
           // patchDocument.ApplyTo(taskToPatch, ModelState);
            patchDocument.ApplyTo(taskToPatch);
            //if (!TryValidateModel(taskToPatch))
            //{
            //    return ValidationProblem(ModelState);
            //}
            _unitOfWork._mapper.Map(taskToPatch, taskFromRepo);
             await _unitOfWork._taskRepository.UpdateAsync(taskFromRepo);
            return NoContent();
        }
        /// <summary>
        /// /These Endpoint created a new task, it takes in a model dto
        /// </summary>
        /// <param name="modelDto"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult<Tasks>> CreateNewTask(CreateTasksModel modelDto)
        {
            var taskEntity = _unitOfWork._mapper.Map<Tasks>(modelDto);
            await _unitOfWork._taskRepository.AddAsync(taskEntity);
            var taskToReturn = _unitOfWork._mapper.Map<CreateTasksModel>(taskEntity);
             return CreatedAtRoute("GetTaskById", new { Id = taskEntity.Id }, taskEntity);
          
        }
        /// <summary>
        /// Get Task  based On Parameters either by TaskPriority, TaskStatus, DueForCurrent Parameter
        /// DueCurrentWeek Parameter specifes  DueTask in less than a   given parameters
        /// to retrieve
        /// </summary>
        /// <param name="taskResource"></param>
        /// <returns></returns>
        [HttpGet("GetTaskBased")]
        public async Task<ActionResult<List<Tasks>>> GetTaskBasedOnParameters([FromQuery] TaskResourceParameters taskResource)
        {
            var taskToReturn = await _unitOfWork._taskRepository.GetTaskBased(taskResource);
            if (taskToReturn == null)
            {
                return BadRequest("Sorry we cant find what you are looking for");
            }
            return Ok(taskToReturn);
        }
        /// <summary>
        /// Get Task Due in less than a   week
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTaskDueInaWeek")]
        public async Task<ActionResult<List<Tasks>>> GetTaskDueInaWeek()
        {
            var parameters = new TaskResourceParameters() { DueForCurrent = 7 };
            var taskToReturn = await _unitOfWork._taskRepository.GetTaskBased(parameters);
            if (taskToReturn == null)
            {
                return BadRequest("Sorry No task due for current Week");
            }
            return Ok(taskToReturn);
        }
        /// <summary>
        /// Add a Task To a project
        /// </summary>
        /// <param name="taskId">The id of the task</param>
        /// <param name="projectId">The id of the project </param>
        /// <returns></returns>
        [HttpPost("AddTaskToProject/{taskId}/{projectId}")]
        public async Task<ActionResult<BaseResponse>> AddTaskToProject(Guid taskId, Guid projectId)
        { 
            var result = await _unitOfWork._taskRepository.AddTaskToProject(taskId, projectId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);    
        }
       /// <summary>
       /// Change the Statud of a Tasj
       /// </summary>
       /// <param name="taskId">The id of the task </param>
       /// <param name="taskStatus">the status in int</param>
       /// <returns></returns>
        [HttpPost("ChangeTaskStatus/{taskId}/{taskStatus}")]
        public async Task<ActionResult<BaseResponse>> ChangeTaskStatus(Guid taskId, TaskStatuses taskStatus)
        {
            var result = await _unitOfWork._taskRepository.ChangeTasksStatus(taskId, taskStatus);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        /// <summary>
        /// Assings a Task to a User
        /// </summary>
        /// <param name="taskId"> taskId of the task u wish to assign to someone</param>
        /// <param name="email">email of the user </param>
        /// <returns></returns>
        
        [HttpPost("AssignTaskToUser/{taskId}/{email}")]
        public async Task<ActionResult<BaseResponse>> AssignTaskToUser(Guid taskId, string email)
        {
            var result = await _unitOfWork._taskRepository.AssignTasksToUser(email, taskId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }


    }
}

//Patch not working,
//Test GeTTask Due In a Current Weak, TestING fLOW with Kelvin Docks Datetime offset
//get Task Priority Not Working 


