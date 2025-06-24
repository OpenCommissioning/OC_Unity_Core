namespace OC.Communication
{
    public static class ClientVariableExtension
    {
        /// <summary>
        /// Returns a path with each segment wrapped in backticks if it contains invalid characters.
        /// </summary>
        public static string GetClientCompatiblePath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;
            
            var parts = path.Split('.');
            
            for (var i = 0; i < parts.Length; i++)
            {
                if (!IsVariableNameValid(parts[i]))
                {
                    parts[i] = $"`{parts[i]}`";
                }
            }
            
            return string.Join(".", parts);
        }
        
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
    }
}