using System.Collections.Frozen;
using System.Collections.Generic;

namespace aql.KeyFlip.Core.Detection;

public static class WordLists
{
    public static readonly FrozenSet<string> CommonEnglishWords;
    public static readonly string[] CommonArabicPrefixesInEn;

    static WordLists()
    {
        var words = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "the", "be", "to", "of", "and", "a", "in", "that", "have", "i",
            "it", "for", "not", "on", "with", "he", "as", "you", "do", "at",
            "this", "but", "his", "by", "from", "they", "we", "say", "her", "she",
            "or", "an", "will", "my", "one", "all", "would", "there", "their", "what",
            "so", "up", "out", "if", "about", "who", "get", "which", "go", "me",
            "when", "make", "can", "like", "time", "no", "just", "him", "know", "take",
            "people", "into", "year", "your", "good", "some", "could", "them", "see", "other",
            "than", "then", "now", "look", "only", "come", "its", "over", "think", "also",
            "back", "after", "use", "two", "how", "our", "work", "first", "well", "way",
            "even", "new", "want", "because", "any", "these", "give", "day", "most", "us",
            "hello", "world", "google", "microsoft", "windows", "app", "code", "file", "test"
        };
        CommonEnglishWords = words.ToFrozenSet(System.StringComparer.OrdinalIgnoreCase);

        CommonArabicPrefixesInEn = new[]
        {
            "hg",   // ال (Definite article)
            ",hg",  // وال
            "thg",  // فال
            "fhg",  // بال
            ";hg",  // كال
            "ghg",  // لل
            "sh",   // س (Future prefix)
            "lh",   // ما
            "lk",   // من
            "ug"    // عل
        };
    }
}
