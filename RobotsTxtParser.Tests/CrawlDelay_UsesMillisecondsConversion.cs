namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void CrawlDelay_UsesMillisecondsConversion()
    {
        var content = @"
User-agent: MyBot
Crawl-delay: 4.25

User-agent: *
Crawl-delay: 2";
        var robots = Robots.Load(content);

        // Specific first: 4.25 seconds = 4250 ms
        Assert.AreEqual(TimeSpan.FromMilliseconds(4250), robots.CrawlDelay("MyBot"));

        // Other agents get global delay: 2000 ms
        Assert.AreEqual(TimeSpan.FromMilliseconds(2000), robots.CrawlDelay("OtherBot"));

        // Since there is a global "*" rule, even unknown agents get 2000 ms
        Assert.AreEqual(TimeSpan.FromMilliseconds(2000), robots.CrawlDelay("Unknown"), "Unknown should fall back to the global rule");
    }

}
