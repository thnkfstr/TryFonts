namespace TryFonts.Core.Models;

/// <summary>How a search term is matched against font family names.</summary>
public enum SearchMode
{
    /// <summary>Match anywhere in the family name (case-insensitive).</summary>
    Contains = 0,

    /// <summary>Match only at the start of the family name (case-insensitive).</summary>
    StartsWith = 1,
}
