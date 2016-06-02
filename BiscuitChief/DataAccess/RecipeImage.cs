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
using System.Text.RegularExpressions;


namespace BiscuitChief.Models
{
    public partial class RecipeImage
    {
        #region Constructors

        public RecipeImage()
        {
            this.IsPrimary = false;
            this.SortOrder = 0;
        }

        public RecipeImage(DataRow dr)
        {
            LoadDataRow(dr);
        }

        #endregion

        #region Public Methods

        public void SaveImage()
        {
            using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
            {
                conn.Open();
                SaveImage(conn);
                conn.Close();
            }
        }

        /// <summary>
        /// Save an image to the database
        /// </summary>
        /// <param name="conn">Open database connection</param>
        public void SaveImage(MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("Recipe_SaveIngredient", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pRecipeID", this.RecipeID);
            cmd.Parameters.AddWithValue("@pIngredientName", this.ImageName);
            cmd.Parameters.AddWithValue("@pIsPrimary", this.IsPrimary);
            cmd.Parameters.AddWithValue("@pSortOrder", this.SortOrder);
            cmd.ExecuteNonQuery();

            //add code here to move images from the temp folder to the permanent folder
        }

        #endregion

        #region Private Methods

        private void LoadDataRow(DataRow dr)
        {
            this.RecipeID = dr["RecipeID"].ToString();
            this.ImageName = dr["ImageName"].ToString().Trim();
            this.SortOrder = Convert.ToInt32(dr["SortOrder"]);
            this.IsPrimary = Convert.ToBoolean(dr["IsPrimary"]);
        }

        #endregion
    }
}