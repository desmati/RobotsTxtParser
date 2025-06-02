namespace RobotsTxtParser;

internal class AccessRule : Rule
{
    /// <summary>
    /// Gets the URL path this rule applies to (always starts with "/").
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Indicates whether access is allowed (true for "Allow", false otherwise).
    /// </summary>
    public bool Allowed { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AccessRule"/>.
    /// </summary>
    /// <param name="userAgent">The user-agent string (non-null, non-empty).</param>
    /// <param name="line">The parsed line from robots.txt (non-null).</param>
    /// <param name="order">The order in which this rule should be evaluated.</param>
    public AccessRule(string userAgent, Line line, int order)
        : base(userAgent, order)
    {
        if (line is null)
            throw new ArgumentNullException(nameof(line));

        // Normalize the path so it always begins with "/"
        var raw = line.Value ?? string.Empty;
        if (raw.Length > 0 && !raw.StartsWith("/"))
            raw = "/" + raw;

        Path = raw;
        Allowed = string.Equals(line.Field, "allow", StringComparison.OrdinalIgnoreCase);
    }
}
