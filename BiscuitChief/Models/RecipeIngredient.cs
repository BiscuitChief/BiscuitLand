using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BiscuitChief.Models
{
    public partial class RecipeIngredient
    {
        #region Public Properties

        public int IngredientID { get; set; }

        public string RecipeID { get; set; }

        [Display(Name = "Ingredient Name:")]
        [Required(ErrorMessage = "Please enter an Ingredient Name")]
        public string IngredientName { get; set; }

        [Display(Name = "Quantity:")]
        [Range(0, Double.MaxValue, ErrorMessage="Please enter a valid Quantity")]
        public Nullable<decimal> Quantity { get; set; }

        [Display(Name = "Quantity:")]
        public string DisplayQuantity { get; set; }

        [Display(Name = "Unit Of Measure:")]
        public string UnitOfMeasure { get; set; }

        [Display(Name = "Display Type:")]
        [Required(ErrorMessage = "Please enter a Display Type")]
        public Recipe.DisplayTypeCodes DisplayType { get; set; }

        [Display(Name = "Notes:")]
        public string Notes { get; set; }

        public int SortOrder { get; set; }

        #endregion

        #region Private Properties
        #endregion

    }
}