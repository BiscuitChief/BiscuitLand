using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Net.Mail;
using System.Web.UI;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Drawing;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Linq;

namespace BiscuitChief
{
    public class PortalUtility
    {
        #region Encryption

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="key">The key that will be used to encrypt the data</param>
        /// <param name="data">The string you wish to encrypt</param>
        /// <returns>Encrypted data</returns>
        public static string Encrypt(string key, string data)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(data))
            {
                byte[] m_Key = new byte[8];
                byte[] m_IV = new byte[8];

                InitKey(key, ref m_Key, ref m_IV);

                DESCryptoServiceProvider csprov = new DESCryptoServiceProvider();
                MemoryStream memstream = new MemoryStream();
                CryptoStream crstream = new CryptoStream(memstream, csprov.CreateEncryptor(m_Key, m_IV), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(crstream);

                sw.Write(data);
                sw.Flush();
                crstream.FlushFinalBlock();
                memstream.Flush();

                result = Convert.ToBase64String(memstream.GetBuffer(), 0, Convert.ToInt32(memstream.Length));

                sw.Close();
                crstream.Close();
                memstream.Close();
            }

            return result;
        }

        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="key">The key that was used to encrypt the string</param>
        /// <param name="data">The encrypted data you wish to decrypt</param>
        /// <returns>Unencrypted data</returns>
        public static string Decrypt(string key, string data)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(data))
            {
                byte[] m_Key = new byte[8];
                byte[] m_IV = new byte[8];

                InitKey(key, ref m_Key, ref m_IV);

                DESCryptoServiceProvider csprov = new DESCryptoServiceProvider();
                MemoryStream memstream = new MemoryStream(Convert.FromBase64String(data));
                CryptoStream crstream = new CryptoStream(memstream, csprov.CreateDecryptor(m_Key, m_IV), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(crstream);

                result = sr.ReadToEnd();

                sr.Close();
                memstream.Close();
                crstream.Close();
            }

            return result;
        }

        public static bool InitKey(string strKey, ref byte[] m_Key, ref byte[] m_IV)
        {
            try
            {
                //Convert Key to byte array
                byte[] bp = new byte[strKey.Length];
                ASCIIEncoding aEnc = new ASCIIEncoding();
                aEnc.GetBytes(strKey, 0, strKey.Length, bp, 0);

                //Hash the key using SHA1
                SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
                byte[] bpHash = sha.ComputeHash(bp);

                int i;

                //use the low 64-bits for the key value
                for(i = 0; i <= 7; i++)
                {
                    m_Key[i] = bpHash[i];
                }
                for (i = 8; i <= 15; i++)
                {
                    m_IV[i - 8] = bpHash[i];
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                //return false;
            }
        }

        public static string HashString(string seed, string data)
        {
            byte[] dataseed = Encoding.ASCII.GetBytes(data + seed);
            SHA512 mySHA512 = SHA512Managed.Create();
            byte[] hashbytes = mySHA512.ComputeHash(dataseed);

            string returnval = Convert.ToBase64String(hashbytes);

            return returnval;
        }


        #endregion

        #region Database

        /// <summary>
        /// Added this just to shorten up code where I user SQL connections
        /// </summary>
        /// <param name="connname">connection name from web.config</param>
        /// <returns></returns>
        public static string GetConnectionString(string connname)
        {
            return ConfigurationManager.ConnectionStrings[connname].ConnectionString;
        }

        public static object CheckDbNull(object value)
        {
            if (value == DBNull.Value)
            { return null; }
            else
            { return value; }
        }

        #endregion

        #region Email

        public static string SendEmail(string _subject, string _body)
        {
            return SendEmail(_subject, _body, String.Empty, String.Empty, String.Empty);
        }

        public static string SendEmail(string _subject, string _body, string _mailto, string _mailfrom, string _fromdisplayname)
        {
            string error = String.Empty;
            string smtpuser = String.Empty;
            string smtppass = String.Empty;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
                {
                    conn.Open();

                    smtpuser = GetSiteSetting("ContactUsername", conn);
                    smtppass = GetSiteSetting("ContactPassword", conn);

                    if (String.IsNullOrEmpty(_mailfrom))
                    { _mailfrom = GetSiteSetting("ContactFrom", conn); }

                    if (String.IsNullOrEmpty(_mailto))
                    { _mailto = GetSiteSetting("ContactTo", conn); }

                    conn.Close();
                }
                MailMessage mailmsg = new MailMessage();
                mailmsg.To.Add(_mailto);

                mailmsg.Subject = _subject;
                mailmsg.Body = _body;
                mailmsg.From = new MailAddress(_mailfrom, _fromdisplayname);

                if (HttpContext.Current.Request.ServerVariables["SERVER_NAME"] == "localhost")
                {
                    return String.Empty;
                    //mailmsg.To.Clear();
                    //mailmsg.To.Add("alternate email to");
                }

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential(smtpuser, smtppass);
                client.Send(mailmsg);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }

        public static string SendErrorEmail(Exception ex)
        {
            string error = String.Empty;

            StringBuilder body = new StringBuilder();
            try { body.AppendLine(HttpContext.Current.Request.Url.AbsoluteUri); }
            catch { }
            try { body.AppendLine(HttpContext.Current.User.Identity.Name); }
            catch { }

            body.AppendLine(ex.Message);
            body.AppendLine(ex.TargetSite.Name);
            body.AppendLine(ex.StackTrace);

            error = SendEmail("BiscuitChief.net error email", body.ToString());

            return error;
        }

        /// <summary>
        /// Retrieves an email template as a text string
        /// </summary>
        /// <param name="_filename">The filename of the email template</param>
        /// <returns>The email template string</returns>
        public static String GetEmailTemplate(string _filename)
        {
            String strReturn = String.Empty;

            StreamReader _textStreamReader = new StreamReader(HttpContext.Current.Server.MapPath(Path.Combine("/App_Data/EmailTemplates", _filename)));

            strReturn = _textStreamReader.ReadToEnd();
            _textStreamReader.Close();

            return strReturn;
        }

        #endregion

        public static void ValidateAntiForgeryToken()
        {
            string cookieToken = String.Empty;
            string formToken = String.Empty;

            HttpRequest request = HttpContext.Current.Request;
            formToken = request.Headers["__RequestVerificationToken"];
            cookieToken = HttpContext.Current.Request.Cookies["__RequestVerificationToken"].Value;
            AntiForgery.Validate(cookieToken, formToken);
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(newImage))
            { graphics.DrawImage(image, 0, 0, newWidth, newHeight); }

            return newImage;
        }

        public static string GetSiteSetting(string _settingcode)
        {
            string returnval = String.Empty;
            using (MySqlConnection conn = new MySqlConnection(PortalUtility.GetConnectionString("default")))
            {
                conn.Open();
                returnval = GetSiteSetting(_settingcode, conn);
                conn.Close();
            }
            return returnval;
        }

        public static string GetSiteSetting(string _settingcode, MySqlConnection conn)
        {
            string returnval = String.Empty;

            MySqlCommand cmd = new MySqlCommand("Lookup_Select_SiteSetting", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pSettingCode", _settingcode);
            returnval = Convert.ToString(cmd.ExecuteScalar());

            return returnval;
        }

        public static void CleanupTempFiles()
        {
            string tempfolder = HttpContext.Current.Server.MapPath("/Content/Images/Temp");
            DateTime cutoffdate = DateTime.Now.AddHours(-6);

            foreach (string filename in Directory.GetFiles(tempfolder, "*", SearchOption.AllDirectories))
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.CreationTime < cutoffdate)
                { File.Delete(filename); }
            }
        }

        public class PlainTextResult : IHttpActionResult
        {
            public PlainTextResult(string msg, HttpStatusCode statuscd)
            {
                this.Content = msg;
                this.StatusCode = statuscd;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage response = new HttpResponseMessage(StatusCode);
                response.Content = new StringContent(Content);
                return Task.FromResult(response);
            }

            public string Content { get; set; }
            public HttpStatusCode StatusCode { get; set; }
        }

        public class PagerHelper
        {
            public static int GetPageCount(int pagesize, int itemcount)
            {
                int returnval = 0;

                returnval = itemcount / pagesize;
                if ((itemcount % pagesize) > 0 || returnval == 0)
                { returnval++; }

                return returnval;
            }

            public static int GetPagerStart(int currentpage, int pagespread, int pagecount)
            {
                int returnval = 0;

                int pagesright = pagecount - currentpage;
                int pagesleft = pagespread;
                if (pagesright < pagespread)
                { pagesleft = pagesleft + pagespread - pagesright; }

                returnval = currentpage - pagesleft;
                if (returnval <= 0)
                { returnval = 1; }

                return returnval;
            }

            public static int GetPagerEnd(int currentpage, int pagespread, int pagecount)
            {
                int returnval = 0;

                int pagesright = pagespread;
                int pagesleft = currentpage - 1;
                if (pagesleft < pagespread)
                { pagesright = pagesright + pagespread - pagesleft; }

                returnval = currentpage + pagesright;
                if (returnval > pagecount)
                { returnval = pagecount; }

                return returnval;
            }

            public static int GetPreviousPage(int currentpage)
            {
                int returnval = 0;

                returnval = currentpage - 1;
                if (currentpage < 1)
                { currentpage = 1; }

                return returnval;
            }

            public static int GetNextPage(int currentpage, int pagecount)
            {
                int returnval = 0;

                returnval = currentpage + 1;
                if (currentpage > pagecount)
                { currentpage = pagecount; }

                return returnval;
            }

            public static int CheckPageValid(int currentpage, int pagecount)
            {
                int returnval = currentpage;

                if (returnval < 1)
                { returnval = 1; }
                else if (returnval > pagecount)
                { returnval = pagecount; }

                return returnval;
            }
        }
    }
}