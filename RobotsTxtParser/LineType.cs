namespace RobotsTxtParser;

/// <summary>
/// Represents the various kinds of lines that can appear in a robots.txt file.
/// </summary>
internal enum LineType
{
    /// <summary>
    /// A comment line (starts with “#”).
    /// </summary>
    Comment,

    /// <summary>
    /// A “User-agent” line indicating which crawler the following rules apply to.
    /// </summary>
    UserAgent,

    /// <summary>
    /// A “Sitemap” line providing the URL of a sitemap.
    /// </summary>
    Sitemap,

    /// <summary>
    /// An “Allow” or “Disallow” line defining an access rule.
    /// </summary>
    AccessRule,

    /// <summary>
    /// A “Crawl-delay” line specifying how many seconds to wait between requests.
    /// </summary>
    CrawlDelayRule,

    /// <summary>
    /// Any unrecognized or unsupported field.
    /// </summary>
    Unknown,
}
