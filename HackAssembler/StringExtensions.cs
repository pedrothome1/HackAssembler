using System.Text.RegularExpressions;

namespace HackAssembler
{
    public static class StringExtensions
    {
        public static string RemoveSpaces(this string str)
        {
            return Regex.Replace(str, " +", "");
        }
    }
}
