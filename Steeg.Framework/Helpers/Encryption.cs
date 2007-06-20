using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Steeg.Framework.Helpers
{
    public class Encryption
    {
        private Encryption()
        {
        }

        /// <summary>
        /// Calculates the MD5 of a given string.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>The (hexadecimal) string representatation of the MD5 hash.</returns>
        public static string MD5(string inputString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }
    }
}
