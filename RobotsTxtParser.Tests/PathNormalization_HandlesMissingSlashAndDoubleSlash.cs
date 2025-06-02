namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void PathNormalization_HandlesMissingSlashAndDoubleSlash()
    {
        var content = @"
User-agent: *
Disallow: //folder//sub";
        var robots = Robots.Load(content);

        // The rule path is stored as "//folder//sub" (without normalization),
        // so it won’t match "/folder/sub" or any variant. All normalized inputs should therefore be allowed.
        Assert.IsTrue(robots.IsPathAllowed("any", "folder/sub"), "Missing leading slash should be added and then allowed (no match).");
        Assert.IsTrue(robots.IsPathAllowed("any", "/folder/sub"), "Normalized path does not match the raw rule, so it’s allowed.");
        Assert.IsTrue(robots.IsPathAllowed("any", "//folder///sub///page"), "Multiple slashes collapse, but still don’t match the raw rule, so it’s allowed.");
    }
}
