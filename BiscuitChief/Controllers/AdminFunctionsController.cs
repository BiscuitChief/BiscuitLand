﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace BiscuitChief.Controllers
{
    public class AdminFunctionsController : Controller
    {
        [Authorize(Roles = "ADMIN")]
        public ActionResult CreateDataBackupScripts()
        {
            string recipesaveproc = "call Recipe_SaveRecipe('{0}','{1}',0,pRecipeID);";
            string ingredientsaveproc = "call Recipe_SaveIngredient(0,pRecipeID,'{0}',{1},'{2}','{3}','{4}',{5},pIngredientID);";
            string directionsaveproc = "call Recipe_SaveDirections(0,pRecipeID,{0},'{1}','{2}',pDirectionID);";
            string categorysaveproc = "call Recipe_SaveRecipeCategory(pRecipeID,'{0}');";

            StringBuilder backupscript = new StringBuilder();
            backupscript.AppendLine("declare pRecipeID varchar(36);");
            backupscript.AppendLine("declare pIngredientID int;");
            backupscript.AppendLine("declare pDirectionID int;");
            backupscript.AppendLine();
            backupscript.AppendLine("TRUNCATE TABLE Recipe_Recipes;");
            backupscript.AppendLine("TRUNCATE TABLE Recipe_Ingredients;");
            backupscript.AppendLine("TRUNCATE TABLE Recipe_Directions;");
            backupscript.AppendLine();

            List<Models.Recipe> recipes = Models.Recipe.SearchRecipes(String.Empty, new string[] { }, new string[] { });
            foreach (Models.Recipe item in recipes)
            {
                Models.Recipe rcp = new Models.Recipe(item.RecipeID);

                backupscript.AppendLine(String.Format(recipesaveproc, rcp.Title.Replace("'", "''"), rcp.Description.Replace("'", "''")));

                foreach (Models.RecipeIngredient ing in rcp.IngredientList)
                {
                    backupscript.AppendLine(String.Format(ingredientsaveproc, ing.IngredientName.Trim().Replace("'", "''"), ing.Quantity, ing.UnitOfMeasure.Replace("'", "''"), ing.Notes.Trim().Replace("'", "''"), Enum.GetName(ing.DisplayType.GetType(), ing.DisplayType), ing.SortOrder));
                }

                foreach (Models.RecipeDirection dir in rcp.DirectionList)
                {
                    backupscript.AppendLine(String.Format(directionsaveproc, dir.SortOrder, dir.DirectionText.Trim().Replace("'", "''"), Enum.GetName(dir.DisplayType.GetType(), dir.DisplayType)));
                }

                foreach (Models.Recipe.Category ctg in rcp.CategoryList)
                {
                    backupscript.AppendLine(String.Format(categorysaveproc, ctg.CategoryCode));
                }
                backupscript.AppendLine();
                backupscript.AppendLine();
            }

            System.IO.File.WriteAllText(Server.MapPath("/DownloadFiles/DataRestoreScript.txt"), backupscript.ToString());

            ViewBag.ResultMessage = "Success";
            return View();
        }
    }
}