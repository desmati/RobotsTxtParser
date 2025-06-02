using System;

namespace RobotsTxtParser;

/// <summary>
/// Represents a single line from a robots.txt file, including its raw text,
/// determined type, and parsed field/value if applicable.
/// </summary>
internal class Line
{
    /// <summary>
    /// Gets the determined <see cref="LineType"/> of this line.
    /// </summary>
    public LineType Type { get; }

    /// <summary>
    /// Gets the original, unmodified line text (non-null, non-empty).
    /// </summary>
    public string Raw { get; }

    /// <summary>
    /// Gets the field portion (text before “:”) if this line is a directive; otherwise <c>null</c>.
    /// </summary>
    public string? Field { get; }

    /// <summary>
    /// Gets the value portion (text after “:”) if this line is a directive; otherwise <c>null</c>.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Line"/> by parsing the provided <paramref name="lineText"/>.
    /// </summary>
    /// <param name="lineText">The raw line text from robots.txt (non-null, non-whitespace).</param>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="lineText"/> is <c>null</c> or consists only of whitespace.
    /// </exception>
    public Line(string lineText)
    {
        if (string.IsNullOrWhiteSpace(lineText))
            throw new ArgumentException("Cannot create a Line from null or whitespace.", nameof(lineText));

        Raw = lineText;
        var trimmed = lineText.Trim();

        // Comment: starts with '#' (after trimming).
        if (trimmed.StartsWith("#", StringComparison.Ordinal))
        {
            Type = LineType.Comment;
            return;
        }

        // If there's an inline comment, strip it off.
        var commentIndex = trimmed.IndexOf('#');
        if (commentIndex >= 0)
            trimmed = trimmed.Substring(0, commentIndex).TrimEnd();

        // Attempt to find the field (text before the first ':').
        var fieldText = GetField(trimmed);
        if (string.IsNullOrWhiteSpace(fieldText))
        {
            Type = LineType.Unknown;
            return;
        }

        Field = fieldText.Trim();
        Type = EnumHelper.GetLineType(Field);

        // Extract value: text after the colon (":").
        // Since GetField found a colon at index = fieldText.Length,
        // we can take substring from (fieldText.Length + 1) onward.
        var separatorIndex = fieldText.Length;
        if (separatorIndex + 1 < trimmed.Length)
            Value = trimmed.Substring(separatorIndex + 1).Trim();
        else
            Value = string.Empty;
    }

    /// <summary>
    /// Retrieves the field portion (text before the first ':') from a trimmed line.
    /// Returns <c>null</c> or empty if no colon is present.
    /// </summary>
    /// <param name="trimmedLine">The line text after trimming and removing inline comments (non-null).</param>
    /// <returns>
    /// The substring before the first ':' if found; otherwise <c>string.Empty</c>.
    /// </returns>
    private static string GetField(string trimmedLine)
    {
        var colonIndex = trimmedLine.IndexOf(':');
        return colonIndex < 0
            ? string.Empty
            : trimmedLine.Substring(0, colonIndex);
    }
}
