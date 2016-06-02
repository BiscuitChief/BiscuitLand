using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BiscuitChief.Models
{
    public partial class Recipe
    {
        #region Public Properties

        public string RecipeID { get; set; }

        [Display(Name = "Title:")]
        [Required(ErrorMessage = "Please enter a Title")]
        public string Title { get; set; }

        [Display(Name = "Description:")]
        [Required(ErrorMessage = "Please enter a Description")]
        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public List<RecipeIngredient> IngredientList { get; set; }

        public List<RecipeDirection> DirectionList { get; set; }

        public List<Category> CategoryList { get; set; }

        public List<RecipeImage> ImageList { get; set; }

        #endregion

        #region Private Properties
        #endregion

        public enum DisplayTypeCodes
        {
            [Display(Name = "Ingredient")]
            ING,
            [Display(Name = "Direction")]
            DIR,
            [Display(Name = "Header")]
            HDR 
        }

        public partial class Category
        {
            public Category() { }
            
            /// <summary>
            /// Standard constructor, if we are only loading the categories assigned to the recipe they are selected by default
            /// </summary>
            /// <param name="_categorycode"></param>
            /// <param name="_categoryname"></param>
            public Category(string _categorycode, string _categoryname)
            {
                LoadData(_categorycode, _categoryname, true);
            }

            /// <summary>
            /// Overloaded constructor if we want a list of all categories
            /// </summary>
            /// <param name="_categorycode"></param>
            /// <param name="_categoryname"></param>
            /// <param name="_isselected"></param>
            public Category(string _categorycode, string _categoryname, bool _isselected)
            {
                LoadData(_categorycode, _categoryname, _isselected);
            }

            private void LoadData(string _categorycode, string _categoryname, bool _isselected)
            {
                this.CategoryCode = _categorycode;
                this.CategoryName = _categoryname;
                this.IsSelected = _isselected;
            }

            public string CategoryCode { get; set; }

            public string CategoryName { get; set; }

            public bool IsSelected { get; set; }
        }
    }
}