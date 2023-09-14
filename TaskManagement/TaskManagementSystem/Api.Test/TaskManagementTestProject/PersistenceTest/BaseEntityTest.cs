using System;
using System.Collections.Generic;
using System.Text;
using TaskDomain.Entities;
using Xunit;

namespace TaskManagementTestProject.PersistenceTest
{
    public  class BaseEntityTest
    {
        private BaseEntity baseentityclass;
        public BaseEntityTest()
        {
            baseentityclass = new BaseEntity();
        }
        [Fact]
        public void BaseEntity_IsDeletedProperty_mustbeFalse()
        {
            Assert.False(baseentityclass.IsDeleted);
            // Assert.True(baseentityclass.ISdeleted);
        }
        
    }
}
