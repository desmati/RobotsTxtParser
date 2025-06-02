namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{

    [TestMethod]
    public void IsPathAllowed_DefaultMoreSpecificBehavior()
    {
        var content = @"
User-agent: *
Disallow: /private
Allow: /private/public";

        var robots = Robots.Load(content);

        // Default AllowRuleImplementation is MoreSpecific
        Assert.AreEqual(AllowRuleImplementation.MoreSpecific, robots.AllowRuleImplementation);

        // "/private" is disallowed
        Assert.IsFalse(robots.IsPathAllowed("any", "/private"), "Exact '/private' should be disallowed");

        // "/private/page" is disallowed because "/private" prefix matches first and is more specific than no allow
        Assert.IsFalse(robots.IsPathAllowed("any", "/private/page"));

        // "/private/public" is allowed because Allow rule with same length path overrides Disallow
        Assert.IsTrue(robots.IsPathAllowed("any", "/private/public"), "More specific allow should override disallow");

        // "/other" is allowed
        Assert.IsTrue(robots.IsPathAllowed("any", "/other"));
    }
}
