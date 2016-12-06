using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace BiscuitChief.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public string Contact(Models.ContactUs _contactinfo)
        {
            string result = String.Empty;

            string body = PortalUtility.GetEmailTemplate("ContactUs.txt");
            body = body.Replace("#NAME#", _contactinfo.FullName);
            body = body.Replace("#EMAIL#", _contactinfo.EmailAddress);
            body = body.Replace("#SUBJECT#", _contactinfo.Subject);
            body = body.Replace("#MESSAGE#", _contactinfo.Message);

            result = PortalUtility.SendEmail(_contactinfo.Subject, body);

            if (String.IsNullOrEmpty(result))
            { result = "Message Sent"; }

            return result;
        }

        //public ActionResult Logout()
        //{
        //    System.Web.Security.FormsAuthentication.SignOut();
        //    Session.Clear();
        //    return Redirect("/");
        //}
    }
}