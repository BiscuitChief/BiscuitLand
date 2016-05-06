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

        public Recipe() { }

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

                }
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