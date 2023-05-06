// LFInteractive LLC. - All Rights Reserved
using Chase.WebDeploy.Core.Types;

namespace Chase.WebDeploy.Core.Models;

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