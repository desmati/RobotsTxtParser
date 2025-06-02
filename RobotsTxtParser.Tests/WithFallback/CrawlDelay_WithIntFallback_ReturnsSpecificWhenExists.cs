namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void CrawlDelay_WithIntFallback_ReturnsSpecificWhenExists()
    {
        var content = @"
User-agent: MyBot
Crawl-delay: 3.5

User-agent: *
Crawl-delay: 2";
        var robots = Robots.Load(content);

        // Specific rule for MyBot is 3.5s = 3500ms; fallback 10000ms should be ignored
        TimeSpan result = robots.CrawlDelay("MyBot", 10000);
        Assert.AreEqual(TimeSpan.FromMilliseconds(3500), result);
    }
}
