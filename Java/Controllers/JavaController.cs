/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Networking;
using Chase.Networking.Event;
using Chase.Vesta.Core;
using Chase.Vesta.Java.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using SharpCompress.Readers;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Chase.Vesta.Java.Controllers;

public static class JavaController
{
    public static string GetLocallyInstalledJavaVersion(string version) => Path.Combine(Values.Directories.Java, version);

    public static bool IsJavaVersionInstalled(string version) => Directory.Exists(GetLocallyInstalledJavaVersion(version));

    public static string? GetVersionExecutable(string version) => IsJavaVersionInstalled(version) ? Path.Combine(Values.Directories.Java, version.ToString(), "bin", OperatingSystem.IsWindows() ? "java.exe" : "java") : null;

    public static async Task<JavaVersionManifest> GetRemoteJavaVersion(string version) => (await GetJavaVersionManifests()).First(s => s.Version == version);

    public static async Task<bool> DoesRemoteJavaVersionExist(string version) => (await GetJavaVersionManifests()).Any(s => s.Version == version);

    public static async Task InstallVersion(JavaVersionManifest version, DownloadProgressEvent progress)
    {
        Log.Information("Installing Java Version {VERSION}", version);
        string archiveFile = await DownloadJavaArchive(version, progress);
        string output = GetLocallyInstalledJavaVersion(version.Version);
        InstallLocalVersion(archiveFile, output, version.Version);
    }

    public static async Task<string> DownloadJavaArchive(JavaVersionManifest version, DownloadProgressEvent progress)
    {
        string archiveFile = "";
        using (NetworkClient client = new())
        {
            Uri? directUri = version.DirectDownloadUri;
            if (directUri != null)
            {
                Log.Debug("Downloading archive: {pageUri}", directUri);
                string file = directUri.Segments.Last();
                archiveFile = Path.Combine(Values.Directories.TEMP, file);
                if (File.Exists(archiveFile))
                {
                    File.Delete(archiveFile);
                }
                await client.DownloadFileAsync(directUri, archiveFile, progress);
            }
        }
        return archiveFile;
    }

    public static void InstallLocalVersion(string archiveFile, string output, string version, bool cleanup = true)
    {
        Log.Debug("Extracting '{file}'", archiveFile);
        if (archiveFile.EndsWith("tar.gz"))
        {
            ExtractTarGz(archiveFile, output, version);
        }
        else if (archiveFile.EndsWith("zip"))
        {
            ExtractZip(archiveFile, output, version);
        }
        if (cleanup)
            File.Delete(archiveFile);
    }

    public static bool ValidateJavaArchive(string archiveFile, out string version)
    {
        version = "";
        string testDirectory = Directory.CreateDirectory(Path.Combine(Values.Directories.TEMP, Path.GetRandomFileName())).FullName;
        try
        {
            InstallLocalVersion(archiveFile, testDirectory, Guid.NewGuid().ToString(), false);
        }
        catch (Exception e)
        {
            Log.Error("Unable to Validate Java Archive: Unable to extract archive - {MSG}", e.Message, e);
            return false;
        }

        string executable = Path.Combine(testDirectory, "bin", OperatingSystem.IsWindows() ? "java.exe" : "java");

        Process process = new()
        {
            StartInfo = new()
            {
                FileName = executable,
                Arguments = "-version",
                RedirectStandardOutput = true,
            },
            EnableRaisingEvents = true,
        };
        process.Start();
        process.WaitForExit();
        string[] words = process.StandardOutput.ReadToEnd().Split('\n').First().Split(' ');
        if (words.Length == 3)
        {
            version = words[2].Trim('"');
            if (version.Contains('.'))
            {
                if (int.TryParse(version.First().ToString(), out int _))
                {
                    version = version.Split('.')[1].Trim();
                    return true;
                }
            }
            return true;
        }

        return false;
    }

    public static async Task<JavaVersionManifest[]> GetJavaVersionManifests(bool useCached = true)
    {
        List<JavaVersionManifest> versions = new();
        string manifestFile = Path.Combine(Values.Directories.Java, "manifest.json");
        if (useCached && File.Exists(manifestFile))
        {
            using FileStream fs = File.OpenRead(manifestFile);
            using StreamReader reader = new(fs);
            return JArray.Parse(reader.ReadToEnd()).ToObject<JavaVersionManifest[]>() ?? Array.Empty<JavaVersionManifest>();
        }
        using (HttpClient client = new())
        {
            // Gets from Oracle
            HttpResponseMessage response = await client.GetAsync("https://www.oracle.com/java/technologies/downloads/archive/");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new()
                {
                    OptionFixNestedTags = true
                };
                document.LoadHtml(content);
                if (document.DocumentNode != null)
                {
                    HtmlNode body = document.DocumentNode.SelectSingleNode("//body");
                    if (body != null)
                    {
                        HtmlNodeCollection links = body.SelectNodes("//div[@class='rc30w5']/h5[text()='Java SE downloads']/following-sibling::ul/li[@class='icn-chevron-right']/a");

                        foreach (HtmlNode link in links)
                        {
                            string name = link.InnerText;
                            Uri pageUri = new($"https://www.oracle.com{link.GetAttributeValue("href", "")}");
                            string value = name.Split("Java SE").Last().Trim().Split(' ').First().Trim();
                            if (int.TryParse(value, out int version))
                            {
                                if (version == 8 && name.Contains("(8u202 and earlier)"))
                                {
                                    continue;
                                }
                                Uri? direct = await GetJavaDirectDownloadLinkFromOracle(version, pageUri);
                                if (direct != null)
                                {
                                    versions.Add(new()
                                    {
                                        Version = version.ToString(),
                                        PageUri = pageUri,
                                        DirectDownloadUri = direct
                                    });
                                }
                            }
                        }
                    }
                }
            }

            // Gets OpenJDK
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    response = await client.GetAsync("https://jdk.java.net/java-se-ri/19");
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        HtmlDocument document = new()
                        {
                            OptionFixNestedTags = true
                        };
                        document.LoadHtml(content);
                        if (document.DocumentNode != null)
                        {
                            HtmlNode body = document.DocumentNode.SelectSingleNode("//body");
                            if (body != null)
                            {
                                HtmlNodeCollection links = body.SelectNodes("//div[@class='links']/div[@class='about' and text()='Reference Implementations']/following-sibling::div[@class='link']/a");

                                foreach (HtmlNode link in links)
                                {
                                    if (int.TryParse(link.InnerText.Split(" ").Last(), out int version))
                                    {
                                        if (!versions.Any(x => x.Version == version.ToString()))
                                        {
                                            Uri pageUri = new($"https://jdk.java.net/java-se-ri/{version}{(version == 8 ? "-MR5" : "")}");
                                            Uri? direct = await GetJavaDirectDownloadLinkFromOpenJDK(version, pageUri);
                                            if (direct != null)
                                            {
                                                versions.Add(new()
                                                {
                                                    Version = version.ToString(),
                                                    PageUri = pageUri,
                                                    DirectDownloadUri = direct
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        using StreamWriter writer = File.CreateText(manifestFile);
        writer.Write(JsonConvert.SerializeObject(versions));
        writer.Flush();
        return versions.ToArray();
    }

    public static string[] GetLocallyInstalledJavaVersions()
    {
        List<string> versions = new();
        string[] entries = Directory.GetFileSystemEntries(Values.Directories.Java, "*", SearchOption.TopDirectoryOnly);
        foreach (string entry in entries)
        {
            FileInfo info = new(entry);
            if (info.Attributes.HasFlag(FileAttributes.Directory))
            {
                versions.Add(info.Name);
            }
        }

        return versions.ToArray();
    }

    public static async Task<Uri?> GetJavaDirectDownloadLinkFromOpenJDK(int version, Uri pageUri)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
        {
            if (Environment.Is64BitOperatingSystem)
            {
                Regex regex = new($"https:\\/\\/download\\.java\\.net\\/openjdk\\/.*?{(OperatingSystem.IsWindows() ? "windows" : "linux")}.*?(?:\\.tar\\.gz|\\.zip)(?=\")");

                using HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync(pageUri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    string url = regex.Match(content).Value;
                    try
                    {
                        Uri uri = new(url);
                        return uri;
                    }
                    catch
                    {
                        Log.Error("Unable to parse url: {url} - {version}", url, version);
                    }
                }
            }
        }

        return null;
    }

    private static async Task<Uri?> GetJavaDirectDownloadLinkFromOracle(int version, Uri pageUri)
    {
        Log.Debug("Getting direct download link for Java {VERSION}", version);
        string? platform = null;
        if (OperatingSystem.IsWindows())
        {
            if (Environment.Is64BitOperatingSystem)
            {
                platform = "windows-x64";
            }
        }
        else if (OperatingSystem.IsLinux())
        {
            if (Environment.Is64BitOperatingSystem)
            {
                platform = "linux-x64";
            }
            else
            {
                platform = "linux-aarch64";
            }
        }
        else if (OperatingSystem.IsMacOS())
        {
            platform = "macos-64";
        }
        else if (OperatingSystem.IsMacCatalyst())
        {
            platform = "macos-aarch64";
        }

        if (platform == null)
        {
            throw new SystemException($"Invalid platform: {Enum.GetName(typeof(PlatformID), Environment.OSVersion.Platform)}");
        }

        Regex regex = new($"(?<=\")https:\\/\\/download\\.oracle\\.com\\/java\\/.*?{platform}.*?(?:\\.tar\\.gz|\\.zip)(?=\")");

        using HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync(pageUri);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            string url = regex.Match(content).Value;
            try
            {
                Uri uri = new(url);
                Log.Debug("Direct uri found: '{URI}'", uri);
                return uri;
            }
            catch
            {
                Log.Error("Unable to parse url: {url} - {version}", url, version);
            }
        }
        return null;
    }

    private static void ExtractZip(string archive, string output, string version)
    {
        using ZipArchive zip = new(File.OpenRead(archive));
        string tmp = Directory.CreateDirectory(Path.Combine(Values.Directories.TEMP, version)).FullName;
        zip.ExtractToDirectory(tmp);
        CleanUpInstallation(tmp, output);
    }

    private static void ExtractTarGz(string archive, string output, string version)
    {
        using GZipStream gzip = new(File.OpenRead(archive), CompressionMode.Decompress);
        const int chunk = 4096;
        using MemoryStream ms = new();
        byte[] buffer = new byte[chunk];
        int read;
        do
        {
            read = gzip.Read(buffer, 0, chunk);
            ms.Write(buffer, 0, read);
        } while (read == chunk);
        ms.Seek(0, SeekOrigin.Begin);
        string tmp = Directory.CreateDirectory(Path.Combine(Values.Directories.TEMP, version)).FullName;

        using FileStream fs = File.OpenRead(archive);

        using IReader reader = ReaderFactory.Open(fs);
        while (reader.MoveToNextEntry())
        {
            if (!reader.Entry.IsDirectory)
            {
                reader.WriteEntryToDirectory(tmp, new() { ExtractFullPath = true, Overwrite = true });
            }
        }
        CleanUpInstallation(tmp, output);
    }

    private static void CleanUpInstallation(string extracted, string output)
    {
        string primary = Directory.GetFileSystemEntries(extracted, "*", SearchOption.TopDirectoryOnly).First(i => new FileInfo(i).Attributes.HasFlag(FileAttributes.Directory));

        if (Directory.Exists(output)) Directory.Delete(output, true);
        Directory.Move(primary, output);
        Directory.Delete(extracted, true);
    }
}