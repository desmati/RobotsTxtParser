namespace RobotsTxtParser;

/// <summary>
/// Specifies the strategy for handling “Allow” rules in a robots.txt parser.
/// </summary>
public enum AllowRuleImplementation
{
    /// <summary>
    /// Use the standard interpretation: “Allow” rules have no special precedence over “Disallow” rules.
    /// </summary>
    Standard,

    /// <summary>
    /// Interpret “Allow” rules as overriding “Disallow” rules when they match the same path.
    /// </summary>
    AllowOverrides,

    /// <summary>
    /// Choose the rule (Allow or Disallow) whose path is more specific (longer) when both match.
    /// </summary>
    MoreSpecific,
}
