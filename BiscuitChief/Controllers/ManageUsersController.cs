using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiscuitChief.Controllers
{
    public class ManageUsersController : Controller
    {
        // GET: ManaageUsers
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Index(BiscuitChief.Models.Login login, string ReturnUrl = "")
        {
            string resultmsg = string.Empty;

            resultmsg = login.AddNewUser();

            ViewBag.ResultMessage = resultmsg;
            return View(login);
        }
    }
}