﻿using IntelligenceCloud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace IntelligenceCloud.Services
{
    public class AccountService
    {
        private CrudRepository<Member> memberRepository;
        private CrudRepository<Role> roleRepository;
        public AccountService()
        {
            memberRepository = new CrudRepository<Member>();
            roleRepository = new CrudRepository<Role>();
        }

        public Member Login(Member member)
        {
            Member authorMember;
            
            if(String.IsNullOrWhiteSpace(member.MemberAccount) || String.IsNullOrWhiteSpace(member.MemberPwd))
            {
                authorMember = null;
            }
            authorMember = memberRepository.Get(m => m.MemberAccount == member.MemberAccount);
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
            if(member.Role.FirstOrDefault(r => 1 ==1 ).RoleLock ==false)
            {
                /////判斷權限等級
                if (member.Role.FirstOrDefault(r => 1 == 1).RoleAdmin ==true)
                {
                    userData = "admin,member";
                }
                else if(member.Role.FirstOrDefault(r => 1 == 1).RoleNormal == true)
                {
                    userData = "member";
                }
               
            }
            else
            {
                 userData = "guest";
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
            ////權限設定初始化
            Role role= new Role {
                RoleAdmin = false,
                RoleNormal = true,
                RoleLock = false,
                isDeleted = false
            };
            member.Role.Add(role);
            return member;
        }
        ////解除帳號的設定
        public Member DeleteAccountSetting(Member member)
        {
            member.Role.FirstOrDefault(r => 1 == 1).isDeleted = true;
            return member;
        }
    }
}