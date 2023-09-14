using System;
using System.Collections.Generic;
using System.Text;
using TaskDomain.Entities;

namespace TaskPersistence.Repositories
{
    class ProjectRepository : BaseRepository<Project>
    {
        public ProjectRepository(PersistentDBContext dbContext) : base(dbContext)
        {
            //EndPoint to GET A Task With the Project 
        }
    }
}
