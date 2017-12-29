namespace JavaScript.Manager.Extensions
{
    internal static class StringExtensions
    {
        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (input.Length == 1)
            {
                return input.ToLowerInvariant();
            }

            return input.Substring(0, 1).ToLowerInvariant() + input.Substring(1);
        }

        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (input.Length == 1)
            {
                return input.ToUpperInvariant();
            }

            return input.Substring(0, 1).ToUpperInvariant() + input.Substring(1);

        }
    }
}