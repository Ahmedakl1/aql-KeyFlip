using Xunit;
using aql.KeyFlip.Core.Conversion;

namespace aql.KeyFlip.Tests;

public class ConversionTests
{
    [Theory]
    [InlineData("hgsl lrd", "السلام عليكم")]
    [InlineData("hglslhk ugd;l", "السلام عليكم")]
    [InlineData("hgsghl ugd;l", "السلام عليكم")]
    [InlineData("wfhp hgodv", "صباح الخير")]
    [InlineData("lshx hgodv", "مساء الخير")]
    [InlineData("a;vh", "شكرا")]
    [InlineData("ugt,h", "عفوا")]
    public void AutoConvertsCommonMistypedPhrases(string input, string expected) =>
        Assert.Equal(expected, TextConverter.ConvertText(input, ConversionMode.Auto).Converted);

    [Theory]
    [InlineData("السلام عليكم", "hgsl lrd")]
    [InlineData("صباح الخير", "wfhp hgodv")]
    [InlineData("مساء الخير", "lshx hgodv")]
    [InlineData("شكرا", "a;vh")]
    public void AutoConvertsArabicToEnglish(string input, string expected) =>
        Assert.Equal(expected, TextConverter.ConvertText(input, ConversionMode.Auto).Converted);

    [Fact]
    public void NumbersArePreserved() => Assert.Equal("12345", TextConverter.ConvertText("12345").Converted);

    [Fact]
    public void NormalEnglishWordsArePreservedInAuto() => Assert.Equal("Hello World", TextConverter.ConvertText("Hello World").Converted);

    [Fact]
    public void MixedTextConvertsMistypedTokenButPreservesNormalEnglishAndNumbers()
    {
        string result = TextConverter.ConvertText("Hello hgsl 123", ConversionMode.Auto).Converted;
        Assert.Contains("Hello", result);
        Assert.Contains("123", result);
        Assert.Contains("السل", result);
    }

    [Fact]
    public void ExplicitEnglishToArabicUsesPureLayoutMapping()
    {
        Assert.Equal("السلام عليكم", TextConverter.ConvertEnToAr("hgsghl ugd;l"));
    }

    [Fact]
    public void ExplicitArabicToEnglishUsesReverseMapping()
    {
        Assert.Equal("hgsghl ugd;l", TextConverter.ConvertArToEn("السلام عليكم"));
    }
}
