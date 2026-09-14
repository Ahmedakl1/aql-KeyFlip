using System;
using aql.KeyFlip.Core.Conversion;

namespace aql.KeyFlip.Core.Detection;

public static class LanguageDetector
{
    public static string DetectOverallDirection(string text, ConversionMode mode = ConversionMode.Auto)
    {
        if (mode == ConversionMode.EnglishToArabic) return "EN_TO_AR";
        if (mode == ConversionMode.ArabicToEnglish) return "AR_TO_EN";
        if (string.IsNullOrWhiteSpace(text)) return "EN_TO_AR";

        int arabic = 0, latin = 0;
        foreach (char c in text)
        {
            if (IsArabic(c)) arabic++;
            else if (IsLatin(c)) latin++;
        }

        if (arabic == 0 && latin > 0) return "EN_TO_AR";
        if (latin == 0 && arabic > 0) return "AR_TO_EN";
        return latin >= arabic ? "EN_TO_AR" : "AR_TO_EN";
    }

    public static bool IsMistypedArabicOnEnglish(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        if (WordLists.CommonEnglishWords.Contains(token)) return false;
        if (!ContainsLatin(token)) return false;

        string candidate = ConvertLatinTokenToArabic(token);
        int sourceLatin = CountLatin(token);
        int candidateArabic = CountArabic(candidate);
        return candidateArabic >= Math.Max(2, sourceLatin / 2);
    }

    public static bool IsLikelyArabicLayoutText(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        string candidate = TextConverter.ConvertEnToAr(token);
        return CountArabic(candidate) > CountLatin(token);
    }

    private static string ConvertLatinTokenToArabic(string token) => TextConverter.ConvertEnToAr(token);
    private static bool ContainsLatin(string text) => CountLatin(text) > 0;
    private static int CountLatin(string text)
    {
        int count = 0;
        foreach (char c in text) if (IsLatin(c)) count++;
        return count;
    }

    private static int CountArabic(string text)
    {
        int count = 0;
        foreach (char c in text) if (IsArabic(c)) count++;
        return count;
    }

    private static bool IsLatin(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    private static bool IsArabic(char c) => c >= '\u0600' && c <= '\u06FF';
}
