// LFInteractive LLC. - All Rights Reserved
using Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Controllers;

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

    public static void AddOrUpdate(InstanceModel model)
    {
        if (Instance.Instances.TryAdd(model.Id, model))
        {
            Instance.Activity.Add(new()
            {
                Instance = model,
                Type = ActivityType.CREATE,
                Details = $"Instance '{model.Name}' was created!"
            });
        }
        else
        {
            Instance.Activity.Add(new()
            {
                Instance = model,
                Type = ActivityType.FAILED_TO_CREATE,
                Details = $"Instance '{model.Name}' failed to be created!"
            });
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