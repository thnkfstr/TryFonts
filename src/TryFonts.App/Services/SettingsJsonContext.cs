using System.Text.Json.Serialization;
using TryFonts.Core.Models;

namespace TryFonts.App.Services;

/// <summary>
/// Source-generated System.Text.Json context for <see cref="AppSettings"/>.
///
/// Why source-gen instead of reflection serialization:
/// 1. Trim safety — the published EXE is trimmed (PublishTrimmed); reflection-based
///    serialization emits IL2026 warnings and depends on metadata the trimmer
///    cannot statically verify. Source-gen is fully analyzable.
/// 2. NaN handling — WindowX/WindowY default to double.NaN ("let the OS decide").
///    Default JSON serialization throws on NaN, and Save() swallows the exception,
///    silently losing settings. AllowNamedFloatingPointLiterals writes "NaN" and
///    reads it back.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    UseStringEnumConverter = true,
    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals)]
[JsonSerializable(typeof(AppSettings))]
internal sealed partial class SettingsJsonContext : JsonSerializerContext;
