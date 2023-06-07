/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

namespace Chase.VestaMC.Modded.Models;

public struct LoaderVersion
{
    public int Major { get; set; }

    public int Minor { get; set; }

    public int Patch { get; set; }

    public string Build { get; set; }

    public string Version => $"{Major}.{Minor}.{Patch}{Build}";

    public string? MinecraftVersion { get; set; }

    public Uri DownloadUri { get; set; }
}