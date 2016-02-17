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
        [Authorize(Roles="ADMIN")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost()]
        [Authorize(Roles = "ADMIN")]
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