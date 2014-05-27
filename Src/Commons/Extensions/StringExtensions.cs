using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            return !String.IsNullOrEmpty(target) && emailExpression.IsMatch(target);
        }

        private static readonly Regex webUrlExpression = new Regex(Expressions.WebUrl, RegexOptions.Singleline | RegexOptions.Compiled);
        public static bool IsWebUrl(this string target)
        {
            return !String.IsNullOrEmpty(target) && webUrlExpression.IsMatch(target);
        }

        public static string NullSafe(this string target)
        {
            return (target ?? String.Empty).Trim();
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

        public static string SplitUpperCaseToString(this string source)
        {
            if (source == null)
            {
                return null;
            }
            return String.Join(" ", source.SplitUpperCase());
        }

        public static string[] SplitUpperCase(this string source)
        {
            if (source == null)
                return new string[] { }; //Return empty array.

            if (source.Length == 0)
                return new string[] { "" };

            StringCollection words = new StringCollection();
            int wordStartIndex = 0;

            char[] letters = source.ToCharArray();
            char previousChar = Char.MinValue;
            // Skip the first letter. we don't care what case it is.
            for (int i = 1; i < letters.Length; i++)
            {
                if (Char.IsUpper(letters[i]) && !Char.IsWhiteSpace(previousChar))
                {
                    //Grab everything before the current index.
                    words.Add(new String(letters, wordStartIndex, i - wordStartIndex));
                    wordStartIndex = i;
                }
                previousChar = letters[i];
            }
            //We need to have the last word.
            words.Add(new String(letters, wordStartIndex, letters.Length - wordStartIndex));

            //Copy to a string array.
            string[] wordArray = new string[words.Count];
            words.CopyTo(wordArray, 0);
            return wordArray;
        }

        public static string FormatWith(this string target, params object[] args)
        {
            Check.Argument.IsNotEmpty(target, "target");

            return String.Format(CultureInfo.CurrentCulture, target, args);
        }

        public static T ToEnum<T>(this string target, T defaultValue) where T : struct, IComparable, IFormattable
        {
            var convertedValue = defaultValue;

            if (!String.IsNullOrEmpty(target))
            {
                try
                {
                    convertedValue = (T) Enum.Parse(typeof (T), target.Trim());
                }
                catch (ArgumentException){}
            }

            return convertedValue;
        }

        public static int LevenshteinDistance(this String value, String compareString)
        {
            return LevenshteinDistance(value, compareString, false);
        }

        public static int LevenshteinDistance(this String value, String compareString, bool removeSpecialChars)
        {
            return value.LevenshteinDistance(compareString, removeSpecialChars, false);
        }

        public static int LevenshteinDistancePercentage(this String value, String compareString, bool removeSpecialChars)
        {
            return value.LevenshteinDistance(compareString, removeSpecialChars, true);
        }

        public static int LevenshteinDistancePercentage(this String value, String compareString)
        {
            return value.LevenshteinDistance(compareString, false, true);
        }

        ///*****************************
        /// Compute Levenshtein distance 
        /// Memory efficient version
        ///*****************************
        static int LevenshteinDistance(this String value, String compareString, bool removeSpecialChars, bool returnPercentage)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (compareString == null)
                throw new ArgumentNullException("compareString");

            /// Test string length
            var maxlen = Math.Pow(2, 31);
            if (value.Length > maxlen)
                throw new ArgumentException("\nMaximum string length in Levenshtein.iLD is " +maxlen + ".\nYours is " + value.Length + ".", "value");
            if (compareString.Length > maxlen)
                throw new ArgumentException("\nMaximum string length in Levenshtein.iLD is " + maxlen + ".\nYours is " + compareString.Length + ".", "compareString");

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

            // Step 1

            if (RowLen == 0 && ColLen == 0)
            {
                return 0;
            }

            if (ColLen == 0 || RowLen == 0)
            {
                return returnPercentage ? 100 : Math.Max(ColLen, RowLen);
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
            Console.WriteLine("iDist=" + v0[RowLen]);
            if (returnPercentage)
            {
                int max = Math.Max(RowLen, ColLen);
                return ((100*v0[RowLen])/max);
            }
            return v0[RowLen];
        }
    }
}
