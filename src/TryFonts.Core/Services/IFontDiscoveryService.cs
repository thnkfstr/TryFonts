using TryFonts.Core.Models;

namespace TryFonts.Core.Services;

/// <summary>
/// Enumerates installed font families on the current platform.
/// Implementations must be isolated enough that they can be mocked for testing.
/// </summary>
public interface IFontDiscoveryService
{
    /// <summary>
    /// Discovers all available font families asynchronously.
    /// The returned list is deduplicated, sorted deterministically, and safe to enumerate
    /// from any thread after the task completes.
    /// </summary>
    Task<IReadOnlyList<FontFamilyInfo>> DiscoverAsync(CancellationToken cancellationToken = default);
}
