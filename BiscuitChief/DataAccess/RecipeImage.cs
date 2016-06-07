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
using System.IO;


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
            MySqlCommand cmd = new MySqlCommand("Recipe_SaveImage", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pRecipeID", this.RecipeID);
            cmd.Parameters.AddWithValue("@pImageName", this.ImageName);
            cmd.Parameters.AddWithValue("@pIsPrimary", this.IsPrimary);
            cmd.Parameters.AddWithValue("@pSortOrder", this.SortOrder);
            cmd.ExecuteNonQuery();

            if (this.IsTemp)
            {
                string thumbtemp = HttpContext.Current.Server.MapPath(Path.Combine(Path_TempThumbnail, this.ImageName));
                string thumbperm = HttpContext.Current.Server.MapPath(Path.Combine(Path_Thumbnail, this.ImageName));
                string standardtemp = HttpContext.Current.Server.MapPath(Path.Combine(Path_TempStandard, this.ImageName));
                string standardperm = HttpContext.Current.Server.MapPath(Path.Combine(Path_Standard, this.ImageName));

                File.Move(thumbtemp, thumbperm);
                File.Move(standardtemp, standardperm);
            }
        }

        public void DeleteImage()
        {
            using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
            {
                conn.Open();
                DeleteImage(conn);
                conn.Close();
            }
        }

        public void DeleteImage(MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand("Recipe_DeleteImage", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pRecipeID", this.RecipeID);
            cmd.Parameters.AddWithValue("@pImageName", this.ImageName);
            cmd.ExecuteNonQuery();

            string thumbtemp = HttpContext.Current.Server.MapPath(Path.Combine(Path_TempThumbnail, this.ImageName));
            string thumbperm = HttpContext.Current.Server.MapPath(Path.Combine(Path_Thumbnail, this.ImageName));
            string standardtemp = HttpContext.Current.Server.MapPath(Path.Combine(Path_TempStandard, this.ImageName));
            string standardperm = HttpContext.Current.Server.MapPath(Path.Combine(Path_Standard, this.ImageName));

            if (this.IsTemp)
            {
                File.Delete(thumbtemp);
                File.Delete(standardtemp);
            }
            else
            {
                File.Delete(thumbperm);
                File.Delete(standardperm);
            }
        }

        #endregion

        #region Private Methods

        private void LoadDataRow(DataRow dr)
        {
            this.RecipeID = dr["RecipeID"].ToString();
            this.ImageName = dr["ImageName"].ToString().Trim();
            this.SortOrder = Convert.ToInt32(dr["SortOrder"]);
            this.IsPrimary = Convert.ToBoolean(dr["IsPrimary"]);
            this.IsTemp = false;
        }

        #endregion
    }
}