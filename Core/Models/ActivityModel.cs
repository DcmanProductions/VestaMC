﻿/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Vesta.Core.Types;

namespace Chase.Vesta.Core.Models;

public struct ActivityModel
{
    public DateTime Created { get; } = DateTime.Now;

    public string Details { get; set; } = "";

    public Guid Id { get; } = Guid.NewGuid();

    public InstanceModel? Instance { get; set; }

    public ActivityType Type { get; set; }

    public ActivityModel()
    {
    }
}