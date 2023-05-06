// LFInteractive LLC. - All Rights Reserved
using System.ComponentModel.DataAnnotations;

namespace Chase.WebDeploy.Modules.Models;

public readonly struct RunnableInstance
{
    [Required]
    public string WorkingDirectory { get; }

    [Required]
    public string StartupExecutable { get; }

    [Required]
    public int Port { get; }

    public RunnableInstance(string workingDirectory, string startupExecutable, int port)
    {
        WorkingDirectory = workingDirectory;
        StartupExecutable = startupExecutable;
        Port = port;
    }
}