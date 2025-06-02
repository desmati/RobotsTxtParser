// File: Rule.cs
using System;

namespace RobotsTxtParser;

internal abstract class Rule
{
    /// <summary>
    /// Gets the user agent (e.g. crawler or client identifier) this rule applies to.
    /// </summary>
    public string UserAgent { get; }

    /// <summary>
    /// Gets the execution order of the rule.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Rule"/>.
    /// </summary>
    /// <param name="userAgent">The user-agent string this rule targets (non-null, non-empty).</param>
    /// <param name="order">The order in which this rule should run.</param>
    protected Rule(string userAgent, int order)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            throw new ArgumentException("UserAgent cannot be null or empty.", nameof(userAgent));

        UserAgent = userAgent;
        Order = order;
    }
}
