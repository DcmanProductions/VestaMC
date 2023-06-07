/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.VestaMC.Modded.Controllers;
using Newtonsoft.Json;

namespace Chase.VestaMC.Modded.Models;

public struct LoaderVersion
{
    [JsonIgnore]
    public int Major { get; set; }

    [JsonIgnore]
    public int Minor { get; set; }

    [JsonIgnore]
    public int Patch { get; set; }

    [JsonIgnore]
    public string Build { get; set; }

    [JsonProperty]
    public string Version => $"{Major}.{Minor}.{Patch}{Build}";

    [JsonProperty]
    public Uri DownloadUri => new($"{FabricVersionController.FabricMavenUrl}{Version}/fabric-loader-{Version}.jar");
}