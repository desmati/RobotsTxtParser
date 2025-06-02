namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void CrawlDelay_WithTimeSpanFallback_ReturnsFallbackWhenNoRule()
    {
        var content = @"
User-agent: XBot
Crawl-delay: 1.25
";
        var robots = Robots.Load(content);
        TimeSpan fallback = TimeSpan.FromMilliseconds(1234);

        // No rule for OtherBot and no global → fallback used
        TimeSpan result = robots.CrawlDelay("OtherBot", fallback);
        Assert.AreEqual(TimeSpan.FromMilliseconds(1234), result);
    }
}
