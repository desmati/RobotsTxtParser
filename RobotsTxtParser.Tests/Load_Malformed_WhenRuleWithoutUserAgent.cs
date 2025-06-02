namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void Load_Malformed_WhenRuleWithoutUserAgent()
    {
        // Disallow appears before any User-agent → Malformed
        var content = @"
Disallow: /private
User-agent: *
Allow: /public";
        var robots = Robots.Load(content);

        Assert.IsTrue(robots.Malformed, "Malformed should be true when a rule appears before any User-agent");
        Assert.IsTrue(robots.HasRules, "HasRules should be true once a valid rule is parsed");

        // Because the initial "Disallow: /private" is ignored (no preceding User-agent),
        // "/private" remains allowed. Only "/public" is explicitly allowed under "*".
        Assert.IsTrue(robots.IsPathAllowed("any", "/private"), "Path '/private' should be allowed (the early Disallow is ignored)");
        Assert.IsTrue(robots.IsPathAllowed("any", "/public"), "Path '/public' should be allowed after valid User-agent");
    }

}
