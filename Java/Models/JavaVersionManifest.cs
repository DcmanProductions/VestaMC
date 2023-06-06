/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Newtonsoft.Json;

namespace Chase.Vesta.Java.Models;

public struct JavaVersionManifest
{
    public int Version { get; set; }
    public Uri PageUri { get; set; }
    public Uri DirectDownloadUri { get; set; }

    public override string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}