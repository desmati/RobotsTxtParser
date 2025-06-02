namespace RobotsTxtParser.Tests;


public partial class RobotsTests
{
    [TestMethod]
    public void Load_EmptyContent_HasNoRules()
    {
        var robots = Robots.Load(string.Empty);

        Assert.IsFalse(robots.HasRules, "HasRules should be false for empty content");
        Assert.IsFalse(robots.Malformed, "Malformed should be false for empty content");
        Assert.AreEqual(0, robots.Sitemaps.Count, "Sitemaps should be empty");
        Assert.IsTrue(robots.IsPathAllowed("any", "/anything"), "All paths should be allowed when no rules");
        Assert.AreEqual(TimeSpan.Zero, robots.CrawlDelay("any"), "CrawlDelay should be zero when no rules");
    }
}
