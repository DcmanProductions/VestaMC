// LFInteractive LLC. - All Rights Reserved
namespace Core.Models;

public struct ActivityModel
{
    public ActivityModel()
    {
    }

    public DateTime Created { get; } = DateTime.Now;
    public string Details { get; set; } = "";
    public Guid Id { get; } = Guid.NewGuid();
    public InstanceModel? Instance { get; set; }
    public ActivityType Type { get; set; }
}