using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

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

    }
}