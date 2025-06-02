namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void MultipleUserAgents_SpecificOverridesGlobal()
    {
        var content = @"
User-agent: BotA
Disallow: /secret

User-agent: *
Allow: /secret";
        var robots = Robots.Load(content);

        // BotA should be disallowed
        Assert.IsFalse(robots.IsPathAllowed("BotA", "/secret"), "Specific rule for BotA should disallow '/secret'");

        // Other bots should be allowed
        Assert.IsTrue(robots.IsPathAllowed("BotB", "/secret"), "Global allow should permit '/secret' for other user-agents");
    }
}
