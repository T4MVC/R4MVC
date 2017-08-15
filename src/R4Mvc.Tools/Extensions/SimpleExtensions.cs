namespace R4Mvc.Tools.Extensions
{
    public static class SimpleExtensions
    {
        public static string TrimEnd(this string value, string suffix)
        {
            if (value == null)
                return null;
            if (!value.EndsWith(suffix))
                return value;
            return value.Substring(0, value.Length - suffix.Length);
        }
    }
}
