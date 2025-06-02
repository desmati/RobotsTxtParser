using System;

namespace RobotsTxtParser;

/// <summary>
/// Represents a Sitemap directive in a robots.txt file, encapsulating the raw value
/// and, if valid, the parsed <see cref="Uri"/>.
/// </summary>
public class Sitemap
{
    /// <summary>
    /// Gets the parsed sitemap URL, or <c>null</c> if the raw value is not a valid absolute URI.
    /// </summary>
    public Uri? Url { get; }

    /// <summary>
    /// Gets the raw string value of the sitemap directive from robots.txt.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Sitemap"/> with the specified raw value and parsed URL.
    /// </summary>
    /// <param name="value">
    /// The raw string from the robots.txt “Sitemap:” line (non-null, possibly empty).
    /// </param>
    /// <param name="url">
    /// The parsed <see cref="Uri"/> if <paramref name="value"/> was a valid absolute URI; otherwise <c>null</c>.
    /// </param>
    private Sitemap(string value, Uri? url)
    {
        Value = value;
        Url = url;
    }

    /// <summary>
    /// Creates a <see cref="Sitemap"/> instance by parsing the provided <paramref name="line"/>.
    /// </summary>
    /// <param name="line">The <see cref="Line"/> object representing a “Sitemap:” directive (non-null).</param>
    /// <returns>
    /// A <see cref="Sitemap"/> whose <see cref="Value"/> is <c>line.Value</c> (or <c>string.Empty</c> if null),
    /// and whose <see cref="Url"/> is the parsed <see cref="Uri"/> if valid; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="line"/> is <c>null</c>.
    /// </exception>
    internal static Sitemap FromLine(Line line)
    {
        if (line is null)
            throw new ArgumentNullException(nameof(line));

        // Ensure we have a non-null raw value.
        var raw = line.Value ?? string.Empty;

        // Attempt to create a Uri. If it fails (invalid format), leave Url as null.
        Uri? parsedUri = null;
        if (!string.IsNullOrWhiteSpace(raw) &&
            Uri.TryCreate(raw, UriKind.Absolute, out var candidate))
        {
            parsedUri = candidate;
        }

        return new Sitemap(raw, parsedUri);
    }
}
