using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiscuitChief.Models
{
    public partial class RecipeIngredient
    {
        #region Public Properties

        public int IngredientID { get; set; }

        public int RecipeID { get; set; }

        public string IngredientName { get; set; }

        public decimal Quantity { get; set; }

        public string DisplayQuantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public Recipe.DisplayTypeCodes DisplayType { get; set; }

        public string Notes { get; set; }

        public int SortOrder { get; set; }

        #endregion

        #region Private Properties
        #endregion
    }
}