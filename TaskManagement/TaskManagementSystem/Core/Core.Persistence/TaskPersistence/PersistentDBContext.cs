using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskDomain.Entities;

namespace TaskPersistence
{
   public  class PersistentDBContext : IdentityDbContext<ApplicationUser>

    {
        public PersistentDBContext(DbContextOptions<PersistentDBContext> options) : base(options)
        {

        }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        //  public DbSet<Notification> Projects { get; set; }
        //  public DbSet<Notification> Projects { get; set; }


    }
}
