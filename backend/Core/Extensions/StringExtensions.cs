namespace Core.Extensions
{
    public static class StringExtensions
    {
        public static string FirstCharToLowerCase( this string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
                return str;
            if (str[0] == 'I')
                return "i" + str.Substring(1);
            return char.ToLower(str[0]) + str.Substring(1);
        }
    }
}