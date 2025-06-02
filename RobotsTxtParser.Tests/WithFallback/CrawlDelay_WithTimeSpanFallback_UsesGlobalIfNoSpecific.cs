namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void CrawlDelay_WithTimeSpanFallback_UsesGlobalIfNoSpecific()
    {
        var content = @"
User-agent: XBot
Crawl-delay: 1.25

User-agent: *
Crawl-delay: 2.5";
        var robots = Robots.Load(content);
        TimeSpan fallback = TimeSpan.FromSeconds(10);

        // OtherBot no specific → global 2.5s = 2500ms; fallback ignored
        TimeSpan result = robots.CrawlDelay("OtherBot", fallback);
        Assert.AreEqual(TimeSpan.FromMilliseconds(2500), result);
    }
}
