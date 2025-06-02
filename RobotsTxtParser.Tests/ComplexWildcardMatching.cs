namespace RobotsTxtParser.Tests;

public partial class RobotsTests
{
    [TestMethod]
    public void ComplexWildcardMatching()
    {
        var content = @"
User-agent: *
Disallow: /folder/*.html
Allow: /folder/public*.html
Disallow: /folder/secret$";
        var robots = Robots.Load(content);

        // "/folder/page.html" matches wildcard Disallow → disallowed
        Assert.IsFalse(robots.IsPathAllowed("any", "/folder/page.html"));

        // "/folder/public1.html" matches Allow with wildcard → allowed
        Assert.IsTrue(robots.IsPathAllowed("any", "/folder/public1.html"));

        // "/folder/secret" matches the "/folder/secret$" rule (exact match) → disallowed
        Assert.IsFalse(robots.IsPathAllowed("any", "/folder/secret"));

        // "/folder/secret.html" matches wildcard Disallow "'/folder/*.html" → disallowed
        Assert.IsFalse(robots.IsPathAllowed("any", "/folder/secret.html"));
    }
}
