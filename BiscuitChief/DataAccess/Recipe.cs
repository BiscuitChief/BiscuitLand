using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Web.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace BiscuitChief.Models
{
    public partial class Recipe
    {
        #region Constructors

        public Recipe()
        {
            this.IngredientList = new List<RecipeIngredient>();
            this.DirectionList = new List<RecipeDirection>();
            this.CategoryList = new List<Category>();
            this.ImageList = new List<RecipeImage>();
        }

        public Recipe(string _recipeid, decimal _quantity = 1)
        {
            DataSet ds = new DataSet();
            MySqlDataAdapter da;
            using (MySqlConnection conn = new MySqlConnection(WebConfigurationManager.ConnectionStrings["default"].ToString()))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Recipe_Select_Recipe", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pRecipeID", _recipeid);
                da = new MySqlDataAdapter(cmd);
                da.Fill(ds, "Recipe");

                    if (ds.Tables["Recipe"].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables["Recipe"].Rows[0];
                        LoadDataRow(dr);
                    }

                if (!String.IsNullOrEmpty(this.RecipeID))
                {
                    cmd = new MySqlCommand("Recipe_Select_RecipeIngredients", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@pRecipeID", _recipeid);
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(ds, "Ingredients");

                    this.IngredientList = new List<RecipeIngredient>();
                    foreach (DataRow dr in ds.Tables["Ingredients"].Rows)
                    { this.IngredientList.Add(new RecipeIngredient(dr)); }

                    cmd = new MySqlCommand("Recipe_Select_RecipeDirections", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@pRecipeID", _recipeid);
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(ds, "Directions");

                    this.DirectionList = new List<RecipeDirection>();
                    foreach (DataRow dr in ds.Tables["Directions"].Rows)
                    { this.DirectionList.Add(new RecipeDirection(dr)); }

                    cmd = new MySqlCommand("Recipe_Select_Categories", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@pRecipeID", _recipeid);
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(ds, "Categories");

                    this.CategoryList = new List<Recipe.Category>();
                    foreach (DataRow dr in ds.Tables["Categories"].Rows)
                    { this.CategoryList.Add(new Recipe.Category(dr["CategoryCode"].ToString(), dr["CategoryName"].ToString())); }

                    cmd = new MySqlCommand("Recipe_Select_RecipeImages", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@pRecipeID", _recipeid);
                    da = new MySqlDataAdapter(cmd);
                    da.Fill(ds, "Images");

                    this.ImageList = new List<RecipeImage>();
                    foreach (DataRow dr in ds.Tables["Images"].Rows)
                    { this.ImageList.Add(new RecipeImage(dr)); }
                }

                conn.Close();
            }

            this.Quantity = _quantity;
            CalculateRecipeQuantity(this);
        }

        public Recipe(DataRow dr)
        {
            LoadDataRow(dr);
        }

        #endregion

        #region Public Methods

        public static List<Recipe> SearchRecipes(string _searchtext, string[] _ingredients, string[] _categories)
        {
            List<Recipe> results = new List<Recipe>();

            string ingsearchlist = String.Empty;
            foreach (string ing in _ingredients)
            { ingsearchlist += ing + "|"; }

            string ctgsearchlist = String.Empty;
            foreach (string ctg in _categories)
            { ctgsearchlist += ctg + "|"; }

            DataSet ds = new DataSet();
            using (MySqlConnection conn = new MySqlConnection(WebConfigurationManager.ConnectionStrings["default"].ToString()))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Recipe_Select_RecipeSearch", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pSearchText", _searchtext);
                cmd.Parameters.AddWithValue("@pIngredients", ingsearchlist);
                cmd.Parameters.AddWithValue("@pCategories", ctgsearchlist);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(ds, "Recipes");

                Recipe newrcp;
                string[] catlist;
                string[] catitem;

                foreach (DataRow dr in ds.Tables["Recipes"].Rows)
                {
                    newrcp = new Recipe(dr);
                    //The category list is returned in the format of: catcode::catname||catcode::catname||
                    newrcp.CategoryList = new List<Recipe.Category>();
                    catlist = dr["CategoryList"].ToString().Split(new string[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach(string category in catlist)
                    {
                        catitem = category.Split(new string[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
                        newrcp.CategoryList.Add(new Recipe.Category(catitem[0], catitem[1]));
                    }

                    results.Add(newrcp);
                }

            }

            return results;
        }

        public static void CalculateRecipeQuantity(Recipe rcp)
        {
            Dictionary<decimal, string> conversionchart = new Dictionary<decimal, string>();

            using (MySqlConnection conn = new MySqlConnection(WebConfigurationManager.ConnectionStrings["default"].ToString()))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Lookup_Select_QuantityConversion", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    decimal keyvalue = (decimal)TruncateQuantity(Convert.ToDecimal(dr["QuantityDecimal"]));
                    string displayvalue = Convert.ToString(dr["QuantityDisplay"]);
                    conversionchart.Add(keyvalue, displayvalue);
                }
                dr.Close();
                conn.Close();
            }

            foreach(RecipeIngredient ing in rcp.IngredientList)
            {
                if (ing.Quantity == 0)
                { ing.DisplayQuantity = String.Empty; }
                else
                {
                    ing.Quantity = TruncateQuantity(ing.Quantity) * rcp.Quantity;

                    decimal qtynumber = Math.Truncate((decimal)(ing.Quantity ?? 0));
                    decimal qtydecimal = (decimal)(TruncateQuantity(ing.Quantity - qtynumber));
                    if (qtydecimal == TruncateQuantity(Convert.ToDecimal(0.9999999)))
                    {
                        qtynumber += 1;
                        qtydecimal = 0;
                    }

                    if (qtynumber > 0)
                    { ing.DisplayQuantity = qtynumber.ToString(); }
                    if (qtydecimal > 0)
                    { 
                        if (conversionchart.ContainsKey(qtydecimal))
                        { ing.DisplayQuantity += " " + conversionchart[qtydecimal]; }
                        else
                        { ing.DisplayQuantity += " " + qtydecimal.ToString(); }
                    }
                    ing.DisplayQuantity = ing.DisplayQuantity.Trim();

                }
            }
        }

        public void SaveRecipe()
        {
            using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
            {
                conn.Open();
                //Save base recipe information
                MySqlCommand cmd = new MySqlCommand("Recipe_SaveRecipe", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pTitle", this.Title);
                cmd.Parameters.AddWithValue("@pDescription", this.Description);
                cmd.Parameters.AddWithValue("@pRecipeID", this.RecipeID);
                cmd.Parameters["@pRecipeID"].Direction = ParameterDirection.Input;
                cmd.Parameters.Add("@pRecipeIDOut", MySqlDbType.VarString);
                cmd.Parameters["@pRecipeIDOut"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                //Make sure we have the recipe id if it's a new recipe
                this.RecipeID = cmd.Parameters["@pRecipeIDOut"].Value.ToString();

                //Clear out any existing directions, ingredients, and categories.
                //This is just easier/lazier than trying to only delete what has been removed
                cmd = new MySqlCommand("Recipe_ClearData", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pRecipeID", this.RecipeID);
                cmd.ExecuteNonQuery();
                //Save ingredients
                foreach (RecipeIngredient ing in this.IngredientList)
                {
                    ing.RecipeID = this.RecipeID;
                    ing.SaveIngredient(conn);
                }
                //Save directions
                foreach (RecipeDirection dir in this.DirectionList)
                {
                    dir.RecipeID = this.RecipeID;
                    dir.SaveDirection(conn);
                }
                //Save categories
                foreach (Recipe.Category cat in this.CategoryList)
                {
                    //only save selected categories
                    if (cat.IsSelected)
                    { cat.SaveCategory(this.RecipeID, conn); }
                }
                //Save images
                foreach (RecipeImage img in this.ImageList)
                {
                    img.RecipeID = this.RecipeID;
                    img.SaveImage(conn);
                }

                //Delete any images that have been removed
                //Have to do this manually because we need to delete the physical image files
                cmd = new MySqlCommand("Recipe_Select_RecipeImages", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pRecipeID", this.RecipeID);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Images");

                List<RecipeImage> currentimgs = new List<RecipeImage>();
                foreach (DataRow dr in ds.Tables["Images"].Rows)
                { currentimgs.Add(new RecipeImage(dr)); }

                foreach (RecipeImage img in currentimgs)
                {
                    if (!(this.ImageList.Exists(x => x.ImageName == img.ImageName)))
                    { img.DeleteImage(conn); }
                }
                
                conn.Close();
            }
        }

        public static void DeleteRecipe(string _recipeid)
        {
            using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
            {
                conn.Open();
                //Have to get all current images so we can delete the image files
                MySqlCommand imgcmd = new MySqlCommand("Recipe_Select_RecipeImages", conn);
                imgcmd.CommandType = CommandType.StoredProcedure;
                imgcmd.Parameters.AddWithValue("@pRecipeID", _recipeid);
                MySqlDataAdapter da = new MySqlDataAdapter(imgcmd);
                DataSet ds = new DataSet();
                da.Fill(ds, "Images");

                foreach (DataRow dr in ds.Tables["Images"].Rows)
                { (new RecipeImage(dr)).DeleteImage(conn); }

                MySqlCommand cmd = new MySqlCommand("Recipe_DeleteRecipe", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pRecipeID", _recipeid);
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }

        public partial class Category
        {
            public static List<Category> GetAllCategories()
            {
                List<Category> returnval = new List<Category>();

                using (MySqlConnection conn = new MySqlConnection(WebConfigurationManager.ConnectionStrings["default"].ToString()))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("Lookup_Select_Categories", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        returnval.Add(new Models.Recipe.Category(dr["CategoryCode"].ToString(), dr["CategoryName"].ToString(), false));
                    }
                    dr.Close();
                }

                return returnval;
            }

            public void SaveCategory(string _recipeid)
            {
                using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
                {
                    conn.Open();
                    SaveCategory(_recipeid, conn);
                    conn.Close();
                }
            }

            public void SaveCategory(string _recipeid, MySqlConnection conn)
            {
                MySqlCommand cmd = new MySqlCommand("Recipe_SaveRecipeCategory", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pRecipeID", _recipeid);
                cmd.Parameters.AddWithValue("@pCategoryCode", this.CategoryCode);
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Private Methods

        private void LoadDataRow(DataRow dr)
        {
            this.RecipeID = dr["RecipeID"].ToString();
            this.Title = dr["Title"].ToString().Trim();
            this.Description = dr["Description"].ToString().Trim();
        }

        /// <summary>
        /// Truncate the quantity to 4 decimal places.  We only compare the converstion chart to 4 decimal places incase of rounding errors
        /// </summary>
        /// <param name="qty"></param>
        /// <returns></returns>
        private static Nullable<decimal> TruncateQuantity(Nullable<decimal> qty)
        {
            qty = Math.Truncate((decimal)(qty ?? 0) * 10000) / 10000;
            return qty;
        }

        #endregion
    }
}