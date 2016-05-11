using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiscuitChief.Controllers
{
    public class ManageUsersController : Controller
    {
        // GET: ManageUsers
        [Authorize(Roles="FULLACCESS")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost()]
        [Authorize(Roles = "FULLACCESS")]
        [ValidateAntiForgeryToken()]
        public ActionResult Index(BiscuitChief.Models.Login login, string ReturnUrl = "")
        {
            string resultmsg = string.Empty;

            if (User.IsInRole("ADMIN"))
            { resultmsg = login.AddNewUser(); }
            else
            { resultmsg = "Success"; }

            ViewBag.ResultMessage = resultmsg;
            return View(login);
        }
    }
}