using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiscuitChief.Models;
using System.Web.Security;
using System.Web;

namespace BiscuitChief.api
{
    public class LoginController : ApiController
    {

        [Route("api/login")]
        [HttpPost]
        public IHttpActionResult Login(Login login)
        {
            try { PortalUtility.ValidateAntiForgeryToken(); }
            catch { return new PortalUtility.PlainTextResult("Invalid Request Token", HttpStatusCode.BadRequest); }
            
            bool isvalidlogin = Models.Login.ValidateLogin(login.UserName, login.Password);

            if (isvalidlogin)
            {
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, login.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), true, "");
                String cookiecontents = FormsAuthentication.Encrypt(authTicket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookiecontents) { Expires = authTicket.Expiration, Path = FormsAuthentication.FormsCookiePath };
                HttpContext.Current.Response.Cookies.Add(cookie);

                return Ok();
            }
            else
            {
                LogoutTasks();
                return new PortalUtility.PlainTextResult("Authentication Exception", HttpStatusCode.Unauthorized);
            }
        }

        /// <summary>
        /// Logout the user
        /// </summary>
        /// <returns></returns>
        [Route("api/logout")]
        [HttpPost]
        public IHttpActionResult Logout()
        {
            LogoutTasks();
            return Ok();
        }

        /// <summary>
        /// Logout method, created as a seperate method so we can call it if login fails or on logout
        /// It's only one method at this time but in the event that we have other objects we need to dispose of on logout we can do it here
        /// </summary>
        private void LogoutTasks()
        {
            FormsAuthentication.SignOut();
            try
            { HttpContext.Current.Session.Clear(); }
            catch { }
        }
    }
}
