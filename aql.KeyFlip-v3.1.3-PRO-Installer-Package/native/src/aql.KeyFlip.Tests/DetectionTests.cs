using Xunit;
using aql.KeyFlip.Core.Detection;

namespace aql.KeyFlip.Tests;

public class DetectionTests
{
    [Theory]
    [InlineData("hgsl", true)]
    [InlineData("ugd;l", true)]
    [InlineData("hglslhk", true)]
    [InlineData("hello", false)]
    [InlineData("world", false)]
    [InlineData("windows", false)]
    public void MistypedArabicDetectionIsDeterministic(string token, bool expected) =>
        Assert.Equal(expected, LanguageDetector.IsMistypedArabicOnEnglish(token));

    [Theory]
    [InlineData("hglslhk ugd;l", "EN_TO_AR")]
    [InlineData("السلام عليكم", "AR_TO_EN")]
    public void OverallDirectionIsDetected(string text, string expected) =>
        Assert.Equal(expected, LanguageDetector.DetectOverallDirection(text));
}
