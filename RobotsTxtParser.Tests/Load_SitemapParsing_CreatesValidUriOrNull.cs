namespace RobotsTxtParser.Tests;


public partial class RobotsTests
{
    [TestMethod]
    public void Load_SitemapParsing_CreatesValidUriOrNull()
    {
        var content = @"
User-agent: *
Sitemap: http://example.com/sitemap.xml
Sitemap: invalid_uri
";
        var robots = Robots.Load(content);

        Assert.AreEqual(2, robots.Sitemaps.Count, "Should have two sitemap entries");
        var first = robots.Sitemaps[0];
        Assert.AreEqual("http://example.com/sitemap.xml", first.Value);
        Assert.IsNotNull(first.Url, "First Sitemap.Url should be a valid URI");
        Assert.AreEqual(new Uri("http://example.com/sitemap.xml"), first.Url);

        var second = robots.Sitemaps[1];
        Assert.AreEqual("invalid_uri", second.Value);
        Assert.IsNull(second.Url, "Second Sitemap.Url should be null because the URI is invalid");
    }
}
