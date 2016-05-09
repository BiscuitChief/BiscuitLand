using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Web.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;


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

        public void SaveIngredient()
        {
            using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
            {
                conn.Open();
                SaveIngredient(conn);
                conn.Close();
            }
        }

        /// <summary>
        /// Save an ingredient to the database
        /// </summary>
        /// <param name="conn">Open database connection</param>
        public void SaveIngredient(MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("Recipe_SaveIngredient", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pIngredientID", this.IngredientID);
            cmd.Parameters.AddWithValue("@pRecipeID", this.RecipeID);
            cmd.Parameters.AddWithValue("@pIngredientName", this.IngredientName);
            cmd.Parameters.AddWithValue("@pQuantity", this.Quantity);
            cmd.Parameters.AddWithValue("@pUnitOfMeasure", this.UnitOfMeasure);
            cmd.Parameters.AddWithValue("@pNotes", this.Notes);
            cmd.Parameters.AddWithValue("@pDisplayType", Enum.GetName(this.DisplayType.GetType(), this.DisplayType));
            cmd.Parameters.AddWithValue("@pSortOrder", this.SortOrder);
            cmd.Parameters.Add("@pIngredientIDOut", MySqlDbType.Int32);
            cmd.Parameters["@pIngredientIDOut"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            this.IngredientID = Convert.ToInt32(cmd.Parameters["@pIngredientIDOut"].Value);
        }

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