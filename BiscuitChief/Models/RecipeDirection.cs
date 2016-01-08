using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiscuitChief.Models
{
    public partial class RecipeDirection
    {
        #region Public Properties

        public int DirectionID { get; set; }

        public int RecipeID { get; set; }

        public int SortOrder { get; set; }

        public string DirectionText { get; set; }

        public Recipe.DisplayTypeCodes DisplayType { get; set; }

        #endregion

        #region Private Properties
        #endregion
    }
}