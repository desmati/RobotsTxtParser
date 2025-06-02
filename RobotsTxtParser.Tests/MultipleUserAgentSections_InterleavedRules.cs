namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void MultipleUserAgentSections_InterleavedRules()
    {
        var content = @"
User-agent: A
Disallow: /a

User-agent: B
Disallow: /b

User-agent: *
Disallow: /common
";
        var robots = Robots.Load(content);

        // A has a specific rule for "/a" and ignores the global "/common"
        Assert.IsFalse(robots.IsPathAllowed("A", "/a"));
        Assert.IsTrue(robots.IsPathAllowed("A", "/common"));

        // B has a specific rule for "/b" and ignores the global "/common"
        Assert.IsFalse(robots.IsPathAllowed("B", "/b"));
        Assert.IsTrue(robots.IsPathAllowed("B", "/common"));

        // C has no specific rules, so only the global rule applies
        Assert.IsFalse(robots.IsPathAllowed("C", "/common"));
        // C should be allowed for "/a" and "/b" since those only match A/B’s specific rules
        Assert.IsTrue(robots.IsPathAllowed("C", "/a"));
        Assert.IsTrue(robots.IsPathAllowed("C", "/b"));
    }
}

