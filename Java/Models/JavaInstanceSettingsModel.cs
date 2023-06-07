/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

namespace Chase.Vesta.Java.Models;

public struct JavaInstanceSettingsModel
{
    public string JavaVersion { get; set; } = "17";

    public string JavaArguments { get; set; } = "";

    public float MaxRam { get; set; } = 4096f;

    public float InitalRam { get; set; } = 1024f;

    public float PermGenRam { get; set; } = 128f;

    public JavaInstanceSettingsModel()
    {
    }
}