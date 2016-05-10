using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Configuration;


namespace BiscuitChief.Controllers
{
    public class RecipesController : Controller
    {
        // GET: Recipes
        public ActionResult Search()
        {
            Models.RecipeSearch searchdata = new Models.RecipeSearch();
            searchdata.SearchResults = new List<Models.Recipe>();
            searchdata.SearchCategoryList = Models.Recipe.Category.GetAllCategories();
            searchdata.PageCount = 1;
            searchdata.PageNumber = 1;
            searchdata.PageSize = 1;

            return View(searchdata);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        public ActionResult Search(Models.RecipeSearch searchdata)
        {
            if (ModelState.IsValid)
            {
                string [] categories = (from itm in searchdata.SearchCategoryList where itm.IsSelected select itm.CategoryCode).ToArray();
                List<Models.Recipe> allresults = Models.Recipe.SearchRecipes(searchdata.SearchText, new string[] { }, categories);
                searchdata.SearchResultText = allresults.Count.ToString() + " Recipies Found";
                searchdata.PageSize = 10;
                searchdata.PageCount = PortalUtility.PagerHelper.GetPageCount(searchdata.PageSize, allresults.Count);
                searchdata.PageNumber = PortalUtility.PagerHelper.CheckPageValid(searchdata.PageNumber, searchdata.PageCount);
                if (searchdata.PageNumber < 1)
                { searchdata.PageNumber = 1; }
                searchdata.SearchResults = new List<Models.Recipe>();

                if (allresults.Count > 0)
                {
                    int startindex = ((searchdata.PageNumber - 1) * searchdata.PageSize);
                    int range = searchdata.PageSize;
                    if (startindex + range > allresults.Count)
                    { range = allresults.Count - startindex; }
                    searchdata.SearchResults = allresults.GetRange(startindex, range);
                }
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

        [Authorize(Roles = "ADMIN")]
        public ActionResult Create(string recipeid = "")
        {
            Models.Recipe rcp = new Models.Recipe();
            if (String.IsNullOrEmpty(recipeid))
            {
                ViewBag.Title = "New Recipe";
            }
            else
            {
                ViewBag.Title = "Edit Recipe";
                rcp = new Models.Recipe(recipeid);
            }

            List<Models.Recipe.Category> allcat = Models.Recipe.Category.GetAllCategories();
            foreach (Models.Recipe.Category cat in rcp.CategoryList)
            { allcat.Find(act => act.CategoryCode == cat.CategoryCode).IsSelected = true; }
            rcp.CategoryList = allcat;

            return View(rcp);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "ADMIN")]
        public ActionResult Create(Models.Recipe rcp)
        {
            int index = 0;
            foreach (Models.RecipeIngredient ing in rcp.IngredientList)
            { ing.SortOrder = index++; }
            index = 0;
            foreach (Models.RecipeDirection dir in rcp.DirectionList)
            { dir.SortOrder = index++; }

            rcp.SaveRecipe();

            return View(rcp);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "ADMIN")]
        public ActionResult Ingredient_Add(Models.Recipe rcp)
        {
            rcp.IngredientList.Add(new Models.RecipeIngredient());
            return PartialView("PartialViews/CreateIngredientList", rcp);
        }
    }
}