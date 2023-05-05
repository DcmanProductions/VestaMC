// LFInteractive LLC. - All Rights Reserved
namespace Core.Models;

public struct InstanceModel
{
    public InstanceModel()
    {
    }

    public string Description { get; set; } = "";
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public InstanceState State { get; set; }
    public string Type { get; set; } = "";
}