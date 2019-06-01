using System;
using System.Linq;

namespace petshelterApi.Helpers
{
    public static class StringExtensions
    {
        public static string ToCapitalizedString(this string value)
        {
            var capitalized = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                return capitalized;
            }
            var separators = new[] { ' ', '-' };
            var addCapitalized = true;
            foreach(var c in value)
            {
                if (addCapitalized)
                {
                    capitalized += char.ToUpper(c);
                    addCapitalized = false;
                }
                else
                {
                    capitalized += c;
                }
                if (separators.Contains(c))
                {
                    addCapitalized = true;
                }
            }
            return capitalized;
        }
    }
}
