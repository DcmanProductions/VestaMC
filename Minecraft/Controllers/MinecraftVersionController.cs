/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Networking;
using Chase.Networking.Event;
using Chase.VestaMC.Minecraft.Models;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net.NetworkInformation;

namespace Chase.VestaMC.Minecraft.Controllers;

public static class MinecraftVersionController
{
    public static async Task<MinecraftVersionListModel> GetMinecraftVersions()
    {
        Uri manifestUri = new("https://launchermeta.mojang.com/mc/game/version_manifest.json");
        using HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync(manifestUri);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            MinecraftVersionListModel? manifestList = JObject.Parse(content)?.ToObject<MinecraftVersionListModel>();
            if (manifestList != null)
            {
                return manifestList.Value;
            }
        }
        Log.Error("Unable to retrieve Minecraft version manifest from Mojang!");
        throw new NetworkInformationException((int)response.StatusCode);
    }

    public static async Task<MinecraftVersionListModel> GetMinecraftReleaseVersions()
    {
        MinecraftVersionListModel versionListModel = await GetMinecraftVersions();
        return new()
        {
            Latest = new()
            {
                Snapshot = "",
                Release = versionListModel.Latest.Release
            },
            Versions = versionListModel.Versions.Where(i => i.Type == "release").ToArray()
        };
    }

    public static async Task<MinecraftVersionListModel> GetMinecraftSnapshotVersions()
    {
        MinecraftVersionListModel versionListModel = await GetMinecraftVersions();
        return new()
        {
            Latest = new()
            {
                Snapshot = versionListModel.Latest.Snapshot,
                Release = ""
            },
            Versions = versionListModel.Versions.Where(i => i.Type == "snapshot").ToArray()
        };
    }

    public static async Task<MinecraftVersionModel> GetMinecraftVersionByID(string id) => (await GetMinecraftVersions()).Versions.First(i => i.ID == id);

    public static async Task DownloadMinecraftServerJar(MinecraftVersionModel version, string outputDirectory, DownloadProgressEvent progressEvent)
    {
        using NetworkClient client = new();
        Uri downloadUri = await GetServerDownloadUri(version);
        await client.DownloadFileAsync(downloadUri, Path.Combine(outputDirectory, "server.jar"), progressEvent);
    }

    public static async Task<Uri> GetServerDownloadUri(MinecraftVersionModel version)
    {
        using HttpClient client = new();
        var response = await client.GetAsync(version.Url);
        if (response.IsSuccessStatusCode)
        {
            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (json != null)
            {
                if (json.ContainsKey("downloads"))
                {
                    JObject? downloads = json.GetValue("downloads")?.ToObject<JObject>();
                    if (downloads != null && downloads.ContainsKey("server"))
                    {
                        JObject? servers = downloads.GetValue("server")?.ToObject<JObject>();
                        if (servers != null && servers.ContainsKey("url"))
                        {
                            Uri? url = servers.GetValue("url")?.ToObject<Uri>();
                            if (url != null)
                            {
                                return url;
                            }
                        }
                    }
                }
            }
        }
        Log.Error("Unable to retrieve Minecraft version manifest for '{VERSION}' from Mojang!", version.ID);
        throw new NetworkInformationException((int)response.StatusCode);
    }
}