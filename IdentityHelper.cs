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


        public static int? UserId
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
               if (UserId == null)
                {
                    return string.Empty;
                }
                else {
                    MemberService memberService= new MemberService();
                    return memberService.Get(m => m.MemberId == UserId ).MemberName;
                    
                }
            }
            
        }

        public static string GetMemberName(int? id)
        {
            if(id != null)
            {
                MemberService memberService = new MemberService();
                string name = memberService.Get(m => m.MemberId == id).MemberName;
                return name;
            }
            else
            {
                return string.Empty;
            }
            
        }

        

    }
}