using System;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PushyFinder.Delivery;
using PushyFinder.Util;

namespace PushyFinder.Windows
{
    public class ConfigWindow : Window, IDisposable
    {
        private readonly Configuration Configuration;
        private readonly Plugin plugin;
        private readonly TimedBool notifSentMessageTimer = new(3.0f);

        public ConfigWindow(Plugin plugin) : base(
            "PushyFinder Configuration",
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
        {
            this.plugin = plugin;
            Configuration = Plugin.Configuration;
        }

        public void Dispose() { }

        // Define placeholder methods for missing configurations

        private void DrawPushoverConfig()
        {
            ImGui.Text("Pushover configuration section goes here.");
            // Add the actual configuration code for Pushover here
        }

        private void DrawNtfyConfig()
        {
            ImGui.Text("Ntfy configuration section goes here.");
            // Add the actual configuration code for Ntfy here
        }

        private void DrawDiscordConfig()
        {
            ImGui.Text("Discord configuration section goes here.");
            // Add the actual configuration code for Discord here
        }

        private void DrawTelegramConfig()
        {
            ImGui.Text("Telegram configuration section goes here.");
            // Add the actual configuration code for Telegram here
        }

        private void DrawDiscordDMConfig()
        {
            // Checkbox to enable or disable the Discord DM bot
            var enableDiscordBot = Configuration.EnableDiscordBot;
            if (ImGui.Checkbox("Enable Discord DM Bot", ref enableDiscordBot))
            {
                Configuration.EnableDiscordBot = enableDiscordBot;
                Configuration.Save();

                if (enableDiscordBot)
                {
                    Service.PluginLog.Debug("Starting Discord DM bot...");
                    if (plugin.DiscordDMDelivery == null)
                        plugin.DiscordDMDelivery = new DiscordDMDelivery();
                    plugin.DiscordDMDelivery.StartListening();
                }
                else
                {
                    Service.PluginLog.Debug("Stopping Discord DM bot...");
                    plugin.DiscordDMDelivery?.StopListening();
                }
            }

            // Instructions to guide users on obtaining their token and secret key
            ImGui.TextWrapped("To set up the Discord DM bot, type `hello` in a direct message to the bot in Discord. The bot will respond with interactive buttons to register, show, or remove your credentials.");

            // Input field for User Token with label and tooltip
            var userToken = Configuration.DiscordUserToken;
            if (ImGui.InputText("User Token", ref userToken, 2048u))
                Configuration.DiscordUserToken = userToken;
            ImGui.TextWrapped("Paste the Token provided by the bot here.");

            // Input field for User Secret Key with label and tooltip
            var userSecretKey = Configuration.UserSecretKey;
            if (ImGui.InputText("User Secret Key", ref userSecretKey, 2048u))
                Configuration.UserSecretKey = userSecretKey;
            ImGui.TextWrapped("Paste the Secret Key provided by the bot here.");

            // Reminder to save the configuration after entering the values
            ImGui.TextColored(new Vector4(0.9f, 0.9f, 0.1f, 1.0f), "Remember to save your configuration after entering the Token and Secret Key.");

            // Save configuration changes
            Configuration.Save();
        }

        public override void Draw()
        {
            using (var tabBar = ImRaii.TabBar("Services"))
            {
                if (tabBar)
                {
                    using (var pushoverTab = ImRaii.TabItem("Pushover"))
                    {
                        if (pushoverTab) DrawPushoverConfig();
                    }
                    using (var ntfyTab = ImRaii.TabItem("Ntfy"))
                    {
                        if (ntfyTab) DrawNtfyConfig();
                    }
                    using (var discordTab = ImRaii.TabItem("Discord"))
                    {
                        if (discordTab) DrawDiscordConfig();
                    }
                    using (var discordDMTab = ImRaii.TabItem("Discord DM"))
                    {
                        if (discordDMTab) DrawDiscordDMConfig();
                    }
                    using (var telegramTab = ImRaii.TabItem("Telegram"))
                    {
                        if (telegramTab) DrawTelegramConfig();
                    }
                }
            }

            ImGui.NewLine();

            if (ImGui.Button("Send test notification"))
            {
                notifSentMessageTimer.Start();
                MasterDelivery.Deliver("Test notification",
                                       "If you received this, PushyFinder is configured correctly.");
            }

            if (notifSentMessageTimer.Value)
            {
                ImGui.SameLine();
                ImGui.Text("Notification sent!");
            }

            var enableDutyPops = Configuration.EnableForDutyPops;
            if (ImGui.Checkbox("Send message for duty pop?", ref enableDutyPops))
                Configuration.EnableForDutyPops = enableDutyPops;

            var ignoreAfkStatus = Configuration.IgnoreAfkStatus;
            if (ImGui.Checkbox("Ignore AFK status and always notify", ref ignoreAfkStatus))
                Configuration.IgnoreAfkStatus = ignoreAfkStatus;

            if (!Configuration.IgnoreAfkStatus)
            {
                if (!CharacterUtil.IsClientAfk())
                {
                    var red = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
                    ImGui.TextColored(red, "This plugin will only function while your client is AFK (/afk, red icon)!");

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Notifications are disabled unless you're AFK.");
                        ImGui.EndTooltip();
                    }
                }
                else
                {
                    var green = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
                    ImGui.TextColored(green, "You are AFK. The plugin is active and notifications will be served.");
                }
            }

            if (ImGui.Button("Save and close"))
            {
                Configuration.Save();
                IsOpen = false;
            }
        }
    }
}
