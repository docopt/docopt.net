namespace DocoptNet.Internals
{
    static partial class GenerateCodeHelper
    {
        public static string ConvertToPascalCase(string s)
        {
            // Start with uppercase char
            var makeUpperCase = true;
            var result = "";
            for (var i = 0; i < s.Length; i++)
            {
                var replacement = s[i] switch
                {
                    '-' or ' ' when i > 0 => string.Empty,
                    '-' when i == 0 && s.Length == 1 => "Minus",
                    '!' => "Exclamation",
                    '#' => "Hash",
                    '$' => "Dollar",
                    '%' => "Percent",
                    '&' => "Ampersand",
                    '*' => "Star",
                    '+' => "Plus",
                    ',' => "Comma",
                    '.' => "Dot",
                    '/' => "Slash",
                    ':' => "Colon",
                    ';' => "SemiColon",
                    '=' => "Equal",
                    '?' => "Question",
                    _ => null,
                };
                if (replacement is {} r)
                {
                    result += r;
                    makeUpperCase = true;
                }
                else if (s[i] is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9')
                {
                    result += makeUpperCase && char.IsLower(s, i) ? char.ToUpperInvariant(s[i]) : s[i];
                    makeUpperCase = false;
                }
            }

            return result;
        }
    }
}
