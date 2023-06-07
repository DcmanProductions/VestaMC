/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Vesta.Java.Controllers;
using Chase.Vesta.Vesta.Models;
using Chase.Vesta.Vesta.Types;
using Chase.VestaMC.Minecraft.Controllers;
using Chase.VestaMC.Modded.Controllers;
using Chase.VestaMC.Modded.Data;
using Chase.VestaMC.Vesta.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chase.Vesta.Core.Controllers;

public class InstanceController
{
    private static readonly InstanceController Instance = Instance ??= new();

    public List<ActivityModel> Activity { get; private set; }

    public Dictionary<Guid, InstanceModel> Instances { get; private set; }

    private InstanceController()
    {
        Instances = new Dictionary<Guid, InstanceModel>();
        Activity = new List<ActivityModel>();
        LoadManifest();
    }

    public static bool TryAdd(InstanceModel model)
    {
        if (Instance.Instances.TryAdd(model.Id, model))
        {
            Instance.Activity.Add(new()
            {
                Instance = model,
                Type = ActivityType.CREATE,
                Details = $"Instance '{model.Name}' was created!"
            });
            Instance.SaveManifest();
            return true;
        }
        else
        {
            Instance.Activity.Add(new()
            {
                Instance = model,
                Type = ActivityType.FAILED_TO_CREATE,
                Details = $"Instance '{model.Name}' failed to be created!"
            });
            return false;
        }
    }

    public static async Task InstallInstance(InstanceModel instance, EventHandler<InstallingInstanceEventArgs> progressEvent)
    {
        string instanceDirectory = Directory.CreateDirectory(Path.Combine(Values.Directories.Instances, instance.Id.ToString())).FullName;

        if (!JavaController.IsJavaVersionInstalled(instance.JavaSettings.JavaVersion))
        {
            if (await JavaController.DoesRemoteJavaVersionExist(instance.JavaSettings.JavaVersion))
            {
                await JavaController.InstallVersion(await JavaController.GetRemoteJavaVersion(instance.JavaSettings.JavaVersion), (s, e) =>
                {
                    progressEvent.Invoke(null, new()
                    {
                        DownloadProgress = e,
                        Stage = $"Downloading Java {instance.JavaSettings.JavaVersion}"
                    });
                });
            }
            else
            {
                progressEvent.Invoke(null, new()
                {
                    Stage = $"Java {instance.JavaSettings.JavaVersion} not Found!"
                });
            }
        }

        await MinecraftVersionController.DownloadMinecraftServerJar(await MinecraftVersionController.GetMinecraftVersionByID(instance.MinecraftVersion), instanceDirectory, (s, e) =>
        {
            progressEvent.Invoke(null, new()
            {
                DownloadProgress = e,
                Stage = "Downloading Minecraft Server"
            });
        });
        switch (instance.ModLoader)
        {
            case SupportedModloaders.Vanilla:
                Instance.Instances[instance.Id] = new()
                {
                    Id = instance.Id,
                    MinecraftVersion = instance.MinecraftVersion,
                    Name = instance.Name,
                    JavaSettings = instance.JavaSettings,
                    ModLoader = instance.ModLoader,
                    ModLoaderVersion = instance.ModLoaderVersion,
                    State = instance.State,
                    StartingExecutable = "server.jar"
                };
                break;

            case SupportedModloaders.Forge:
                if (instance.ModLoaderVersion != null)
                {
                    await ForgeVersionController.DownloadForgeServerJar(instanceDirectory, instance.MinecraftVersion, instance.ModLoaderVersion, (s, e) =>
                    {
                        progressEvent.Invoke(null, new()
                        {
                            DownloadProgress = e,
                            Stage = "Downloading Forge Server"
                        });
                    });
                    Instance.Instances[instance.Id] = new()
                    {
                        Id = instance.Id,
                        MinecraftVersion = instance.MinecraftVersion,
                        Name = instance.Name,
                        JavaSettings = instance.JavaSettings,
                        ModLoader = instance.ModLoader,
                        ModLoaderVersion = instance.ModLoaderVersion,
                        State = instance.State,
                        StartingExecutable = null
                    };

                    progressEvent.Invoke(null, new()
                    {
                        Stage = "Installing Forge Server"
                    });
                }

                break;

            case SupportedModloaders.Fabric:
                if (instance.ModLoaderVersion != null)
                {
                    await FabricVersionController.DownloadFabricServerJar(instanceDirectory, instance.ModLoaderVersion, (s, e) =>
                    {
                        progressEvent.Invoke(null, new()
                        {
                            DownloadProgress = e,
                            Stage = "Downloading Fabric Server"
                        });
                    });
                    Instance.Instances[instance.Id] = new()
                    {
                        Id = instance.Id,
                        MinecraftVersion = instance.MinecraftVersion,
                        Name = instance.Name,
                        JavaSettings = instance.JavaSettings,
                        ModLoader = instance.ModLoader,
                        ModLoaderVersion = instance.ModLoaderVersion,
                        State = instance.State,
                        StartingExecutable = "fabric-server.jar"
                    };
                }
                break;
        }

        Instance.SaveManifest();
    }

    public static bool Exists(Guid id)
    {
        return Instance.Instances.ContainsKey(id);
    }

    public static InstanceModel[] Get()
    {
        return Instance.Instances.Values.ToArray();
    }

    public static InstanceModel? Get(Guid id)
    {
        return Instance.Instances.TryGetValue(id, out InstanceModel value) ? value : null;
    }

    public static ActivityModel[] GetActivity()
    {
        return Instance.Activity.ToArray();
    }

    private void LoadManifest()
    {
        if (!File.Exists(Values.Files.InstancesManifest))
        {
            SaveManifest();
        }
        try
        {
            using FileStream fs = new(Values.Files.InstancesManifest, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new(fs);
            Instances = JObject.Parse(reader.ReadToEnd()).ToObject<Dictionary<Guid, InstanceModel>>() ?? new Dictionary<Guid, InstanceModel>();
        }
        catch
        {
            SaveManifest();
        }
    }

    private void SaveManifest()
    {
        using FileStream fs = new(Values.Files.InstancesManifest, FileMode.Create, FileAccess.Write);
        using StreamWriter writer = new(fs);
        writer.Write(JsonConvert.SerializeObject(Instances, Formatting.Indented));
    }
}