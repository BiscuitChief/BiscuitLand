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
    public partial class Login
    {
        #region Constructors

        public Login() { }

        public Login(string username)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["default"].ToString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Security.Select_User", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                using (SqlDataReader dr = cmd.ExecuteReader())
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
                string encryptedpass = PortalUtility.Encrypt(userlookup.EncryptionSeed, password);
                if (userlookup.UserName == username && userlookup.Password == encryptedpass)
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
                this.Password = PortalUtility.Encrypt(this.EncryptionSeed, this.Password);

                try
                {
                    using (SqlConnection conn = new SqlConnection(PortalUtility.GetConnectionString("default")))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("Security.Insert_User", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", this.UserName);
                        cmd.Parameters.AddWithValue("@Password", this.Password);
                        cmd.Parameters.AddWithValue("@EncryptionSeed", this.EncryptionSeed);
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