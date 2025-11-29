using SpaceTraders.Core.Enums;
using System;
using System.Text;

namespace SpaceTraders.Core.Extensions;

/// <summary>
/// Extension conversion helpers for mapping generated client enum values to internal domain enums.
/// </summary>
/// <remarks>
/// The generated client (namespace <c>Client</c>) uses uppercase snake-case enum members. These helpers translate them
/// into the idiomatic C# enum declarations used in the core library. An <see cref="ArgumentOutOfRangeException"/> is thrown
/// when an unexpected enum value is encountered, making additions explicit during upgrades.
/// </remarks>
public static class EnumExtensions
{
    public static TOut Convert<TIn, TOut>(this TIn symbol) where TIn : Enum where TOut : struct
    {
        var name = symbol.ToString();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentOutOfRangeException(nameof(symbol), $"Not expected {typeof(TIn).Name} value: {symbol}");

        // Split into tokens by underscore
        var tokens = name.Split('_', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(symbol), $"Not expected {typeof(TIn).Name} value: {symbol}");

        // Determine if the first token is a redundant enum-type prefix (e.g., FRAME_FRIGATE, ENGINE_ION_DRIVE_II)
        // We strip common suffixes from the target enum type (Symbol/Type/Status/Mode/Role) and compare.
        var targetRoot = GetTypeRoot(typeof(TOut).Name);
        int startIndex = 0;
        if (tokens.Length > 1 && string.Equals(tokens[0], targetRoot, StringComparison.OrdinalIgnoreCase))
        {
            startIndex = 1; // drop the enum-type prefix
        }

        // Build PascalCase while preserving Roman numeral tokens (e.g., II, III, IV)
        var sb = new StringBuilder(name.Length);
        for (int i = startIndex; i < tokens.Length; i++)
        {
            var token = tokens[i];
            if (IsRomanNumeral(token))
            {
                sb.Append(token); // preserve capitalization
                continue;
            }

            if (token.Length == 0)
                continue;

            sb.Append(char.ToUpperInvariant(token[0]));
            if (token.Length > 1)
            {
                for (int j = 1; j < token.Length; j++)
                {
                    sb.Append(char.ToLowerInvariant(token[j]));
                }
            }
        }

        var pascalName = sb.ToString();
        if (Enum.TryParse<TOut>(pascalName, out var result))
        {
            return result;
        }

        throw new ArgumentOutOfRangeException(nameof(symbol), $"Not expected {typeof(TIn).Name}?{typeof(TOut).Name} value mapping: {symbol} ? {pascalName}");
    }

    private static string GetTypeRoot(string typeName)
    {
        // Strip common suffixes to get the root name used as a prefix in generated enums
        ReadOnlySpan<string> suffixes = ["Symbol", "Type", "Status", "Mode", "Role"];
        foreach (var suffix in suffixes)
        {
            if (typeName.EndsWith(suffix, StringComparison.Ordinal))
            {
                typeName = typeName[..^suffix.Length];
                break;
            }
        }
        return typeName.ToUpperInvariant();
    }

    private static bool IsRomanNumeral(string token)
    {
        if (token.Length == 0)
            return false;

        for (int i = 0; i < token.Length; i++)
        {
            var c = token[i];
            // Accept standard Roman numeral letters
            if (c is not ('I' or 'V' or 'X' or 'L' or 'C' or 'D' or 'M'))
                return false;
        }
        return true;
    }
}
