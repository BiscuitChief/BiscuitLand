using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiscuitChief.Models
{
    public partial class Recipe
    {
        #region Public Properties

        public string RecipeID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public List<RecipeIngredient> IngredientList { get; set; }

        public List<RecipeDirection> DirectionList { get; set; }

        public Dictionary<String, String> CategoryList { get; set; }

        #endregion

        #region Private Properties
        #endregion

        public enum DisplayTypeCodes { ING, DIR, HDR }
    }
}