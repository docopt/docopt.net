namespace DocoptNet
{
    public static class GenerateCodeHelper
    {
        public static string ConvertDashesToCamelCase(string s)
        {
            // Start with uppercase char
            var makeUpperCase = true;
            var result = "";
            for (int i = 0; i < s.Length; i++)
            {
                if(s[i] == '-')
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
