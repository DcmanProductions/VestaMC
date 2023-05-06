// LFInteractive LLC. - All Rights Reserved

using Chase.WebDeploy.Core.Types;

namespace Chase.WebDeploy.Core.Models;

public struct InstanceModel
{
    public Guid Id { get; } = Guid.NewGuid();

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public InstanceState State { get; set; }

    public string Type { get; set; } = "";

    public InstanceModel()
    {
    }

    public struct Domain
    {
        public string DomainName { get; set; }
        public int Port { get; set; }
        public Domain[] SubDomains { get; set; }
    }
}