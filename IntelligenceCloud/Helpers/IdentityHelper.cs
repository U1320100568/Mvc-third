using IntelligenceCloud.Models;
using IntelligenceCloud.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;

namespace IntelligenceCloud.Helpers
{
    public class IdentityHelper
    {


        public static int? MemberId
        {
            get
            {
                var httpContext = HttpContext.Current;
                var identity = httpContext.User.Identity as FormsIdentity;
                int result;
                if(identity == null)
                {
                    return null;
                }
                if (int.TryParse( identity.Name,out  result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
                

            }
        }

        public static string UserName
        {
            get
            {
               if (MemberId == null)
                {
                    return string.Empty;
                }
                else {
                    CrudRepository<Member> memberRepository = new CrudRepository<Member>();
                    return memberRepository.Get(m => m.MemberId == MemberId ).MemberName;
                    
                }
            }
            
        }

        public static Role GetFeature()
        {
            if (MemberId == null)
            {
                return new Role() {RoleLock =true };
            }
            else
            {
                CrudRepository<Role> roleRepository = new CrudRepository<Role>();
                return  roleRepository.Get(r => r.MemberId == MemberId);
                
            }
        }

        public static string  GetAccessFeature()
        {
            if (MemberId == null)
            {
                return string.Empty;
            }
            else
            {
                CrudRepository<Role> roleRepository = new CrudRepository<Role>();
                Role role = roleRepository.Get(r => r.MemberId == MemberId);

                string stringWithout = "RoleId MemberId RoleLock isDeleted";
                IEnumerable<PropertyInfo> propWithout = typeof(Role).GetProperties().Where(p => stringWithout.Contains(p.Name));
                var props = typeof(Role).GetProperties().Except(propWithout);
                foreach(var prop in props)
                {
                    if(prop.GetValue(role) as bool?== true)
                    {
                        return prop.Name;
                    }
                }
                

            }
            return string.Empty;
        }

    }
}