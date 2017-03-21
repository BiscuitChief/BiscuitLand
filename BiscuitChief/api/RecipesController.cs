using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BiscuitChief.Models;

namespace BiscuitChief.api
{
    public class RecipesController : ApiController
    {
        [Route("api/recipes/categories")]
        [HttpGet]
        public IHttpActionResult GetAllCategories()
        {
            try
            {
                List<Models.Recipe.Category> categories = Models.Recipe.Category.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                PortalUtility.SendErrorEmail(ex);
                return new PortalUtility.PlainTextResult(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        [Route("api/recipes/search")]
        [HttpGet]
        public IHttpActionResult Search(Models.RecipeSearch searchdata)
        {
            try
            {
                string[] categories = (from itm in searchdata.SearchCategoryList where itm.IsSelected select itm.CategoryCode).ToArray();
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

                return Ok(searchdata);
            }
            catch (Exception ex)
            {
                PortalUtility.SendErrorEmail(ex);
                return new PortalUtility.PlainTextResult(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
