// LFInteractive LLC. - All Rights Reserved
using Chase.WebDeploy.Modules.Models;

namespace Chase.WebDeploy.Modules;

public interface IModuleInitialization
{
    async Task Initialize() { }

    string GetCommand(RunnableInstance runnableInstance);

    /// <summary> This is the uri of the modules needed binaries<br> For example if your module
    /// requires a specific executable to operate, instead of bundling it with the module, link a
    /// direct download to the archive </summary> <returns>The direct uri for the archive</returns>
    Uri? GetContentArchiveSource();

    ServerArchitectureModel GetServerArchitecture();

    string GetModuleIdentifier();
}