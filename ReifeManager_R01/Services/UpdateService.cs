using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using ReifeManager_R01.Models;

namespace ReifeManager_R01.Services;

public class UpdateService
{
    private const string Owner = "Acid31-31";
    private const string Repo = "ReifeschrankTracker";

    private static readonly string LatestReleaseApiUrl = $"https://api.github.com/repos/{Owner}/{Repo}/releases/latest";
    private static readonly string TagsApiUrl = $"https://api.github.com/repos/{Owner}/{Repo}/tags?per_page=20";

    public const string ReleasePageUrl = $"https://github.com/{Owner}/{Repo}/releases";

    public async Task<UpdateInfo?> PruefeAufUpdateAsync()
    {
        try
        {
            using var client = ErzeugeClient();

            using var releaseResponse = await client.GetAsync(LatestReleaseApiUrl);
            if (releaseResponse.IsSuccessStatusCode)
            {
                var releaseJson = await releaseResponse.Content.ReadAsStringAsync();
                var releaseUpdate = ParseReleaseUpdate(releaseJson);
                if (releaseUpdate is not null)
                {
                    return releaseUpdate;
                }
            }
            else if (releaseResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                releaseResponse.EnsureSuccessStatusCode();
            }

            return await PruefeTagInstallerFallbackAsync(client);
        }
        catch (Exception ex)
        {
            // Fehler protokollieren aber nicht crashen
            System.Diagnostics.Debug.WriteLine($"Update-Prüfung fehlgeschlagen: {ex.Message}");
            return null;
        }
    }

    public async Task<string> LadeUpdateHerunterAsync(UpdateInfo info)
    {
        var updateOrdner = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ReifeManager",
            "Updates");

        Directory.CreateDirectory(updateOrdner);

        var dateiname = string.IsNullOrWhiteSpace(info.AssetName) ? "ReifeManager_Update.exe" : info.AssetName;
        var zielPfad = Path.Combine(updateOrdner, dateiname);

        using var client = ErzeugeClient();
        var bytes = await client.GetByteArrayAsync(info.DownloadUrl);
        await File.WriteAllBytesAsync(zielPfad, bytes);

        return zielPfad;
    }

    public void StarteInstaller(string installerPfad)
    {
        Process.Start(new ProcessStartInfo(installerPfad)
        {
            UseShellExecute = true,
            Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-"
        });
    }

    private async Task<UpdateInfo?> PruefeTagInstallerFallbackAsync(HttpClient client)
    {
        try
        {
            using var tagsResponse = await client.GetAsync(TagsApiUrl);
            tagsResponse.EnsureSuccessStatusCode();

            var tagsJson = await tagsResponse.Content.ReadAsStringAsync();
            using var tagsDoc = JsonDocument.Parse(tagsJson);

            if (tagsDoc.RootElement.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            string? tagName = null;
            Version? latest = null;

            foreach (var tag in tagsDoc.RootElement.EnumerateArray())
            {
                var name = tag.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var parsed = ParseVersion(name);
                if (parsed <= new Version(1, 0, 0))
                {
                    continue;
                }

                tagName = name;
                latest = parsed;
                break;
            }

            if (latest is null || string.IsNullOrWhiteSpace(tagName))
            {
                return null;
            }

            var current = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
            if (latest <= current)
            {
                return null;
            }

            var cleanTag = tagName.Trim().TrimStart('v', 'V');
            var kandidaten = new[]
            {
                $"https://raw.githubusercontent.com/{Owner}/{Repo}/main/installer/ReifeManager_Setup_v{cleanTag}.exe",
                $"https://raw.githubusercontent.com/{Owner}/{Repo}/main/installer/ReifeManager_Setup_{cleanTag}.exe",
                $"https://raw.githubusercontent.com/{Owner}/{Repo}/main/installer/ReifeManager_Setup.exe"
            };

            foreach (var url in kandidaten)
            {
                try
                {
                    using var head = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                    if (head.IsSuccessStatusCode)
                    {
                        return new UpdateInfo
                        {
                            Version = latest.ToString(),
                            DownloadUrl = url,
                            ReleaseUrl = ReleasePageUrl,
                            AssetName = Path.GetFileName(new Uri(url).AbsolutePath)
                        };
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Fallback Tag-Prüfung fehlgeschlagen: {ex.Message}");
            return null;
        }
    }

    private static UpdateInfo? ParseReleaseUpdate(string releaseJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(releaseJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty("tag_name", out var tagEl))
            {
                return null;
            }

            var tag = tagEl.GetString() ?? string.Empty;
            var latest = ParseVersion(tag);
            var current = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

            if (latest <= current)
            {
                return null;
            }

            var releaseUrl = root.TryGetProperty("html_url", out var htmlEl)
                ? htmlEl.GetString() ?? ReleasePageUrl
                : ReleasePageUrl;

            if (!root.TryGetProperty("assets", out var assets) || assets.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            foreach (var asset in assets.EnumerateArray())
            {
                var name = asset.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null;
                var url = asset.TryGetProperty("browser_download_url", out var urlEl) ? urlEl.GetString() : null;

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url))
                {
                    continue;
                }

                if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    return new UpdateInfo
                    {
                        Version = latest.ToString(),
                        DownloadUrl = url,
                        ReleaseUrl = releaseUrl,
                        AssetName = name
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Release-JSON-Parsing fehlgeschlagen: {ex.Message}");
            return null;
        }
    }

    private static HttpClient ErzeugeClient()
    {
        var client = new HttpClient();
        // GitHub API braucht einen aussagekräftigen User-Agent
        client.DefaultRequestHeaders.UserAgent.ParseAdd("ReifeManager/1.0 (+https://github.com/Acid31-31/ReifeschrankTracker)");
        
        // Timeout setzen um Rate Limiting zu vermeiden
        client.Timeout = TimeSpan.FromSeconds(10);

        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    private static Version ParseVersion(string tag)
    {
        var clean = tag.Trim().TrimStart('v', 'V');
        return Version.TryParse(clean, out var version)
            ? version
            : new Version(1, 0, 0);
    }
}
