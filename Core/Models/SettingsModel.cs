/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

namespace Chase.Vesta.Core.Models;

public struct SettingsModel
{
    public struct AuthenticationSettingsModel
    {
        public string Username { get; set; } = "vesta";

        public string Password { get; set; } = "";

        public AuthenticationSettingsModel()
        {
        }
    }

    public int Port { get; set; } = 8256;

    public Guid Salt { get; set; } = Guid.NewGuid();

    public AuthenticationSettingsModel Authentication { get; set; } = new();

    public SettingsModel()
    {
    }
}