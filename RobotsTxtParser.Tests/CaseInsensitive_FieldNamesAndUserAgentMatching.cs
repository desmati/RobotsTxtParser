namespace RobotsTxtParser.Tests;


public partial class RobotsTests
{
    [TestMethod]
    public void CaseInsensitive_FieldNamesAndUserAgentMatching()
    {
        var content = @"
USER-AGENT: TestBot
ALLOW: /ok
DISALLOW: /no";
        var robots = Robots.Load(content);

        Assert.IsTrue(robots.IsPathAllowed("TestBot", "/ok"), "Case-insensitive 'ALLOW' should allow '/ok'");
        Assert.IsFalse(robots.IsPathAllowed("TestBot", "/no"), "Case-insensitive 'DISALLOW' should disallow '/no'");
    }
}
