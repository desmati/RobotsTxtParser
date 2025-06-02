# RobotsTxtParser

[![NuGet](https://img.shields.io/nuget/v/RobotsTxtParser)](https://www.nuget.org/packages/RobotsTxtParser/2025.9.2)

A simple **.NET Core 9** library for parsing `robots.txt` files.  
Supports:
- Reading and parsing `User-agent`, `Allow`, `Disallow`, `Crawl-delay`, and `Sitemap` directives
- Checking whether a given URL path is allowed for a particular user-agent, with three different “Allow vs Disallow” strategies
- Retrieving crawl-delays (as `TimeSpan`) for specified user-agents
- Collecting all `Sitemap` URLs declared in a `robots.txt`

The parser is fully immutable after construction and exposes a clean, intuitive API.

---

## Table of Contents

1. [Installation](#installation)  
2. [Quick Start](#quick-start)  
3. [API Reference](#api-reference)  
   - [`Robots` class](#robots-class)  
   - [`IRobotsParser` interface](#irobotsparser-interface)  
   - [`AllowRuleImplementation` enum](#allowruleimplementation-enum)  
   - [`Sitemap` class](#sitemap-class)  
4. [Usage Examples](#usage-examples)  
   - [Basic “Allow/Disallow” check](#basic-allowdisallow-check)  
   - [Switching “Allow” rule strategy](#switching-allow-rule-strategy)  
   - [Crawl-delay retrieval](#crawl-delay-retrieval)  
   - [Extracting all Sitemap URLs](#extracting-all-sitemap-urls)  
   - [Handling malformed lines](#handling-malformed-lines)  
5. [Notes & Caveats](#notes--caveats)  
6. [License](#license)  
7. [Commercial Licensing](#commercial-licensing)  

---

## Installation

#### Via .NET CLI

```bash
dotnet add package RobotsTxtParser --version 2025.9.2
````

#### Via Package Manager (Visual Studio)

```powershell
Install-Package RobotsTxtParser -Version 2025.9.2
```

#### Direct Reference

If you prefer to reference the local project, copy the `RobotsTxtParser` folder into your solution and add the project as a reference. The library targets **net9.0** with C# 11 and has `<Nullable>enable</Nullable>` by default.

---

## Quick Start

```csharp
using RobotsTxtParser;

// Suppose you have robots.txt content as a string:
string robotsTxtContent = @"
User-agent: Googlebot
Disallow: /private
Allow: /public

User-agent: *
Disallow: /tmp
Crawl-delay: 1.5
Sitemap: https://example.com/sitemap.xml
";

// Parse it:
var robots = Robots.Load(robotsTxtContent);

// Check if "/public/page.html" is allowed for "Googlebot":
bool canGooglebotAccess = robots.IsPathAllowed("Googlebot", "/public/page.html");

// Check crawl-delay for a generic crawler:
TimeSpan defaultDelay = robots.CrawlDelay("SomeOtherBot");

// Retrieve all sitemap URLs:
foreach (var site in robots.Sitemaps)
{
    if (site.Url != null)
        Console.WriteLine($"Valid sitemap URL: {site.Url}");
    else
        Console.WriteLine($"Malformed sitemap entry: {site.Value}");
}
```

For more use-cases review the test units inside the `RobotsTxtParser.Tests` project.

---

## API Reference

### `Robots` class

```csharp
namespace RobotsTxtParser
{
    public class Robots : IRobotsParser
    {
        // Properties
        public string Raw { get; }                      // Original robots.txt content
        public List<Sitemap> Sitemaps { get; private set; }
        public bool Malformed { get; private set; }     // True if any line was malformed
        public bool HasRules { get; private set; }      // True if ≥1 Access or Crawl-delay rule parsed
        public bool IsAnyPathDisallowed { get; private set; }
        public AllowRuleImplementation AllowRuleImplementation { get; set; }

        // Static Factory
        public static Robots Load(string content);

        // IRobotsParser Implementation
        public bool IsPathAllowed(string userAgent, string path);
        public TimeSpan CrawlDelay(string userAgent);
    }
}
```

* **`Load(string content)`**
  Parses the entire `robots.txt` content and returns a `Robots` instance.
  If `content` is `null` or whitespace, no rules are parsed and `HasRules == false`.

* **`IsPathAllowed(string userAgent, string path) : bool`**
  Returns `true` if the given `path` is allowed for the specified `userAgent`, after normalizing `path`.
  Throws `ArgumentException` if `userAgent` is `null`/empty/whitespace.
  If there are no rules, or if no Disallow rules exist, always returns `true`.
  The logic respects the chosen `AllowRuleImplementation`.

* **`CrawlDelay(string userAgent) : TimeSpan`**
  Returns the crawl-delay (in milliseconds, as a `TimeSpan`) for the `userAgent`.
  Throws `ArgumentException` if `userAgent` is `null`/empty/whitespace.
  If no crawl-delay rule matches, returns `TimeSpan.Zero`.
  Tries specific rules first, then the global (`*`) rule.

* **`Raw`**
  The unmodified string passed into `Load(...)`.

* **`Sitemaps`**
  A list of `Sitemap` objects representing each `Sitemap:` directive.

* **`Malformed`**
  `true` if at least one line was out of expected context (e.g. `Disallow` before any `User-agent`, or an unrecognized directive). Parsed valid rules still apply.

* **`HasRules`**
  `true` if at least one `Allow`/`Disallow` or `Crawl-delay` directive was successfully recorded under some `User-agent`.

* **`IsAnyPathDisallowed`**
  `true` if there is at least one `Disallow` with a non-empty path (meaning not “Disallow: ”).

* **`AllowRuleImplementation`**
  Determines how to resolve conflicts when multiple `Allow`/`Disallow` rules match a path. Default is `MoreSpecific`.

---

### `IRobotsParser` interface

```csharp
namespace RobotsTxtParser
{
    public interface IRobotsParser
    {
        bool IsPathAllowed(string userAgent, string path);
        TimeSpan CrawlDelay(string userAgent);
    }
}
```

---

### `AllowRuleImplementation` enum

```csharp
namespace RobotsTxtParser
{
    public enum AllowRuleImplementation
    {
        Standard,       // Pick the matched rule with lowest “order” (first-seen)
        AllowOverrides, // If any matching rule is Allow, path is allowed
        MoreSpecific    // Pick the rule with longest Path, then by order
    }
}
```

---

### `Sitemap` class

```csharp
namespace RobotsTxtParser
{
    public class Sitemap
    {
        public string Value { get; }   // Raw text after “Sitemap:” (never null)
        public Uri? Url { get; }       // Parsed absolute Uri, or null if invalid

        internal static Sitemap FromLine(Line line);
    }
}
```

* Use `robots.Sitemaps` after calling `Robots.Load(...)`; each item has:

  * `Value` – the exact substring from `robots.txt` after “Sitemap:”
  * `Url` – a `Uri` if `Value` is a well-formed absolute URL; otherwise `null`.

---

## Usage Examples

### Basic “Allow/Disallow” check

```csharp
string robotsTxt = @"
User-agent: *
Disallow: /private
Allow: /public
";

var robots = Robots.Load(robotsTxt);

// Default is MoreSpecific
Console.WriteLine(robots.IsPathAllowed("anybot", "/public/index.html"));  // True
Console.WriteLine(robots.IsPathAllowed("anybot", "/private/data.txt"));   // False
```

### Switching “Allow” rule strategy

```csharp
string robotsTxt = @"
User-agent: *
Disallow: /foo
Allow: /foo
";

var r = Robots.Load(robotsTxt);

// Standard: pick first-seen → Disallow
r.AllowRuleImplementation = AllowRuleImplementation.Standard;
Console.WriteLine(r.IsPathAllowed("Bot", "/foo")); // False

// AllowOverrides: any Allow wins → allowed
r.AllowRuleImplementation = AllowRuleImplementation.AllowOverrides;
Console.WriteLine(r.IsPathAllowed("Bot", "/foo")); // True

// MoreSpecific: tie-break by order (since both are "/foo")
r.AllowRuleImplementation = AllowRuleImplementation.MoreSpecific;
Console.WriteLine(r.IsPathAllowed("Bot", "/foo")); // False  (Disallow first)
```

### Crawl-delay retrieval

```csharp
string robotsTxt = @"
User-agent: MyBot
Crawl-delay: 4.25

User-agent: *
Crawl-delay: 2
";

var robots = Robots.Load(robotsTxt);

// “MyBot” → 4250 ms
TimeSpan myDelay = robots.CrawlDelay("MyBot");
Console.WriteLine(myDelay.TotalMilliseconds); // 4250

// Other bots → 2000 ms
TimeSpan otherDelay = robots.CrawlDelay("OtherBot");
Console.WriteLine(otherDelay.TotalMilliseconds); // 2000

// If no matching rule (and no global "*"), returns TimeSpan.Zero
var empty = Robots.Load(@"User-agent: BotOnly");
Console.WriteLine(empty.CrawlDelay("BotOnly") == TimeSpan.Zero); // True
```

### Extracting all Sitemap URLs

```csharp
string robotsTxt = @"
User-agent: *
Sitemap: https://example.com/sitemap1.xml
Sitemap: not_a_real_url
Sitemap: https://cdn.example.com/other-sitemap.xml
";

var robots = Robots.Load(robotsTxt);

foreach (var site in robots.Sitemaps)
{
    Console.WriteLine($"Raw value: '{site.Value}'");
    if (site.Url != null)
        Console.WriteLine($" Parsed URI: {site.Url}");
    else
        Console.WriteLine(" (Invalid URI)");
}

// Output:
// Raw value: 'https://example.com/sitemap1.xml'
// Parsed URI: https://example.com/sitemap1.xml
// Raw value: 'not_a_real_url'
// (Invalid URI)
// Raw value: 'https://cdn.example.com/other-sitemap.xml'
// Parsed URI: https://cdn.example.com/other-sitemap.xml
```

### Handling malformed lines

```csharp
string content = @"
Disallow: /private          # no preceding User-agent → malformed
User-agent: *
Allow: /public
FooBar: /ignored            # unknown field → malformed
";

var robots = Robots.Load(content);

Console.WriteLine($"Malformed? {robots.Malformed}");  // True
Console.WriteLine($"HasRules? {robots.HasRules}");    // True (because “Allow” under valid UA)
Console.WriteLine(robots.IsPathAllowed("any", "/private")); // True (early Disallow ignored)
Console.WriteLine(robots.IsPathAllowed("any", "/public"));  // True
```

---

## Notes & Caveats

1. **Normalization of `path`**

   * `IsPathAllowed(...)` calls `NormalizePath(path)`:

     * Converts `null` or whitespace to `"/"`.
     * Ensures a leading `/`.
     * Collapses repeated `//` into a single `/`.
   * Matching logic strips the leading `/` before comparing to rule paths.

2. **Wildcard & `$` support**

   * `*` in a rule path matches any sequence of characters.
   * A trailing `$` means “end-of-string” match.
   * Internally, `IsPathMatch(pathWithoutSlash, rulePathWithoutSlash)` implements these recursively.

3. **Case-insensitive matching**

   * Directive names (`User-agent`, `Allow`, `Disallow`, etc.) are matched case-insensitively.
   * User-agent value matching (in `AccessRule` and `CrawlDelayRule`) is also case-insensitive substring match.

4. **Malformed lines**

   * A line is marked malformed if it appears out of context (e.g. `Disallow` before any `User-agent`) or if the field name is unrecognized.
   * Malformed lines set `robots.Malformed = true`, but valid rules under valid `User-agent` still apply.

5. **Global (`*`) rules**

   * A rule’s `UserAgent` of `"*"` is stored in global lists.
   * If no specific rule matches a given user-agent, the parser falls back to the global rule.
   * If multiple global rules exist, the first one (lowest `Order`) is used unless `MoreSpecific` is in effect.

---

## License

**RobotsTxtParser** is licensed under the **GNU Affero General Public License, version 3 (AGPL-3.0-or-later)**.
See [LICENSE](./LICENSE) for full text.

---

## Commercial Licensing

While RobotsTxtParser is available under the AGPL-3.0 for all free/open-source usage, a separate **commercial license** is required to incorporate this code into proprietary or closed-source products without adhering to AGPL’s copyleft obligations.

To purchase a commercial license, please contact:

```
Hossein Esmati  
Email: desmati@gmail.com
```

The commercial license will be provided under mutually agreed terms, which supersede AGPL-3.0 for your proprietary usage.

---

```
