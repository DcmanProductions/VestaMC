/*
VestaMC - LFInteractive LLC. (c) 2020-2024
a minecraft server hosting platform for windows and linux
https://github.com/dcmanproductions/VestaMC
Licensed under the GNU General Public License v3.0
https://www.gnu.org/licenses/lgpl-3.0.html
*/

using Chase.Vesta.Core;
using Chase.Vesta.Java.Controllers;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

namespace Chase.VestaMC.Test;

internal class Program
{
    private static readonly Dictionary<string, Action> TestOptions = new()
    {
        { "Java Validation", ValidateJavaTest }
    };

    private static void Main()
    {
        try
        {
            Directory.Delete(Values.Directories.TEMP, true);
        }
        catch { }
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(LogEventLevel.Verbose)
            .CreateLogger();
        Stopwatch stopwatch = Stopwatch.StartNew();
        Console.Clear();
        Console.WriteLine("Options:");
        Console.WriteLine("\t(*) All Options");
        for (int i = 0; i < TestOptions.Count; i++)
        {
            Console.WriteLine($"\t({i}) {TestOptions.ElementAt(i).Key}");
        }
        Console.Write("Pick Option: ");
        string input = Console.ReadLine()?.Trim() ?? "";

        if (!string.IsNullOrEmpty(input))
        {
            if (input == "*")
            {
                foreach (var item in TestOptions)
                {
                    item.Value.Invoke();
                }
            }
            else if (int.TryParse(input, out int value))
            {
                if (value < TestOptions.Count)
                {
                    TestOptions.ElementAt(value).Value.Invoke();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"Option {value} over the max");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"Option {input} is not valid");
                Console.ResetColor();
            }
        }

        Console.ForegroundColor = ConsoleColor.Red;
        stopwatch.Stop();
        Console.WriteLine($"Test finished in {stopwatch.Elapsed}");
        Console.ResetColor();

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Main();
    }

    private static void ValidateJavaTest()
    {
        Log.Information("Starting Java Test");
        List<string> failures = new();
        List<string> successes = new();
        Chase.Vesta.Java.Models.JavaVersionManifest[] items = JavaController.GetJavaVersionManifests().Result;
        Parallel.ForEach(items, item =>
        {
            try
            {
                string archive = JavaController.DownloadJavaArchive(item, (s, e) => { }).Result;
                if (JavaController.ValidateJavaArchive(archive, out string version))
                {
                    if (version == item.Version)
                    {
                        successes.Add(item.Version);
                        Log.Information("{ITEM} Succeeded!", item.Version);
                    }
                    else
                    {
                        Log.Information("{ITEM} Failed!", item.Version);
                        Log.Error("{ITEM} != {VALUE}", version, item.Version);
                        failures.Add(item.Version);
                    }
                }
                else
                {
                    Log.Information("{ITEM} Failed!", item.Version);
                    failures.Add(item.Version);
                }
            }
            catch(Exception e)
            {
                Log.Information("{ITEM} Failed due to exception {MSG}!", item.Version, e.Message);
                failures.Add(item.Version);
            }
        });
        Log.Information("{FAIL} Failures", failures.Count);
        Log.Information("{OK} Successes", successes.Count);
        try
        {
            Directory.Delete(Values.Directories.TEMP, true);
        }
        catch { }
    }
}