using System.Globalization;

namespace RobotsTxtParser;

internal class CrawlDelayRule : Rule
{
    /// <summary>
    /// Gets the crawl‐delay in milliseconds (parsed from the robots.txt value).
    /// </summary>
    public TimeSpan Delay { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="CrawlDelayRule"/>.
    /// </summary>
    /// <param name="userAgent">The user-agent string this rule targets (non-null, non-empty).</param>
    /// <param name="line">The parsed line from robots.txt (non-null).</param>
    /// <param name="order">The order in which this rule should be evaluated.</param>
    public CrawlDelayRule(string userAgent, Line line, int order)
        : base(userAgent, order)
    {
        if (line is null)
            throw new ArgumentNullException(nameof(line));

        // Try to parse the value as a floating-point number of seconds.
        // If parsing fails, default to 0.0 seconds.
        if (!double.TryParse(line.Value,
                             NumberStyles.Float,
                             CultureInfo.InvariantCulture,
                             out var seconds))
        {
            seconds = 0.0;
        }

        // Store crawl-delay in milliseconds.
        Delay = TimeSpan.FromSeconds(seconds);
    }
}
