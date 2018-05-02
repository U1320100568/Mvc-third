using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Models
{
    public partial class IntelligenceCloudEntities : DbContext
    {
        public int GetRoleNum()
        {
            var rawQuery = Database.SqlQuery<int>("SELECT NEXT VALUE FOR dbo.GetRoleNumSequence;");
            var task = rawQuery.SingleAsync();
            int nextVal = task.Result;
            return nextVal;
        }
    }
}