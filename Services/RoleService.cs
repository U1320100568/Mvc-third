using IntelligenceCloud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Services
{
    public class RoleService : CrudGenericService<Role>
    {
        public RoleService()
        {

        }

        public void LockRole(int id)
        {
            Role role = Get(r => r.RoleId == id);
            role.RLock = role.RLock == true ? false : true;
            Update(role);
        }
    }
}