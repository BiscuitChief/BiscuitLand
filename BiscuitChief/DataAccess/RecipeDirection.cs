﻿using System;
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