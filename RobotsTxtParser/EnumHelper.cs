namespace RobotsTxtParser;

/// <summary>
/// Provides helper methods for mapping a raw field name to a <see cref="LineType"/>.
/// </summary>
internal static class EnumHelper
{
    /// <summary>
    /// Returns the <see cref="LineType"/> corresponding to a given field string (case‐insensitive).
    /// </summary>
    /// <param name="field">The field name from a robots.txt line (e.g. "User-Agent", "Allow", etc.).</param>
    /// <returns>The matching <see cref="LineType"/>, or <see cref="LineType.Unknown"/> if no match is found.</returns>
    public static LineType GetLineType(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            return LineType.Unknown;

        return field.ToLowerInvariant() switch
        {
            "user-agent" => LineType.UserAgent,
            "allow" or "disallow" => LineType.AccessRule,
            "crawl-delay" => LineType.CrawlDelayRule,
            "sitemap" => LineType.Sitemap,
            _ => LineType.Unknown,
        };
    }
}
