/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Vesta.Java.Models;
using Chase.VestaMC.Vesta.Types;

namespace Chase.Vesta.Vesta.Models;

public struct InstanceModel
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public InstanceState State { get; set; }
    public string StartingExecutable { get; set; }
    public JavaInstanceSettingsModel JavaSettings { get; set; } = new();

    public InstanceModel()
    {
    }
}