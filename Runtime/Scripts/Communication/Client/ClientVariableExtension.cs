using System.Text.RegularExpressions;
using UnityEngine;

namespace OC.Communication
{
    public static class ClientVariableExtension
    {
        private static readonly Regex InvalidChars = new ("[^A-Za-z0-9_]", RegexOptions.Compiled);
        
        /// <summary>
        /// Returns true if:
        ///  - name is not null/empty
        ///  - first char is a letter
        ///  - all chars are letters, digits or underscore
        /// </summary>
        public static bool IsVariableNameValid(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            if (!char.IsLetter(name[0])) return false;
            
            for (var i = 1; i < name.Length; i++)
            {
                var character = name[i];
                if (!(char.IsLetterOrDigit(character) || character == '_')) return false;
            }
            
            return true;
        }

        /// <summary>
        /// Replace spaces and hyphens with underscore
        /// Remove any other invalid characters  
        /// Ensure it starts with a letter by prefixing 'A' if needed  
        /// </summary>
        public static string CorrectVariableName(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            var withUnderscores = Regex.Replace(input, @"[\s-]+", "_");
            var cleaned = InvalidChars.Replace(withUnderscores, "");

            if (char.IsLetter(cleaned[0])) return cleaned;
            
            var name = "A" + cleaned;
            
            cleaned = name;

            return cleaned;
        }
    }
}