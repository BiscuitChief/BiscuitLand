using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Web.Configuration;

namespace BiscuitChief.Models
{
    public partial class Login
    {
        #region Constructors

        public Login() { }

        public Login(string username)
        {
            using (MySqlConnection conn = new MySqlConnection(WebConfigurationManager.ConnectionStrings["default"].ToString()))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Security_Select_User", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pUsername", username);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        this.UserName = dr["Username"].ToString();
                        this.Password = dr["Password"].ToString();
                        this.EncryptionSeed = dr["EncryptionSeed"].ToString();
                    }
                }
                conn.Close();
            }
        }

        #endregion

        #region Public Methods

        public static bool ValidateLogin(string username, string password)
        {
            bool isvalid = false;

            Login userlookup = new Login(username);
            if (!string.IsNullOrEmpty(userlookup.UserName))
            {
                string encryptedpass = PortalUtility.HashString(userlookup.EncryptionSeed, password);
                if (userlookup.UserName.ToLower() == username.ToLower() && userlookup.Password == encryptedpass)
                { isvalid = true; }
            }

            return isvalid;
        }

        public string AddNewUser()
        {
            string resultmsg = string.Empty;

            if (!string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.Password))
            {
                this.EncryptionSeed = Guid.NewGuid().ToString();
                this.Password = PortalUtility.HashString(this.EncryptionSeed, this.Password);

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
                    {
                        conn.Open();

                        MySqlCommand cmd = new MySqlCommand("Security_Insert_User", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pUsername", this.UserName);
                        cmd.Parameters.AddWithValue("@pPassword", this.Password);
                        cmd.Parameters.AddWithValue("@pEncryptionSeed", this.EncryptionSeed);
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        resultmsg = "Success";
                    }
                }
                catch(Exception ex)
                {
                    resultmsg = ex.Message;
                }
            }

            return resultmsg;
        }

        #endregion
    }
}