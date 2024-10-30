namespace Rise.Client.Utils
{
    public static class ExtensionMethods
    {
        private const string universalDateFormat = "yyyy-MM-dd";
        public static string ToUniversalStringDate(this DateOnly date)
        {
            return date.ToString(universalDateFormat);
        }
    }
}

