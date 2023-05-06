// LFInteractive LLC. - All Rights Reserved
namespace Chase.WebDeploy.Core;

public static class Values
{
    public static class Directories
    {
        public static string Configuration => Directory.CreateDirectory(Path.Combine(Root, "configuration")).FullName;
        public static string Instances => Directory.CreateDirectory(Path.Combine(Root, "instances")).FullName;
        public static string Logs => Directory.CreateDirectory(Path.Combine(Root, "logs")).FullName;
        public static string Modules => Directory.CreateDirectory(Path.Combine(Root, "modules")).FullName;
        public static string TEMP => Directory.CreateDirectory(Path.Combine(Root, "tmp")).FullName;
        public static string Root => Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LFInteractive", "Web Deploy")).FullName;
    }

    public static class Files
    {
        public static string ApplicationConfig => Path.Combine(Directories.Configuration, "settings.json");
        public static string InstancesManifest => Path.Combine(Directories.Instances, "instances.json");
    }
}