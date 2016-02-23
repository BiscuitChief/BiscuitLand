using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Web.Configuration;


namespace BiscuitChief.Models
{
    public partial class RecipeIngredient
    {
        #region Constructors

        public RecipeIngredient() { }

        public RecipeIngredient(DataRow dr)
        {
            LoadDataRow(dr);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        private void LoadDataRow(DataRow dr)
        {
            this.IngredientID = Convert.ToInt32(dr["IngredientID"]);
            this.RecipeID = dr["RecipeID"].ToString();
            this.IngredientName = dr["IngredientName"].ToString().Trim();
            this.Quantity = Convert.ToDecimal(PortalUtility.CheckDbNull(dr["Quantity"]));
            this.UnitOfMeasure = dr["UnitOfMeasure"].ToString().Trim();
            this.Notes = dr["Notes"].ToString().Trim();
            this.SortOrder = Convert.ToInt32(dr["SortOrder"]);
            this.DisplayType = (Recipe.DisplayTypeCodes)(Enum.Parse(typeof(Recipe.DisplayTypeCodes), dr["DisplayType"].ToString()));
        }

        #endregion
    }
}