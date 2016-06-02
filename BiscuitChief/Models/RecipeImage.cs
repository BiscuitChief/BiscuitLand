using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BiscuitChief.Models
{
    public partial class RecipeImage
    {
        #region Public Properties

        public string RecipeID { get; set; }

        [Display(Name = "Ingredient Name:")]
        [Required(ErrorMessage = "Please enter an Ingredient Name")]
        public string ImageName { get; set; }

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }

        #endregion

        #region Private Properties
        #endregion

    }
}