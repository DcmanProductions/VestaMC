﻿/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using System.Reflection;

namespace Chase.Vesta.Core;

public static class Values
{
    public static class Directories
    {
        public static string Configuration => Directory.CreateDirectory(Path.Combine(Root, "configuration")).FullName;
        public static string Instances => Directory.CreateDirectory(Path.Combine(Root, "instances")).FullName;
        public static string Java => Directory.CreateDirectory(Path.Combine(Root, "java")).FullName;
        public static string Logs => Directory.CreateDirectory(Path.Combine(Root, "logs")).FullName;
        public static string TEMP => Directory.CreateDirectory(Path.Combine(Root, "tmp")).FullName;
        public static string Root => Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName ?? "./";
    }

    public static class Files
    {
        public static string ApplicationConfig => Path.Combine(Directories.Configuration, "settings.json");
        public static string InstancesManifest => Path.Combine(Directories.Instances, "instances.json");
    }

    public static string ApplicationName = "Vesta";
}