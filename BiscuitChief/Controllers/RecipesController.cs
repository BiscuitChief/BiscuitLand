using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

namespace BiscuitChief.Controllers
{
    public class RecipesController : Controller
    {
        // GET: Recipes
        public ActionResult Search()
        {
            Models.RecipeSearch searchdata = new Models.RecipeSearch();
            searchdata.SearchResults = new List<Models.Recipe>();
            return View(searchdata);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Search(Models.RecipeSearch searchdata)
        {
            if (ModelState.IsValid)
            {
                searchdata.SearchResults = Models.Recipe.SearchRecipes(searchdata.SearchText, new string[] { });
                searchdata.SearchResultText = searchdata.SearchResults.Count.ToString() + " Recipies Found";
            }
            return View(searchdata);
        }

        public ActionResult Recipe(string recipeid, decimal quantity = 1)
        {
            List<SelectListItem> qtyvalues = new List<SelectListItem>();
            qtyvalues.Add(new SelectListItem() { Text = "1/2", Value = ".5" });
            qtyvalues.Add(new SelectListItem() { Text = "1", Value = "1" });
            qtyvalues.Add(new SelectListItem() { Text = "1 1/2", Value = "1.5" });
            qtyvalues.Add(new SelectListItem() { Text = "2", Value = "2" });
            qtyvalues.Add(new SelectListItem() { Text = "2 1/2", Value = "2.5" });
            qtyvalues.Add(new SelectListItem() { Text = "3", Value = "3" });
            qtyvalues.Add(new SelectListItem() { Text = "4", Value = "4" });
            qtyvalues.Add(new SelectListItem() { Text = "5", Value = "5" });
            ViewBag.QuantityValues = qtyvalues;

            Models.Recipe rcp = new Models.Recipe(recipeid, quantity);

            return View(rcp);
        }
    }
}