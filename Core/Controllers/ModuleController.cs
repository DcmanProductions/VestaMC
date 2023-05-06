// LFInteractive LLC. - All Rights Reserved
using Chase.Networking;
using Chase.Networking.Event;
using Chase.WebDeploy.Modules;
using CLMath;
using Serilog;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reflection;

namespace Chase.WebDeploy.Core.Controllers;

public class ModuleController
{
    public static readonly ModuleController Instance = Instance ??= new();
    public ConcurrentDictionary<string, IModuleInitialization> Modules { get; private set; }

    private ModuleController()
    {
        Modules = new ConcurrentDictionary<string, IModuleInitialization>();
        Load();
    }

    public async Task<bool> InstallModule(Uri dllArchive, string moduleIdentifier, DownloadProgressEvent progressEvent)
    {
        using NetworkClient client = new();
        string dll = Path.Combine(Values.Directories.Modules, $"{moduleIdentifier}.dll");
        await client.DownloadFileAsync(dllArchive, dll, (s, e) =>
        {
            Log.Debug("[{}] Downloading Module: {} at {}/s", moduleIdentifier, e.Percentage.ToString("P2"), CLFileMath.AdjustedFileSize(e.BytesPerSecond));
            progressEvent.Invoke(s, e);
        });
        return await Load(dll);
    }

    public async Task<bool> DownloadArchive(string moduleIdentifier, DownloadProgressEvent progressEvent)
    {
        try
        {
            if (Modules.TryGetValue(moduleIdentifier, out IModuleInitialization value))
            {
                Uri? downloadUri = value.GetContentArchiveSource();
                if (downloadUri != null)
                {
                    string archiveName = Path.Combine(Values.Directories.TEMP, $"{moduleIdentifier}-{Path.GetRandomFileName()}");
                    using (NetworkClient client = new())
                    {
                        await client.DownloadFileAsync(downloadUri, archiveName, (s, e) =>
                        {
                            Log.Debug("[{}] Downloading Archive: {} at {}/s", moduleIdentifier, e.Percentage.ToString("P2"), CLFileMath.AdjustedFileSize(e.BytesPerSecond));
                            progressEvent.Invoke(s, e);
                        });
                    }
                    string moduleContentDirectory = Path.Combine(Values.Directories.Instances, moduleIdentifier);
                    if (Directory.Exists(moduleContentDirectory))
                    {
                        Directory.Delete(moduleContentDirectory, true);
                    }
                    using FileStream fs = new(archiveName, FileMode.Open, FileAccess.Read);
                    using ZipArchive zip = new(fs, ZipArchiveMode.Read);
                    zip.ExtractToDirectory(Directory.CreateDirectory(moduleContentDirectory).FullName);
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("[{}] Unable to download module content archive!", moduleIdentifier, e);
        }

        return false;
    }

    private void Load()
    {
        string[] module_dlls = Directory.GetFiles(Values.Directories.Modules, "*.dll", SearchOption.TopDirectoryOnly);
        Parallel.ForEach(module_dlls, async dll =>
        {
            await Load(dll);
        });
    }

    private async Task<bool> Load(string dll)
    {
        Log.Information("Loading module file '{}'", new FileInfo(dll).Name);
        Assembly assembly = Assembly.Load(dll);
        if (assembly != null)
        {
            Type[] types = assembly.GetTypes();
            Type baseType = typeof(IModuleInitialization);
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(baseType))
                {
                    IModuleInitialization moduleInitialization = (IModuleInitialization)type;
                    if (Modules.TryAdd(moduleInitialization.GetModuleIdentifier(), moduleInitialization))
                    {
                        try
                        {
                            Log.Debug("Beginning initialization sequence for module '{MODULE}'", moduleInitialization.GetModuleIdentifier());
                            await moduleInitialization.Initialize();
                            Log.Debug("Completed initialization sequence for module '{MODULE}'", moduleInitialization.GetModuleIdentifier());
                        }
                        catch (Exception e)
                        {
                            Log.Error("[{}] Unable to initialize module", moduleInitialization.GetModuleIdentifier(), e);
                            return false;
                        }
                    }

                    return true;
                }
            }
        }
        return false;
    }
}