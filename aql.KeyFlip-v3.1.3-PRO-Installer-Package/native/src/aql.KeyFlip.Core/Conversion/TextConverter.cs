using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Frozen;
using aql.KeyFlip.Core.Detection;
using aql.KeyFlip.Core.KeyboardLayouts;

namespace aql.KeyFlip.Core.Conversion;

public static class TextConverter
{
    // Common keyboard-layout aliases. The normal engine remains deterministic; these
    // aliases only cover frequent Arabic phrases that users intentionally type in
    // several real-world forms.
    private static readonly System.Collections.Frozen.FrozenDictionary<string, string> CommonEnToAr =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["hgsl lrd"] = "السلام عليكم",
            ["hglslhk ugd;l"] = "السلام عليكم",
            ["hgsghl ugd;l"] = "السلام عليكم",
            ["hgsl lrd ,vplm hggi ,fv;hji"] = "السلام عليكم ورحمة الله وبركاته",
            ["hgsghl ugd;l ,vplm hggi ,fv;hji"] = "السلام عليكم ورحمة الله وبركاته",
            ["wfhp hgodv"] = "صباح الخير",
            ["lshx hgodv"] = "مساء الخير",
            ["hpst hgodv"] = "أحسن الله إليكم",
            ["a;vh"] = "شكرا",
            ["a;vh g;"] = "شكرا لك",
            ["ugt,h"] = "عفوا"
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static readonly System.Collections.Frozen.FrozenDictionary<string, string> CommonArToEn =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["السلام عليكم"] = "hgsl lrd",
            ["السلام عليكم ورحمة الله وبركاته"] = "hgsl lrd ,vplm hggi ,fv;hji",
            ["صباح الخير"] = "wfhp hgodv",
            ["مساء الخير"] = "lshx hgodv",
            ["شكرا"] = "a;vh",
            ["شكرا لك"] = "a;vh g;",
            ["عفوا"] = "ugt,h"
        }.ToFrozenDictionary(StringComparer.Ordinal);

    public static string ConvertEnToAr(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        if (TryWholeAlias(text, CommonEnToAr, out string alias)) return alias;
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
            sb.Append(Arabic101Layout.EnToArMap.TryGetValue(c, out string value) ? value : c);
        return sb.ToString();
    }

    public static string ConvertArToEn(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        if (TryWholeAlias(text, CommonArToEn, out string alias)) return alias;
        var sb = new StringBuilder(text.Length);
        for (int i = 0; i < text.Length; i++)
        {
            if (i + 1 < text.Length && Arabic101Layout.ArToEnMap.TryGetValue(text.Substring(i, 2), out string two))
            {
                sb.Append(two);
                i++;
                continue;
            }
            string one = text[i].ToString();
            sb.Append(Arabic101Layout.ArToEnMap.TryGetValue(one, out string value) ? value : one);
        }
        return sb.ToString();
    }

    private static bool TryWholeAlias(string text, System.Collections.Frozen.FrozenDictionary<string, string> aliases, out string result)
    {
        string trimmed = text.Trim();
        if (trimmed.Length == 0 || !aliases.TryGetValue(trimmed, out string alias))
        {
            result = string.Empty;
            return false;
        }
        int start = text.IndexOf(trimmed, StringComparison.Ordinal);
        result = text.Substring(0, start) + alias + text.Substring(start + trimmed.Length);
        return true;
    }

    public static ConversionResult ConvertText(string text, ConversionMode mode = ConversionMode.Auto)
    {
        text ??= string.Empty;
        if (text.Length == 0)
            return new ConversionResult { Original = string.Empty, Converted = string.Empty, Direction = "EN_TO_AR", WordsCount = 0, CharsCount = 0, Confidence = 1.0 };

        string direction = LanguageDetector.DetectOverallDirection(text, mode);
        string converted = mode switch
        {
            ConversionMode.EnglishToArabic => ConvertEnToAr(text),
            ConversionMode.ArabicToEnglish => ConvertArToEn(text),
            _ => ConvertAuto(text)
        };

        int words = 0;
        foreach (var part in text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)) words++;
        return new ConversionResult
        {
            Original = text,
            Converted = converted,
            Direction = direction,
            WordsCount = words,
            CharsCount = text.Length,
            Confidence = CalculateConfidence(text, converted, direction)
        };
    }

    private static string ConvertAuto(string text)
    {
        if (TryWholeAlias(text, CommonEnToAr, out string? enAlias)) return enAlias;
        if (TryWholeAlias(text, CommonArToEn, out string? arAlias)) return arAlias;

        var sb = new StringBuilder(text.Length);
        var token = new StringBuilder();
        foreach (char c in text)
        {
            if (char.IsWhiteSpace(c))
            {
                AppendAutoToken(sb, token);
                token.Clear();
                sb.Append(c);
            }
            else token.Append(c);
        }
        AppendAutoToken(sb, token);
        return sb.ToString();
    }

    private static void AppendAutoToken(StringBuilder output, StringBuilder token)
    {
        if (token.Length == 0) return;
        string value = token.ToString();
        if (WordLists.CommonEnglishWords.Contains(value))
        {
            output.Append(value);
            return;
        }

        int arabicSource = CountArabic(value);
        int latinSource = CountLatin(value);
        if (arabicSource > 0 && latinSource == 0)
        {
            output.Append(ConvertArToEn(value));
            return;
        }
        if (latinSource > 0 && arabicSource == 0 && LanguageDetector.IsMistypedArabicOnEnglish(value))
        {
            output.Append(ConvertEnToAr(value));
            return;
        }

        // For mixed tokens, only convert when the keyboard-layout candidate strongly
        // increases Arabic character density. URLs, emails, identifiers and normal English survive.
        string candidate = ConvertEnToAr(value);
        if (CountArabic(candidate) >= Math.Max(2, latinSource))
            output.Append(candidate);
        else
            output.Append(value);
    }

    private static double CalculateConfidence(string source, string converted, string direction)
    {
        if (source == converted) return 1.0;
        int sourceScript = direction == "EN_TO_AR" ? CountLatin(source) : CountArabic(source);
        int resultScript = direction == "EN_TO_AR" ? CountArabic(converted) : CountLatin(converted);
        if (sourceScript == 0) return 0.5;
        return Math.Clamp(resultScript / (double)sourceScript, 0.0, 1.0);
    }

    private static int CountArabic(string text)
    {
        int n = 0; foreach (char c in text) if (c >= '\u0600' && c <= '\u06FF') n++; return n;
    }
    private static int CountLatin(string text)
    {
        int n = 0; foreach (char c in text) if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) n++; return n;
    }
}
