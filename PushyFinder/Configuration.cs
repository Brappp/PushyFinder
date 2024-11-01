using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace PushyFinder
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        [NonSerialized]
        private IDalamudPluginInterface? PluginInterface;

        public string PushoverAppKey { get; set; } = "";
        public string PushoverUserKey { get; set; } = "";
        public string PushoverDevice { get; set; } = "";
        public string DiscordWebhookToken { get; set; } = "";
        public string NtfyServer { get; set; } = "https://ntfy.sh/";
        public string NtfyTopic { get; set; } = "";
        public string NtfyToken { get; set; } = "";
        public bool EnableForDutyPops { get; set; } = true;
        public bool IgnoreAfkStatus { get; set; } = false;
        public bool DiscordUseEmbed { get; set; } = true;
        public uint DiscordEmbedColor { get; set; } = 0x00FF00;
        public int Version { get; set; } = 1;
        public string TelegramBotToken { get; set; } = "";
        public string TelegramChatId { get; set; } = "";
        public bool EnableTelegramBot { get; set; } = false;

        // New properties for DiscordDMDelivery
        public string DiscordUserToken { get; set; } = ""; // Stores the unique user token for Discord DM bot verification
        public bool EnableDiscordBot { get; set; } = false; // Toggles Discord DM bot functionality
        public string UserSecretKey { get; set; } = ""; // New property for storing the user-specific secret key

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
