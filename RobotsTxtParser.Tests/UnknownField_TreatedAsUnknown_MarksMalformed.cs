namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void UnknownField_TreatedAsUnknown_MarksMalformed()
    {
        var content = @"
User-agent: Z
FooBar: /path";
        var robots = Robots.Load(content);

        Assert.IsTrue(robots.Malformed, "Unknown field 'FooBar' should mark as Malformed");
        // Since no valid AccessRule was parsed, all paths remain allowed
        Assert.IsTrue(robots.IsPathAllowed("Z", "/path"), "Unknown field should not affect path allowance");
    }
}
