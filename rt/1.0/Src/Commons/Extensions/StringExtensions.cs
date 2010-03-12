using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using BoC.Helpers;
using BoC.Validation;

namespace BoC.Extensions
{
    public static class StringExtensions
    {

        public static string JsSerialize(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return value;
            
            return new JavaScriptSerializer().Serialize(value);
        }

        static readonly Regex guidRegEx = new Regex(Expressions.Guid, RegexOptions.Compiled);
        public static bool IsGuid(this string expression)
        {
            return !String.IsNullOrEmpty(expression) 
                    && expression.Contains("-")
                    && guidRegEx.IsMatch(expression);
        }

        private static readonly Regex emailExpression = new Regex(Expressions.Email, RegexOptions.Singleline | RegexOptions.Compiled);
        public static bool IsEmail(this string target)
        {
            return !string.IsNullOrEmpty(target) && emailExpression.IsMatch(target);
        }

        private static readonly Regex webUrlExpression = new Regex(Expressions.WebUrl, RegexOptions.Singleline | RegexOptions.Compiled);
        public static bool IsWebUrl(this string target)
        {
            return !string.IsNullOrEmpty(target) && webUrlExpression.IsMatch(target);
        }

        public static string MD5(this string s)
        {
            var provider = new MD5CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(s);
            var builder = new StringBuilder();

            bytes = provider.ComputeHash(bytes);
            
            foreach (var b in bytes)
                builder.Append(b.ToString("x2").ToLower());

            return builder.ToString();
        }

        public static string FormatWith(this string target, params object[] args)
        {
            Check.Argument.IsNotEmpty(target, "target");

            return string.Format(CultureInfo.CurrentCulture, target, args);
        }

        public static int LevenshteinDistance(this String value, String compareString)
        {
            return LevenshteinDistance(value, compareString, false);
        }
        ///*****************************
        /// Compute Levenshtein distance 
        /// Memory efficient version
        ///*****************************
        public static int LevenshteinDistance(this String value, String compareString, bool removeSpecialChars)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (compareString == null)
                throw new ArgumentNullException("compareString");

            if (removeSpecialChars)
            {
                value = Regex.Replace(value.ToLower(), "\\W", "");
                compareString = Regex.Replace(compareString.ToLower(), "\\W", "");
            }

            int RowLen = value.Length;  // length of sRow
            int ColLen = compareString.Length;  // length of sCol
            int RowIdx;                // iterates through sRow
            int ColIdx;                // iterates through sCol
            char Row_i;                // ith character of sRow
            char Col_j;                // jth character of sCol
            int cost;                   // cost

            /// Test string length
            if (Math.Max(value.Length, compareString.Length) > Math.Pow(2, 31))
                throw (new Exception("\nMaximum string length in Levenshtein.iLD is " + Math.Pow(2, 31) + ".\nYours is " + Math.Max(value.Length, compareString.Length) + "."));

            // Step 1

            if (RowLen == 0 && ColLen == 0)
            {
                return 0;
            }

            if (ColLen == 0 || RowLen == 0)
            {
                return 100;
            }

            /// Create the two vectors
            int[] v0 = new int[RowLen + 1];
            int[] v1 = new int[RowLen + 1];
            int[] vTmp;



            /// Step 2
            /// Initialize the first vector
            for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
            {
                v0[RowIdx] = RowIdx;
            }

            // Step 3

            /// Fore each column
            for (ColIdx = 1; ColIdx <= ColLen; ColIdx++)
            {
                /// Set the 0'th element to the column number
                v1[0] = ColIdx;

                Col_j = compareString[ColIdx - 1];


                // Step 4

                /// Fore each row
                for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
                {
                    Row_i = value[RowIdx - 1];


                    // Step 5

                    if (Row_i == Col_j)
                    {
                        cost = 0;
                    }
                    else
                    {
                        cost = 1;
                    }

                    // Step 6

                    /// Find minimum
                    int m_min = v0[RowIdx] + 1;
                    int b = v1[RowIdx - 1] + 1;
                    int c = v0[RowIdx - 1] + cost;

                    if (b < m_min)
                    {
                        m_min = b;
                    }
                    if (c < m_min)
                    {
                        m_min = c;
                    }

                    v1[RowIdx] = m_min;
                }

                /// Swap the vectors
                vTmp = v0;
                v0 = v1;
                v1 = vTmp;

            }


            // Step 7

            /// Value between 0 - 100
            /// 0==perfect match 100==totaly different
            /// 
            /// The vectors where swaped one last time at the end of the last loop,
            /// that is why the result is now in v0 rather than in v1
            System.Console.WriteLine("iDist=" + v0[RowLen]);
            int max = System.Math.Max(RowLen, ColLen);
            return ((100 * v0[RowLen]) / max);
        }

    }
}
