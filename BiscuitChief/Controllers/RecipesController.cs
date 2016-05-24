using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Configuration;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Drawing;


namespace BiscuitChief.Controllers
{
    public class RecipesController : Controller
    {
        #region Search Recipe

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

        #endregion

        #region View Recipe

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

        #endregion

        #region Create Recipe

        [Authorize(Roles = "FULLACCESS")]
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
        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Create(Models.Recipe rcp)
        {
            if (User.IsInRole("ADMIN"))
            {
                int index = 0;
                foreach (Models.RecipeIngredient ing in rcp.IngredientList)
                { ing.SortOrder = index++; }
                index = 0;
                foreach (Models.RecipeDirection dir in rcp.DirectionList)
                { dir.SortOrder = index++; }

                rcp.SaveRecipe();
            }

            if (!String.IsNullOrEmpty(rcp.RecipeID))
            {
                return Redirect(Url.Action("Recipe", "Recipes", new { recipeid = rcp.RecipeID }));
            }
            else
            {
                ViewBag.Title = "New Recipe";
                return View(rcp);
            }
        }

        #region Ingredients

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Ingredient_Add(Models.Recipe rcp)
        {
            rcp.IngredientList.Add(new Models.RecipeIngredient());
            ModelState.Clear();
            return PartialView("PartialViews/CreateIngredientList", rcp);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Ingredient_MoveUp(Models.Recipe rcp, int _index)
        {
            if (rcp.IngredientList.Count - 1 > _index)
            {
                Models.RecipeIngredient temp = rcp.IngredientList[_index + 1];
                rcp.IngredientList[_index + 1] = rcp.IngredientList[_index];
                rcp.IngredientList[_index] = temp;
            }
            ModelState.Clear();
            return PartialView("PartialViews/CreateIngredientList", rcp);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Ingredient_MoveDown(Models.Recipe rcp, int _index)
        {
            if (rcp.IngredientList.Count - 1 > 0)
            {
                Models.RecipeIngredient temp = rcp.IngredientList[_index - 1];
                rcp.IngredientList[_index - 1] = rcp.IngredientList[_index];
                rcp.IngredientList[_index] = temp;
            }
            ModelState.Clear();
            return PartialView("PartialViews/CreateIngredientList", rcp);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Ingredient_Delete(Models.Recipe rcp, int _index)
        {
            if (rcp.IngredientList.Count > _index)
            { rcp.IngredientList.RemoveAt(_index); }
            ModelState.Clear();
            return PartialView("PartialViews/CreateIngredientList", rcp);
        }

        #endregion

        #region Directions

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Direction_Add(Models.Recipe rcp)
        {
            rcp.DirectionList.Add(new Models.RecipeDirection());
            ModelState.Clear();
            return PartialView("PartialViews/CreateDirectionList", rcp);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Direction_MoveUp(Models.Recipe rcp, int _index)
        {
            if (rcp.DirectionList.Count - 1 > _index)
            {
                Models.RecipeDirection temp = rcp.DirectionList[_index + 1];
                rcp.DirectionList[_index + 1] = rcp.DirectionList[_index];
                rcp.DirectionList[_index] = temp;
            }
            ModelState.Clear();
            return PartialView("PartialViews/CreateDirectionList", rcp);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Direction_MoveDown(Models.Recipe rcp, int _index)
        {
            if (rcp.DirectionList.Count - 1 > 0)
            {
                Models.RecipeDirection temp = rcp.DirectionList[_index - 1];
                rcp.DirectionList[_index - 1] = rcp.DirectionList[_index];
                rcp.DirectionList[_index] = temp;
            }
            ModelState.Clear();
            return PartialView("PartialViews/CreateDirectionList", rcp);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Direction_Delete(Models.Recipe rcp, int _index)
        {
            if (rcp.DirectionList.Count > _index)
            { rcp.DirectionList.RemoveAt(_index); }
            ModelState.Clear();
            return PartialView("PartialViews/CreateDirectionList", rcp);
        }

        #endregion

        [HttpPost]
        public async Task<JsonResult> UploadImage()
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        string path = Path.Combine(Server.MapPath("~/App_Data/Images"), "test.png");
                        Stream stream = fileContent.InputStream;
                        Image img = Image.FromStream(stream);
                        Image thumb = PortalUtility.ScaleImage(img, 100, 100);
                        thumb.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            return Json("File uploaded successfully");
        }

        #endregion

        #region Delete Recipe

        [Authorize(Roles = "FULLACCESS")]
        public ActionResult Delete(string recipeid = "")
        {
            if (User.IsInRole("ADMIN"))
            {
                Models.Recipe.DeleteRecipe(recipeid);
            }
            return Redirect(Url.Action("Search", "Recipes"));
        }

        #endregion
    }
}