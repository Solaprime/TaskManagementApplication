﻿using AutoMapper;
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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectsController( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
         [HttpGet]
         public async Task<ActionResult<List<Project>>> GetAll()
        {
            var allProject = await _unitOfWork._projectRepository.ListAllAsync();
            return Ok(allProject);
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<Project>> GetSingle(Guid projectId)
        {
            var allProject = await _unitOfWork._projectRepository.GetByIdAsync(projectId);
            if (allProject == null)
            {
                return NotFound();
            }
            return Ok(allProject);
        }
        [HttpDelete("{projectId}", Name ="GetProjectById")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            var project = await _unitOfWork._projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }
            await _unitOfWork._projectRepository.SoftDeleteAsync(projectId);
            return NoContent();
        }
        //Update
        //cREATE
        // CreateProjectModel

        [HttpPost]
        public async Task<ActionResult<Project>> CreateNew(CreateProjectModel modelDto)
        {
            var projectEntity = _unitOfWork._mapper.Map<Project>(modelDto);
            await _unitOfWork._projectRepository.AddAsync(projectEntity);
          //  var taskToReturn = _mapper.Map<CreateTasksModel>(projectEntity);
            return CreatedAtRoute("GetById", new { Id = projectEntity.Id }, projectEntity);
        }

        //patch flow
        [HttpPatch("{projectId}")]
        public async Task<ActionResult> UpdateTask(Guid projectId,
         JsonPatchDocument<CreateProjectModel> patchDocument)
        {
            //Check if Product Exist 
            if (!await _unitOfWork._projectRepository.ExistAsync(projectId))
            {
                return NotFound();
            }
            var projectFromRepo = await _unitOfWork._projectRepository.GetByIdAsync(projectId);
            var projectToPatch = _unitOfWork._mapper.Map<CreateProjectModel>(projectFromRepo);
            // add validation
            patchDocument.ApplyTo(projectToPatch);
            //if (!TryValidateModel(projectToPatch))
            //{
            //    return ValidationProblem(ModelState);
            //}
            _unitOfWork._mapper.Map(projectToPatch, projectFromRepo);
            await _unitOfWork._projectRepository.UpdateAsync(projectFromRepo);

            return NoContent();
        }
    }
}
