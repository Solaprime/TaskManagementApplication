using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Entities
{
    //IdentityuSER REFACCTO
   public class ApplicationUser :  IdentityUser
    {
        public string Name { get; set; }
        public List<Tasks> TaskCreated { get; set; }
    }
}
