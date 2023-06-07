/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Networking.Event;

namespace Chase.VestaMC.Vesta.Events;

public class InstallingInstanceEventArgs : EventArgs
{
    public string Stage { get; set; } = "";
    public DownloadProgressEventArgs? DownloadProgress { get; set; } = null;
}