namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void CrawlDelay_InvalidNumberDefaultsToZero()
    {
        var content = @"
User-agent: BotX
Crawl-delay: not_a_number";
        var robots = Robots.Load(content);

        Assert.AreEqual(TimeSpan.Zero, robots.CrawlDelay("BotX"), "Invalid crawl-delay should result in zero");
    }
}
