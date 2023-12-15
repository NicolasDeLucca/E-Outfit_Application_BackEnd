namespace Blogs.Domain
{
    public static class Utilities
    {
        public static bool IsWordInText(string text, string word)
        {
            string[] words = text.Split(new[] {' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?'},
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string w in words)
            {
                if (string.Equals(w, word, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static bool ExistsWord(List<string> words, string text)
        {
            return words.ToList().Exists(w => IsWordInText(text, w));
        }
    }
}
