using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BiscuitChief.Models
{
    public partial class RecipeDirection
    {
        #region Public Properties

        public int DirectionID { get; set; }

        public string RecipeID { get; set; }

        public int SortOrder { get; set; }

        [Display(Name = "Direction Text:")]
        [Required(ErrorMessage = "Please enter the Direction Text")]
        public string DirectionText { get; set; }

        [Display(Name = "Display Type:")]
        [Required(ErrorMessage = "Please enter a Display Type")]
        public Recipe.DisplayTypeCodes DisplayType { get; set; }

        #endregion

        #region Private Properties
        #endregion
    }
}