/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.VestaMC.Modded.Models;
using HtmlAgilityPack;
using Serilog;

namespace Chase.VestaMC.Modded.Controllers;

public static class FabricVersionController
{
    public static readonly Uri FabricMavenUrl = new("https://maven.fabricmc.net/net/fabricmc/fabric-loader/");

    public static async Task<LoaderVersion> GetFabricLoaderByID(string id) => (await GetFabricLoaderVersions()).First(i => i.Version == id);

    public static async Task<LoaderVersion[]> GetFabricLoaderVersions()
    {
        List<LoaderVersion> versions = new();

        // The fabric maven url

        using HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync(FabricMavenUrl);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            HtmlDocument document = new()
            {
                OptionFixNestedTags = true
            };
            document.LoadHtml(content);
            if (document != null)
            {
                HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//a");
                if (nodes != null)
                {
                    foreach (HtmlNode node in nodes)
                    {
                        if (node.InnerText != "../" && !node.InnerText.StartsWith("maven"))
                        {
                            string version = node.InnerText.Trim('/');
                            string[] versionParts = version.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                            string extra = version.Contains('+') ? "+" + version.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Last() : "";
                            try
                            {
                                int Major = int.Parse(versionParts[0]);
                                int Minor = int.Parse(versionParts[1]);
                                int Patch = int.Parse(versionParts[2].Replace("+build", ""));
                                if (versionParts.Length > 3 && string.IsNullOrWhiteSpace(extra))
                                {
                                    extra = "." + versionParts[3].Trim();
                                }
                                LoaderVersion loaderVersion = new LoaderVersion()
                                {
                                    Major = Major,
                                    Minor = Minor,
                                    Patch = Patch,
                                    Build = extra,
                                    MinecraftVersion = null
                                };
                                loaderVersion.DownloadUri = new($"{FabricVersionController.FabricMavenUrl}{loaderVersion.Version}/fabric-loader-{loaderVersion.Version}.jar");
                                versions.Add(loaderVersion);
                            }
                            catch (FormatException e)
                            {
                                Log.Error("Unable to parse version {VERSION}", version, e);
                            }
                        }
                    }
                }
            }
        }
        return versions.OrderByDescending(i => i.Major).ThenByDescending(i => i.Minor).ThenByDescending(i => i.Patch).ToArray();
    }
}