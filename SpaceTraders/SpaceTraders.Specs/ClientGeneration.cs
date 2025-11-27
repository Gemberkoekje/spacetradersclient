using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpaceTraders.Specs;

/// <summary>
/// Provides utility routines for generating the SpaceTraders typed HTTP client from the live OpenAPI/Swagger document.
/// </summary>
/// <remarks>
/// The generation process fetches the remote OpenAPI JSON, applies schema and enum description patches, and
/// writes (or updates) the generated C# client file inside the <c>SpaceTraders.Client/Generated</c> directory.
/// </remarks>
public sealed class ClientGeneration
{
    /// <summary>
    /// Fetches the SpaceTraders OpenAPI specification, applies name and enum description fixes, and writes
    /// the regenerated NSwag client into the <c>SpaceTraders.Client</c> project if content changed.
    /// </summary>
    /// <remarks>
    /// This is an explicit test acting as an on-demand code generation task. It has a side effect of writing a file
    /// (<c>SpaceTradersClient.cs</c>) to disk. The test asserts successful generation by checking for the client class name.
    /// </remarks>
    /// <exception cref="HttpRequestException">Thrown when the remote OpenAPI document cannot be retrieved.</exception>
    /// <exception cref="IOException">Thrown if reading or writing the target client file fails.</exception>
    /// <exception cref="JsonReaderException">Thrown if the fetched JSON cannot be parsed.</exception>
    [Test]
    [Explicit("Generates and writes the NSwag client into SpaceTraders.Client project with fixed schema names.")]
    public async Task GeneratesClientAndSavesToProject()
    {
        using var http = new HttpClient();

        var rawJson = await http.GetStringAsync("https://api.spacetraders.io/v2/documentation/json");
        rawJson = FixConflictingNames(rawJson);          // Rename schemas that collide with 'System'
        rawJson = PatchEnumDescriptions(rawJson);        // Fix x-enumDescriptions map vs array
        var document = await OpenApiDocument.FromJsonAsync(rawJson);

        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "SpaceTradersClient",
            CSharpGeneratorSettings =
            {
                Namespace = "SpaceTraders.Client",
            },
        };

        var generator = new CSharpClientGenerator(document, settings);
        var code = generator.GenerateFile();

        // Post-process: ensure JSON POSTs without a body send an empty object `{}` instead of an empty string.
        code = FixEmptyJsonBodiesInGeneratedCode(code);
        code = FixTraitsInGeneratedCode(code);

        var solutionRoot = FindSolutionRoot();
        Assert.That(solutionRoot, Is.Not.Null, "Solution root could not be located.");

        var targetDir = Path.Combine(solutionRoot!, "SpaceTraders.Client", "Generated");
        Directory.CreateDirectory(targetDir);
        var targetFile = Path.Combine(targetDir, "SpaceTradersClient.cs");

        if (!File.Exists(targetFile) || await File.ReadAllTextAsync(targetFile) != code)
        {
            await File.WriteAllTextAsync(targetFile, code);
        }

        await TestContext.Out.WriteLineAsync($"Generated client written to: {targetFile}");
        Assert.That(code, Does.Contain("class SpaceTradersClient"));
    }

    /// <summary>
    /// Renames the schema <c>System</c> to <c>StarSystem</c> (and updates all <c>$ref</c> values) to avoid
    /// collision with the NSwag-emitted alias <c>using System = global::System;</c>.
    /// </summary>
    /// <param name="json">The raw OpenAPI JSON document text.</param>
    /// <returns>The modified JSON string if the schema existed; otherwise the original input.</returns>
    /// <exception cref="JsonReaderException">Thrown if <paramref name="json"/> is not valid JSON.</exception>
    private static string FixConflictingNames(string json)
    {
        var root = JObject.Parse(json);

        var schemas = (JObject?)root.SelectToken("components.schemas");
        if (schemas == null || !schemas.TryGetValue("System", out var systemSchema))
            return json;

        // Add new key then remove old
        schemas["StarSystem"] = systemSchema;
        schemas.Remove("System");

        // Update all $ref occurrences
        foreach (var token in root.Descendants().OfType<JValue>().Where(v => v.Type == JTokenType.String))
        {
            if (string.Equals((string?)token.Value, "#/components/schemas/System", StringComparison.Ordinal))
            {
                token.Value = "#/components/schemas/StarSystem";
            }
        }

        return root.ToString();
    }

    /// <summary>
    /// Converts <c>x-enumDescriptions</c> objects (maps keyed by enum value) to arrays whose order matches the associated <c>enum</c> array.
    /// </summary>
    /// <param name="json">The raw OpenAPI JSON document text.</param>
    /// <returns>The modified JSON string with normalized enum descriptions; original if no changes applied.</returns>
    /// <remarks>
    /// NSwag expects <c>x-enumDescriptions</c> as an array aligned positionally with the <c>enum</c> definition.
    /// Some upstream specs provide a dictionary instead; this routine reconciles that format.
    /// </remarks>
    /// <exception cref="JsonReaderException">Thrown if <paramref name="json"/> is not valid JSON.</exception>
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
    private static string PatchEnumDescriptions(string json)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
    {
        var root = JObject.Parse(json);
        var schemas = (JObject?)root.SelectToken("components.schemas");
        if (schemas == null)
            return json;

        foreach (var schemaProp in schemas.Properties())
        {
            if (schemaProp.Value is not JObject schemaObj)
                continue;

            var enumToken = schemaObj["enum"] as JArray;
            var enumDescriptionsToken = schemaObj["x-enumDescriptions"];

            if (enumToken != null && enumDescriptionsToken is JObject dict)
            {
                var arr = new JArray();
                foreach (var enumValue in enumToken)
                {
                    var key = enumValue.Type == JTokenType.String
                        ? enumValue.Value<string>()
                        : enumValue.ToString();

                    var desc = dict.TryGetValue(key!, out var descToken)
                        ? descToken?.ToString()
                        : key;

                    arr.Add(desc);
                }
                schemaObj["x-enumDescriptions"] = arr;
            }
        }

        return root.ToString();
    }

    /// <summary>
    /// Traverses parent directories from the current AppContext base path attempting to locate the solution root.
    /// </summary>
    /// <returns>The absolute path containing a <c>.slnx</c> file if found; otherwise <c>null</c>.</returns>
    private static string? FindSolutionRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(dir))
        {
            // Using .slnx as per current solution format
            if (Directory.GetFiles(dir, "*.slnx").Length > 0)
                return dir;
            dir = Directory.GetParent(dir)?.FullName;
        }
        return null;
    }

    /// <summary>
    /// NSwag emits an empty string content for POSTs with no request body. The SpaceTraders API requires an
    /// empty JSON object ({}). This normalizes the generated code accordingly.
    /// </summary>
    /// <param name="code">The generated C# client source code.</param>
    /// <returns>Patched code which sends '{}' instead of an empty string for JSON requests without a body.</returns>
    private static string FixEmptyJsonBodiesInGeneratedCode(string code)
    {
        // Most specific: constructor that already declares application/json
        code = code.Replace(
            "new StringContent(string.Empty, System.Text.Encoding.UTF8, \"application/json\")",
            "new StringContent(\"{}\", System.Text.Encoding.UTF8, \"application/json\")");

        code = code.Replace(
            "new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, \"application/json\")",
            "new System.Net.Http.StringContent(\"{}\", System.Text.Encoding.UTF8, \"application/json\")");

        // Fallback: plain empty content (content type typically set on the next line)
        code = code.Replace(
            "new StringContent(string.Empty)",
            "new StringContent(\"{}\")");

        code = code.Replace(
            "new System.Net.Http.StringContent(string.Empty)",
            "new System.Net.Http.StringContent(\"{}\")");

        return code;
    }

    private static string FixTraitsInGeneratedCode(string code)
    {
        // Fix generated code that uses 'new Client.Traits()' instead of 'new Traits()'
        code = code.Replace(
            "Traits traits",
            "WaypointTraitSymbol? traits");
        return code;
    }
}
