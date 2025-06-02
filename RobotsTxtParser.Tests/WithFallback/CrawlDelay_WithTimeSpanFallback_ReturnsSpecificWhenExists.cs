namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void CrawlDelay_WithTimeSpanFallback_ReturnsSpecificWhenExists()
    {
        var content = @"
User-agent: XBot
Crawl-delay: 1.25

User-agent: *
Crawl-delay: 2.5";
        var robots = Robots.Load(content);
        TimeSpan fallback = TimeSpan.FromSeconds(10);

        // Specific XBot: 1.25s = 1250ms; fallback ignored
        TimeSpan result = robots.CrawlDelay("XBot", fallback);
        Assert.AreEqual(TimeSpan.FromMilliseconds(1250), result);
    }
}
