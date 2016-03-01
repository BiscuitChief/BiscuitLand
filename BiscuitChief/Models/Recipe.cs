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

        public List<Category> CategoryList { get; set; }

        #endregion

        #region Private Properties
        #endregion

        public enum DisplayTypeCodes { ING, DIR, HDR }

        public class Category
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