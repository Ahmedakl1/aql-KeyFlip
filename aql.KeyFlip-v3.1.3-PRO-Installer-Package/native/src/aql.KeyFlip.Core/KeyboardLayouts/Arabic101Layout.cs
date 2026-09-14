using System.Collections.Frozen;
using System.Collections.Generic;

namespace aql.KeyFlip.Core.KeyboardLayouts;

/// <summary>
/// Deterministic mapping table between Microsoft Windows Arabic (101) Layout and English (US QWERTY).
/// Includes numbers, punctuation, Shift characters, Harakat (diacritics), and Lam-Alef ligatures.
/// </summary>
public static class Arabic101Layout
{
    public static readonly FrozenDictionary<char, string> EnToArMap;
    public static readonly FrozenDictionary<string, string> ArToEnMap;

    static Arabic101Layout()
    {
        var enToAr = new Dictionary<char, string>
        {
            // Row 1 (Numbers and symbols)
            ['`'] = "ذ", ['~'] = "ّ", // Shadda
            ['1'] = "1", ['!'] = "!",
            ['2'] = "2", ['@'] = "@",
            ['3'] = "3", ['#'] = "#",
            ['4'] = "4", ['$'] = "$",
            ['5'] = "5", ['%'] = "%",
            ['6'] = "6", ['^'] = "^",
            ['7'] = "7", ['&'] = "&",
            ['8'] = "8", ['*'] = "*",
            ['9'] = "9", ['('] = ")", // Mirrored in Arabic typing
            ['0'] = "0", [')'] = "(", // Mirrored in Arabic typing
            ['-'] = "-", ['_'] = "_",
            ['='] = "=", ['+'] = "+",

            // Row 2 (QWERTY)
            ['q'] = "ض", ['Q'] = "َ", // Fatha
            ['w'] = "ص", ['W'] = "ً", // Tanween Fath
            ['e'] = "ث", ['E'] = "ُ", // Damma
            ['r'] = "ق", ['R'] = "ٌ", // Tanween Damm
            ['t'] = "ف", ['T'] = "لإ", // Lam Alef Hamza below
            ['y'] = "غ", ['Y'] = "إ", // Alef Hamza below
            ['u'] = "ع", ['U'] = "‘", // Left single quote
            ['i'] = "ه", ['I'] = "÷", // Division
            ['o'] = "خ", ['O'] = "×", // Multiplication
            ['p'] = "ح", ['P'] = "؛", // Arabic semicolon
            ['['] = "ج", ['{'] = "<",
            [']'] = "د", ['}'] = ">",
            ['\\'] = "\\", ['|'] = "|",

            // Row 3 (ASDFGH)
            ['a'] = "ش", ['A'] = "ِ", // Kasra
            ['s'] = "س", ['S'] = "ٍ", // Tanween Kasr
            ['d'] = "ي", ['D'] = "]",
            ['f'] = "ب", ['F'] = "[",
            ['g'] = "ل", ['G'] = "لأ", // Lam Alef Hamza above
            ['h'] = "ا", ['H'] = "أ", // Alef Hamza above
            ['j'] = "ت", ['J'] = "ـ", // Tatweel
            ['k'] = "ن", ['K'] = "،", // Arabic comma
            ['l'] = "م", ['L'] = "/",
            [';'] = "ك", [':'] = ":",
            ['\''] = "ط", ['"'] = "\"",

            // Row 4 (ZXCVBN)
            ['z'] = "ئ", ['Z'] = "~",
            ['x'] = "ء", ['X'] = "ْ", // Sukun
            ['c'] = "ؤ", ['C'] = "}",
            ['v'] = "ر", ['V'] = "{",
            ['b'] = "لا", ['B'] = "لآ", // Lam Alef Madda
            ['n'] = "ى", ['N'] = "آ", // Alef Madda
            ['m'] = "ة", ['M'] = "’", // Right single quote
            [','] = "و", ['<'] = ",",
            ['.'] = "ز", ['>'] = ".",
            ['/'] = "ظ", ['?'] = "؟", // Arabic question mark
            [' '] = " ", ['\t'] = "\t", ['\n'] = "\n", ['\r'] = "\r"
        };

        EnToArMap = enToAr.ToFrozenDictionary();

        var arToEn = new Dictionary<string, string>
        {
            // Letters
            ["ذ"] = "`", ["ض"] = "q", ["ص"] = "w", ["ث"] = "e",
            ["ق"] = "r", ["ف"] = "t", ["غ"] = "y", ["ع"] = "u",
            ["ه"] = "i", ["خ"] = "o", ["ح"] = "p", ["ج"] = "[",
            ["د"] = "]", ["ش"] = "a", ["س"] = "s", ["ي"] = "d",
            ["ب"] = "f", ["ل"] = "g", ["ا"] = "h", ["ت"] = "j",
            ["ن"] = "k", ["م"] = "l", ["ك"] = ";", ["ط"] = "'",
            ["ئ"] = "z", ["ء"] = "x", ["ؤ"] = "c", ["ر"] = "v",
            ["ى"] = "n", ["ة"] = "m", ["و"] = ",", ["ز"] = ".",
            ["ظ"] = "/",

            // Composite Ligatures
            ["لا"] = "b", ["\uFEFB"] = "b", ["\uFEFC"] = "b",
            ["لأ"] = "G", ["\uFEF7"] = "G", ["\uFEF8"] = "G",
            ["لإ"] = "T", ["\uFEF9"] = "T", ["\uFEFA"] = "T",
            ["لآ"] = "B", ["\uFEF5"] = "B", ["\uFEF6"] = "B",

            // Modified Alefs
            ["أ"] = "H", ["إ"] = "Y", ["آ"] = "N",

            // Diacritics (Harakat)
            ["َ"] = "Q", ["ً"] = "W", ["ُ"] = "E", ["ٌ"] = "R",
            ["ِ"] = "A", ["ٍ"] = "S", ["ْ"] = "X", ["ّ"] = "~",
            ["ـ"] = "J",

            // Punctuation and symbols
            ["؛"] = "P", ["،"] = "K", ["؟"] = "?",
            ["÷"] = "I", ["×"] = "O", ["‘"] = "U", ["’"] = "M",

            // Mirrored brackets
            [")"] = "9", ["("] = "0",
            ["]"] = "D", ["["] = "F",
            ["}"] = "C", ["{"] = "V",
            [">"] = "}", ["<"] = "{",

            // Spacing
            [" "] = " ", ["\t"] = "\t", ["\n"] = "\n", ["\r"] = "\r"
        };

        ArToEnMap = arToEn.ToFrozenDictionary();
    }
}
