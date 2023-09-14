using AppShared.ResourceParameters;
using AppShared.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using TaskDomain.Entities;

namespace TaskApplication.Contracts
{
   public  interface ITaskRepository : IAsyncRepository<Tasks>
    {
        //Fetch tASk based on PrIORITY
        //Task Due for Curent Week
        Task<BaseResponse> AddTaskToProject(Guid TaskId, Guid ProjectId);
        Task<BaseResponse> RemoveTaskFromProject(Guid TaskId, Guid ProjectId);
        //PagedList, //resourceParameters
        Task<List<Tasks>> GetTaskBased(TaskResourceParameters parameters);

        Task<BaseResponse> ChangeTasksStatus(Guid taskId, TaskStatuses taskStatus);
        Task<BaseResponse> AssignTasksToUser(string userEmailtobeAssignedTo, Guid taskId);
    }


    //Similar to Category
}


//Oagination
