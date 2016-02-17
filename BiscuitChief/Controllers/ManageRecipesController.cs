using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiscuitChief.Controllers
{
    public class ManageRecipesController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Manage Recipes page.";

            if (User.IsInRole("ADMIN"))
            { ViewBag.Message = "Admin logged in"; }

            return View();
        }
    }
}