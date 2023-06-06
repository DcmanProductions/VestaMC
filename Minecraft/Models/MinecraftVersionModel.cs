/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

namespace Chase.VestaMC.Minecraft.Models;

public struct MinecraftVersionModel
{
    public string ID { get; set; }
    public string Type { get; set; }
    public Uri Url { get; set; }
    public DateTime Time { get; set; }
    public DateTime ReleaseType { get; set; }
}