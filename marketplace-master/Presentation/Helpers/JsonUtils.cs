namespace Presentation.Helpers
{
    public static class JsonUtils
    {
        /// <summary>
        /// Escapes single quotes and other problematic characters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EscapeJsonString(string str)
        {
            var finalString = str.Replace(@"'", @"\'");
            return finalString;
        }


    }
}