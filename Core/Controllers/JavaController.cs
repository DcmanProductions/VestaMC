// LFInteractive LLC. - All Rights Reserved
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Chase.Vesta.Core.Controllers;

public static class JavaController
{
    public static async Task<Uri?> GetJavaDirectDownloadLink(int version)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
        {
            Regex regex = new($"https:\\/\\/download\\.java\\.net\\/openjdk\\/.*?{(OperatingSystem.IsWindows() ? "windows" : "linux")}.*?(?:\\.tar\\.gz|\\.zip)(?=\")");

            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync($"https://jdk.java.net/java-se-ri/{version}{(version == 8 ? "-MR5" : "")}");
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
                    Console.WriteLine($"Unable to parse url: {url} - {version}");
                }
            }
        }

        return null;
    }

    public static async Task<int[]> GetJavaVersions()
    {
        List<int> versions = new();
        using (HttpClient client = new())
        {
            HttpResponseMessage response = await client.GetAsync("https://jdk.java.net/java-se-ri/19");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new();
                document.OptionFixNestedTags = true;
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
                                versions.Add(version);
                            }
                        }
                    }
                }
            }
        }
        return versions.ToArray();
    }
}