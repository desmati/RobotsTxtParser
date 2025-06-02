namespace RobotsTxtParser;

/// <summary>
/// Defines methods for checking access rules and crawl delays from a robots.txt source.
/// </summary>
public interface IRobotsParser
{
    /// <summary>
    /// Determines whether the specified <paramref name="path"/> is allowed for the given <paramref name="userAgent"/>.
    /// </summary>
    /// <param name="userAgent">The user-agent string to check (non-null, non-empty).</param>
    /// <param name="path">The URL path to test (non-null, leading slash optional).</param>
    /// <returns><c>true</c> if the path is allowed; otherwise, <c>false</c>.</returns>
    bool IsPathAllowed(string userAgent, string path);

    /// <summary>
    /// Retrieves the crawl-delay defined for the specified <paramref name="userAgent"/>. 
    /// </summary>
    /// <param name="userAgent">The user-agent string to query (non-null, non-empty).</param>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing the delay. 
    /// Implementations should return <see cref="TimeSpan.Zero"/> if no delay is specified or if parsing fails.
    /// </returns>
    TimeSpan CrawlDelay(string userAgent);
}
