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

public static class ForgeVersionController
{
    public static Uri ForgeUniversalJarUri(string minecraftVersion, string fmlVersion) =>
        new($"https://maven.minecraftforge.net/net/minecraftforge/forge/{minecraftVersion}-{fmlVersion}/forge-{minecraftVersion}-{fmlVersion}-universal.jar");

    public static async Task<LoaderVersion> GetForgeLoaderVersionByID(string minecraftVersion, string fmlVersion) => (await GetForgeLoaderVersions(minecraftVersion)).First(i => i.Version == fmlVersion);

    public static async Task<LoaderVersion[]> GetForgeLoaderVersions(string minecraftVersion)
    {
        List<LoaderVersion> versions = new();
        using HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync($"https://files.minecraftforge.net/net/minecraftforge/forge/index_{minecraftVersion}.html");
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
                HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//td[@class='download-version']");
                if (nodes != null)
                {
                    foreach (HtmlNode node in nodes)
                    {
                        string version = node.InnerText.Trim();
                        string[] versionParts = version.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            int Major = int.Parse(versionParts[0]);
                            int Minor = int.Parse(versionParts[1]);
                            int Patch = int.Parse(versionParts[2]);
                            LoaderVersion loaderVersion = new()
                            {
                                Major = Major,
                                Minor = Minor,
                                Patch = Patch,
                                Build = "",
                                MinecraftVersion = minecraftVersion
                            };
                            loaderVersion.DownloadUri = ForgeUniversalJarUri(minecraftVersion, loaderVersion.Version);
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
        return versions.OrderByDescending(i => i.Major).ThenByDescending(i => i.Minor).ThenByDescending(i => i.Patch).ToArray();
    }
}