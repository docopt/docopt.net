namespace DocoptNet
{
    static class GenerateCodeHelper
    {
        public static string ConvertToPascalCase(string s)
        {
            // Start with uppercase char
            var makeUpperCase = true;
            var result = "";
            for (var i = 0; i < s.Length; i++)
            {
                if(s[i] is '-' or ' ')
                {
                    makeUpperCase = true;
                    continue;
                }

                result += makeUpperCase ? char.ToUpperInvariant(s[i]) : s[i];
                makeUpperCase = false;
            }

            return result;
        }
    }
}
