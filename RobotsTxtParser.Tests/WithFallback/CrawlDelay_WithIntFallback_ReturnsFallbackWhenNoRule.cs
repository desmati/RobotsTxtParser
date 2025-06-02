namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void CrawlDelay_WithIntFallback_ReturnsFallbackWhenNoRule()
    {
        var content = @"
User-agent: MyBot
Crawl-delay: 3.5
";
        var robots = Robots.Load(content);

        // No rule matches OtherBot, and no global → fallback 750ms used
        TimeSpan result = robots.CrawlDelay("OtherBot", 750);
        Assert.AreEqual(TimeSpan.FromMilliseconds(750), result);
    }
}
