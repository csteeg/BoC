using System;
using System.Collections.Specialized;

namespace ModelMetadataExtensions.Extensions {
    public static class StringExtensions {
        public static string SplitUpperCaseToString(this string source) {
            if (source == null) {
                return null;
            }
            return string.Join(" ", source.SplitUpperCase());
        }

        public static string[] SplitUpperCase(this string source) {
            if (source == null)
                return new string[] { }; //Return empty array.

            if (source.Length == 0)
                return new string[] { "" };

            StringCollection words = new StringCollection();
            int wordStartIndex = 0;

            char[] letters = source.ToCharArray();
            char previousChar = char.MinValue;
            // Skip the first letter. we don't care what case it is.
            for (int i = 1; i < letters.Length; i++) {
                if (char.IsUpper(letters[i]) && !char.IsWhiteSpace(previousChar)) {
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

    }
}