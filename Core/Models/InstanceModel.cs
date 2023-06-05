// LFInteractive LLC. - All Rights Reserved

using Chase.Vesta.Core.Types;

namespace Chase.Vesta.Core.Models;

public struct InstanceModel
{
    public Guid Id { get; } = Guid.NewGuid();

    public string Name { get; set; } = "";
    public int Java { get; set; }

    public InstanceState State { get; set; }

    public InstanceModel()
    {
    }
}