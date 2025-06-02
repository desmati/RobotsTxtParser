namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void HasRules_BecomesTrue_WhenAccessOrCrawlDelayAppears()
    {
        var content = @"
User-agent: X
# Just a comment

User-agent: Y
Disallow: /something
";
        var robots = Robots.Load(content);

        Assert.IsTrue(robots.HasRules, "HasRules should be true when a Disallow appears under a valid User-agent");
    }
}
