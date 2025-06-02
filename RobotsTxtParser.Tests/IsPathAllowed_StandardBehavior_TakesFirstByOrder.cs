namespace RobotsTxtParser.Tests;


public partial class RobotsTests
{
    [TestMethod]
    public void IsPathAllowed_StandardBehavior_TakesFirstByOrder()
    {
        var content = @"
User-agent: *
Disallow: /foo
Allow: /foo";

        var robots = Robots.Load(content);
        robots.AllowRuleImplementation = AllowRuleImplementation.Standard;

        // Order: Disallow rule has order=1, Allow has order=2
        // Standard: pick rule with lowest Order → Disallow
        Assert.IsFalse(robots.IsPathAllowed("any", "/foo"), "Standard: Disallow should win because it has earlier order");

        // If we swap order by placing Allow first
        var swapped = Robots.Load(@"
User-agent: *
Allow: /foo
Disallow: /foo");
        swapped.AllowRuleImplementation = AllowRuleImplementation.Standard;

        Assert.IsTrue(swapped.IsPathAllowed("any", "/foo"), "Standard: Allow should win because it appears first");
    }
}
