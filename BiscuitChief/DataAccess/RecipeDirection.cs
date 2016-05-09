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
    public partial class RecipeDirection
    {
        #region Constructors

        public RecipeDirection() { }

        public RecipeDirection(DataRow dr)
        {
            LoadDataRow(dr);
        }

        #endregion

        #region Public Methods

        public void SaveDirection()
        {
            using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
            {
                conn.Open();
                SaveDirection(conn);
                conn.Close();
            }
        }

        /// <summary>
        /// Save recipe direction to the database
        /// </summary>
        /// <param name="conn">Open database connection</param>
        public void SaveDirection(MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("Recipe_SaveDirections", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pDirectionID", this.DirectionID);
            cmd.Parameters.AddWithValue("@pRecipeID", this.RecipeID);
            cmd.Parameters.AddWithValue("@pSortOrder", this.SortOrder);
            cmd.Parameters.AddWithValue("@pDirectionText", this.DirectionText);
            cmd.Parameters.AddWithValue("@pDisplayType", Enum.GetName(this.DisplayType.GetType(), this.DisplayType));
            cmd.Parameters.Add("@pDirectionIDOut", MySqlDbType.Int32);
            cmd.Parameters["@pDirectionIDOut"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            this.DirectionID = Convert.ToInt32(cmd.Parameters["@pDirectionIDOut"].Value);
        }

        #endregion

        #region Private Methods

        private void LoadDataRow(DataRow dr)
        {
            this.DirectionID = Convert.ToInt32(dr["DirectionID"]);
            this.RecipeID = dr["RecipeID"].ToString();
            this.DirectionText = dr["DirectionText"].ToString().Trim();
            this.SortOrder = Convert.ToInt32(dr["SortOrder"]);
            this.DisplayType = (Recipe.DisplayTypeCodes)(Enum.Parse(typeof(Recipe.DisplayTypeCodes), dr["DisplayType"].ToString()));
        }

        #endregion
    }
}