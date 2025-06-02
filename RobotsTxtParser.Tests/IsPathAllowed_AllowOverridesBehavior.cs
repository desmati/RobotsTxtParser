namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void IsPathAllowed_AllowOverridesBehavior()
    {
        var content = @"
User-agent: *
Disallow: /foo
Allow: /foo";

        var robots = Robots.Load(content);
        robots.AllowRuleImplementation = AllowRuleImplementation.AllowOverrides;

        // AllowOverrides: if any matching rule has Allowed=true, path is allowed
        Assert.IsTrue(robots.IsPathAllowed("any", "/foo"), "AllowOverrides should allow because an Allow rule exists");

        // If only Disallow exists
        var onlyDisallow = Robots.Load(@"
User-agent: *
Disallow: /bar");
        onlyDisallow.AllowRuleImplementation = AllowRuleImplementation.AllowOverrides;
        Assert.IsFalse(onlyDisallow.IsPathAllowed("any", "/bar"), "Disallow without any Allow → disallowed");
    }
}
