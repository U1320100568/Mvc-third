using IntelligenceCloud.Models;
using IntelligenceCloud.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelligenceCloud.Services
{
    public class RoleService : CrudGenericService<Role>
    {
        private CrudGenericService<RoleMember> srv;
        public RoleService()
        {
            srv = new CrudGenericService<RoleMember>();
        }

        public void LockRole(int id)
        {
            Role role = Get(r => r.RoleId == id);
            role.RLock = role.RLock == true ? false : true;
            Update(role);
        }

        public IQueryable<RoleViewModel> GetRoleMember(int id)
        {
            //id = memberId

            List<RoleMember> roleMem;
            using (IntelligenceCloudEntities ctx = new IntelligenceCloudEntities())
            {
                roleMem = ctx.RoleMember.Where(rm => rm.MemberId == id).ToList();
            }
            var result = roleMem.Select(rm => new RoleViewModel()
            {
                RoleMem = rm,
                RoleName = Get(r => r.RoleId == rm.RoleId).RName
            });
            return result.AsQueryable() ;
        }

        public IQueryable<Role> GetRestRole(int id)
        {
            List<int> restRole;
            using (IntelligenceCloudEntities ctx = new IntelligenceCloudEntities())
            {
                restRole = ctx.RoleMember.Where(rm => rm.MemberId == id).Select(rm => rm.RoleId).ToList();
            }
            return GetAll().Where(r => !restRole.Contains(r.RoleId));
        }

        public void MemberAddRole(int memberId, int roleId)
        {
            RoleMember roleMember = new RoleMember()
            {
                RoleId = roleId,
                MemberId = memberId
            };
            srv.Create(roleMember);
        }

        public void MemberRemoveRole(int roleMemberId)
        {
            RoleMember roleMember = srv.Get(rm => rm.RMNum == roleMemberId);
            srv.Delete(roleMember);
        }

        
    }
}