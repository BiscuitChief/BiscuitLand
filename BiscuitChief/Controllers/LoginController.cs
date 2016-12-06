using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BiscuitChief.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Login page.";

            return View();
        }

        //[HttpPost()]
        //[ValidateAntiForgeryToken()]
        //public ActionResult Index(BiscuitChief.Models.Login login, string ReturnUrl = "")
        //{
        //    if (ModelState.IsValid)
        //    {

        //        bool isvalidlogin = Models.Login.ValidateLogin(login.UserName, login.Password);

        //        if (isvalidlogin)
        //        {
        //            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, login.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), true, "");
        //            String cookiecontents = FormsAuthentication.Encrypt(authTicket);
        //            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookiecontents) { Expires = authTicket.Expiration, Path = FormsAuthentication.FormsCookiePath };
        //            Response.Cookies.Add(cookie);

        //            if (!String.IsNullOrEmpty(ReturnUrl))
        //            { return Redirect(ReturnUrl); }
        //            else
        //            { return Redirect("/"); }
        //        }
        //        else
        //        {
        //            FormsAuthentication.SignOut();
        //            Session.Clear();
        //        }
        //    }

        //    return View(login);
        //}
    }
}