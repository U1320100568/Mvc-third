using IntelligenceCloud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace IntelligenceCloud.Services
{
    public class MemberService : CrudGenericService<Member>
    {
        private RoleService roleSrv;
        public MemberService()
        {
            roleSrv = new RoleService();
        }

        public Member Login(Member member)
        {
            Member authorMember;
            
            if(String.IsNullOrWhiteSpace(member.MemberAccount) || String.IsNullOrWhiteSpace(member.MemberPwd))
            {
                authorMember = null;
            }
            authorMember = Get(m => m.MemberAccount == member.MemberAccount);
            if (authorMember != null)
            {
                ////判斷密碼相同  &  帳號是否被刪除
                if (authorMember.MemberPwd != member.MemberPwd && member.isDeleted == false)
                {
                    authorMember = null;
                }
                else
                {
                    ///////身分驗證通過
                    Authentication(authorMember);
                }
            }
            return authorMember;
        }

        public void Logout()
        {
            //Cancel Authentitcation 身分註銷
            FormsAuthentication.SignOut();
            HttpContext.Current.Session.RemoveAll();
            // 建立一個同名的 Cookie 來覆蓋原本的 Cookie
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(cookie);

            // 建立 ASP.NET 的 Session Cookie 同樣是為了覆蓋
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(cookie2);
            //Cancel Authentication
            
            
        }

        ///////身分註冊
        public void Authentication(Member member)
        {
            var now = DateTime.Now;
            string userData = "guest";

            ///登入角色權限
            ///判斷權限被鎖住

            /////加入使用者擁有的權限
            /**/
            var roles = roleSrv.GetRoleMember(member.MemberId);
            foreach(var role in roles)
            {
                    userData += ","+role.RoleName;
            }
                
               
            
            
           
            
            var ticket = new FormsAuthenticationTicket(
                    version: 1,
                    name: member.MemberId.ToString(),
                    issueDate: now,
                    expiration: now.AddMinutes(30),
                    isPersistent: false,
                    userData: userData,
                    cookiePath: FormsAuthentication.FormsCookiePath
                );
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        ////建立帳號的初始設定
        public Member CreateAccountSetting(Member member)
        {
            
            return member;
            
        }
        ////解除帳號的設定
        public Member DeleteAccountSetting(Member member)
        {
            
            return member;
        }
    }
}