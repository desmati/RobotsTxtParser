namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void CrawlDelay_WithIntFallback_UsesGlobalIfNoSpecific()
    {
        var content = @"
User-agent: MyBot
Crawl-delay: 3.5

User-agent: *
Crawl-delay: 2";
        var robots = Robots.Load(content);

        // No specific for OtherBot → global 2s = 2000ms; fallback 5000ms ignored
        TimeSpan result = robots.CrawlDelay("OtherBot", 5000);
        Assert.AreEqual(TimeSpan.FromMilliseconds(2000), result);
    }
}
