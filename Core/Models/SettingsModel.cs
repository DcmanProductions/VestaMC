// LFInteractive LLC. - All Rights Reserved
namespace Core.Models;

public struct SettingsModel
{
    public int Port { get; set; }
    public AuthenticationSettingsModel Authentication { get; set; }

    public struct AuthenticationSettingsModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}