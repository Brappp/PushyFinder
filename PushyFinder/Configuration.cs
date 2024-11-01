using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace DiscordRelay.Util
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        [NonSerialized]
        private IDalamudPluginInterface PluginInterface;

        public string DiscordUserToken { get; set; } = ""; // For Discord token
        public string UserSecretKey { get; set; } = ""; // For secret key
        public bool EnableDiscordBot { get; set; } = false; // Toggle for the bot
        public bool EnableForDutyPops { get; set; } = false; // New property for duty pops
        public bool IgnoreAfkStatus { get; set; } = false; // New property for ignoring AFK status

        // Required by IPluginConfiguration
        public int Version { get; set; } = 1;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            PluginInterface = pluginInterface;
        }

        public void Save()
        {
            PluginInterface!.SavePluginConfig(this);
        }
    }
}
