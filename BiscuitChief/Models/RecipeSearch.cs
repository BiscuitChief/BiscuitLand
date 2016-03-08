using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BiscuitChief.Models
{
    public class RecipeSearch
    {
        #region Public Properties

        [Display(Name = "Search Text")]
        [MaxLength(100)]
        public string SearchText { get; set; }

        [Display(Name = "Ingredient List")]
        public List<string> SearchIngredientList { get; set; }

        public List<Recipe.Category> SearchCategoryList { get; set; }

        public List<Recipe> SearchResults { get; set; }

        public string SearchResultText { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int PageCount { get; set; }

        #endregion

        #region Private Properties
        #endregion
    }
}