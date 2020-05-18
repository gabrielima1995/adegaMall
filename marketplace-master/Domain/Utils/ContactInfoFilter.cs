using System.Text.RegularExpressions;

namespace Domain.Utils
{
    public class ContactInfoFilter
    {
        private static readonly RegexOptions flags = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        private static readonly Regex emailPattern = new Regex(@"[\w+\._\-]+@[\w\.]+\.\w+", flags);
        private static readonly Regex phonePattern = new Regex(@"(?>\(?\d\d\)?\s*)?(?>[\d][ \-\.]?){7,10}\d", flags);
        //private static readonly Regex urlPattern = new Regex(@"((http|ftp|https):\/\/([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-])?|((http|ftp|https):\/\/)?www.\w+.\w+(\/\w*)*)", flags);

        public FilterResult Filter(string text)
        {
            var foundAnyMatches = false;

            if (emailPattern.IsMatch(text))
            {
                text = emailPattern.Replace(text, " ************@*****.*** ");
                foundAnyMatches = true;
            }

            if (phonePattern.IsMatch(text))
            {
                text = phonePattern.Replace(text, " ********* ");
                foundAnyMatches = true;
            }

            //if (urlPattern.IsMatch(text))
            //{
            //    text = urlPattern.Replace(text, " ***.********.*** ");
            //    foundAnyMatches = true;
            //}

            return new FilterResult(text, foundAnyMatches);
        }

        public class FilterResult
        {
            internal FilterResult(string filteredText, bool foundAnyMatches)
            {
                FilteredText = filteredText;
                FoundAnyMatches = foundAnyMatches;
            }

            public string FilteredText { get; }
            public bool FoundAnyMatches { get; }
        }
    }
}
