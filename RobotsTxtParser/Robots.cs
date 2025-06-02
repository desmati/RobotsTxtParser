namespace RobotsTxtParser;

/// <summary>
/// Represents the parsed contents of a robots.txt and provides methods
/// to check access rules and crawl-delay values for a given user-agent.
/// </summary>
public class Robots : IRobotsParser
{
    // Internal collections for rules; initialized in readLines()
    private List<AccessRule> _globalAccessRules = new();
    private List<AccessRule> _specificAccessRules = new();
    private List<CrawlDelayRule> _crawlDelayRules = new();

    /// <summary>
    /// Gets the raw robots.txt content (may be empty if none was provided).
    /// </summary>
    public string Raw { get; }

    /// <summary>
    /// Gets the list of parsed Sitemap directives (may be empty).
    /// </summary>
    public List<Sitemap> Sitemaps { get; private set; } = new();

    /// <summary>
    /// True if any line was malformed or out of expected order.
    /// </summary>
    public bool Malformed { get; private set; }

    /// <summary>
    /// True if at least one Access or Crawl-Delay rule was parsed.
    /// </summary>
    public bool HasRules { get; private set; }

    /// <summary>
    /// True if at least one Disallow rule with a non-empty path exists.
    /// </summary>
    public bool IsAnyPathDisallowed { get; private set; }

    /// <summary>
    /// Determines how “Allow” rules are evaluated relative to “Disallow” rules.
    /// </summary>
    public AllowRuleImplementation AllowRuleImplementation { get; set; }

    /// <summary>
    /// Parses the given robots.txt content into a new <see cref="Robots"/> instance.
    /// </summary>
    /// <param name="content">The full text of robots.txt (may be null or whitespace).</param>
    public static Robots Load(string content) => new(content);

    /// <summary>
    /// Initializes a new instance by parsing the provided <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The text of robots.txt. If null or whitespace, no rules are parsed.</param>
    public Robots(string content)
    {
        AllowRuleImplementation = AllowRuleImplementation.MoreSpecific;
        Raw = content ?? string.Empty;

        if (string.IsNullOrWhiteSpace(Raw))
            return;

        // Split on any newline characters, discard empty/whitespace-only lines
        var lines = Raw
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();

        if (lines.Length == 0)
            return;

        ReadLines(lines);
    }

    private void ReadLines(string[] lines)
    {
        // Reset collections in case this is called more than once
        _globalAccessRules = new List<AccessRule>();
        _specificAccessRules = new List<AccessRule>();
        _crawlDelayRules = new List<CrawlDelayRule>();
        Sitemaps = new List<Sitemap>();

        string currentUserAgent = string.Empty;
        int orderCounter = 0;

        foreach (var rawLine in lines)
        {
            var line = new Line(rawLine);

            switch (line.Type)
            {
                case LineType.UserAgent:
                    // The value of a User-agent line is the user-agent string to apply next
                    currentUserAgent = line.Value!;
                    break;

                case LineType.Sitemap:
                    // Add a Sitemap entry (Value may or may not parse as a valid URI)
                    Sitemaps.Add(Sitemap.FromLine(line));
                    break;

                case LineType.AccessRule:
                case LineType.CrawlDelayRule:
                    if (string.IsNullOrEmpty(currentUserAgent))
                    {
                        // Found a rule before any "User-agent" directive
                        Malformed = true;
                        break;
                    }

                    if (line.Type == LineType.AccessRule)
                    {
                        var accessRule = new AccessRule(currentUserAgent, line, ++orderCounter);
                        if (accessRule.UserAgent == "*")
                            _globalAccessRules.Add(accessRule);
                        else
                            _specificAccessRules.Add(accessRule);

                        if (!accessRule.Allowed && !string.IsNullOrEmpty(accessRule.Path))
                            IsAnyPathDisallowed = true;
                    }
                    else
                    {
                        _crawlDelayRules.Add(new CrawlDelayRule(currentUserAgent, line, ++orderCounter));
                    }

                    HasRules = true;
                    break;

                case LineType.Unknown:
                    Malformed = true;
                    break;

                // Comment lines are ignored
                case LineType.Comment:
                    break;
            }
        }
    }

    /// <inheritdoc/>
    public bool IsPathAllowed(string userAgent, string path)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            throw new ArgumentException("Not a valid user-agent string.", nameof(userAgent));

        // If there are no rules or no Disallow rules, everything is allowed
        if (!HasRules || !IsAnyPathDisallowed)
            return true;

        path = NormalizePath(path);

        // Find all specific rules whose UserAgent substring matches (case-insensitive)
        var specificMatches = _specificAccessRules
            .Where(x => userAgent.IndexOf(x.UserAgent, StringComparison.InvariantCultureIgnoreCase) >= 0)
            .ToList();

        // If no specific matches, fall back to global (“*”) rules
        var relevantRules = specificMatches.Count > 0
            ? specificMatches.Where(x =>
                string.IsNullOrEmpty(x.Path) ||
                IsPathMatch(path.Substring(1), x.Path.Substring(1)))
                .ToList()
            : _globalAccessRules.Where(x =>
                string.IsNullOrEmpty(x.Path) ||
                IsPathMatch(path.Substring(1), x.Path.Substring(1)))
                .ToList();

        if (relevantRules.Count == 0)
            return true;

        AccessRule chosenRule;

        if (AllowRuleImplementation != AllowRuleImplementation.MoreSpecific)
        {
            // Standard or AllowOverrides: pick the rule with lowest Order
            chosenRule = relevantRules
                .OrderBy(x => x.Order)
                .First();
        }
        else
        {
            // MoreSpecific: pick the rule with the longest Path (most specific), then by Order
            chosenRule = relevantRules
                .OrderByDescending(x => x.Path.Length)
                .ThenBy(x => x.Order)
                .First();
        }

        return AllowRuleImplementation switch
        {
            AllowRuleImplementation.Standard =>
                string.IsNullOrEmpty(chosenRule.Path) || chosenRule.Allowed,

            AllowRuleImplementation.AllowOverrides =>
                relevantRules.Any(x => x.Allowed) || string.IsNullOrEmpty(chosenRule.Path),

            AllowRuleImplementation.MoreSpecific =>
                string.IsNullOrEmpty(chosenRule.Path) || chosenRule.Allowed,

            _ => false
        };
    }

    /// <inheritdoc/>
    public TimeSpan CrawlDelay(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            throw new ArgumentException("Not a valid user-agent string.", nameof(userAgent));

        // If no rules or no crawl-delay rules, return zero
        if (!HasRules || _crawlDelayRules.Count == 0)
            return TimeSpan.Zero;

        // Global (“*”) crawl-delay rules
        var globalDelays = _crawlDelayRules
            .Where(x => x.UserAgent == "*")
            .ToList();

        // Specific crawl-delay rules matching this user-agent
        var specificDelays = _crawlDelayRules
            .Where(x => x.UserAgent.IndexOf(userAgent, StringComparison.InvariantCultureIgnoreCase) >= 0)
            .ToList();

        if (globalDelays.Count == 0 && specificDelays.Count == 0)
            return TimeSpan.Zero;

        // Prefer the first specific one if any; otherwise take the first global one
        var chosen = specificDelays.Count > 0
            ? specificDelays.First()
            : globalDelays.First();

        // Convert Delay (milliseconds) to TimeSpan
        return chosen.Delay;
    }


    public TimeSpan CrawlDelay(string userAgent, int fallbackAmount)
    {
        var delay = CrawlDelay(userAgent);

        if (delay == TimeSpan.Zero)
        {
            return TimeSpan.FromMilliseconds(fallbackAmount);
        }

        return delay;
    }

    public TimeSpan CrawlDelay(string userAgent, TimeSpan fallbackAmount)
    {
        var delay = CrawlDelay(userAgent);

        if (delay == TimeSpan.Zero)
        {
            return fallbackAmount;
        }

        return delay;
    }

    // Checks if 'path' matches 'rulePath' with wildcard (*) and end-of-line ($) support.
    private static bool IsPathMatch(string path, string rulePath)
    {
        var length = rulePath.Length;

        for (int i = 0; i < length; i++)
        {
            var ch = rulePath[i];

            if (ch == '$' && i == length - 1)
            {
                // '$' at the end means path must end exactly here
                return i == path.Length;
            }

            if (ch == '*')
            {
                // '*' matches any sequence; if it's at the end, everything matches
                if (i == length - 1)
                    return true;

                // Try to match the remainder of rulePath at all subsequent positions
                for (int start = i; start < path.Length; start++)
                {
                    if (IsPathMatch(path[start..], rulePath[(i + 1)..]))
                        return true;
                }

                return false;
            }

            if (i >= path.Length || ch != path[i])
                return false;
        }

        // If we exit the loop, rulePath is a prefix; path must start with it
        return path.StartsWith(rulePath, StringComparison.Ordinal);
    }

    // Normalizes a raw URL path: ensures it starts with '/', collapses '//' to '/', and defaults empty to '/'.
    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            path = "/";

        if (!path.StartsWith("/", StringComparison.Ordinal))
            path = "/" + path;

        while (path.Contains("//", StringComparison.Ordinal))
            path = path.Replace("//", "/", StringComparison.Ordinal);

        return path;
    }
}
