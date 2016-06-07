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

        public bool IsTemp { get; set; }

        public static string Path_TempThumbnail
        { get { return "/Content/Images/Temp/Thumbnails"; } }

        public static string Path_TempStandard
        { get { return "/Content/Images/Temp/Standard"; } }

        public static string Path_Thumbnail
        { get { return "/Content/Images/Recipes/Thumbnails"; } }

        public static string Path_Standard
        { get { return "/Content/Images/Recipes/Standard"; } }

        #endregion

        #region Private Properties
        #endregion

    }
}