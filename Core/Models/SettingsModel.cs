// LFInteractive LLC. - All Rights Reserved
namespace Chase.Vesta.Core.Models;

public struct SettingsModel
{
    public struct AuthenticationSettingsModel
    {
        public string Username { get; set; } = "vesta";

        public string Password { get; set; } = "";

        public AuthenticationSettingsModel()
        {
        }
    }

    public int Port { get; set; } = 8256;

    public Guid Salt { get; set; } = Guid.NewGuid();

    public AuthenticationSettingsModel Authentication { get; set; } = new();

    public SettingsModel()
    {
    }
}