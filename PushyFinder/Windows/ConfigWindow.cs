using System;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using DiscordRelay.Delivery;
using DiscordRelay.Util;

namespace DiscordRelay.Windows // Updated namespace
{
    public class ConfigWindow : Window, IDisposable
    {
        private readonly Configuration Configuration; // Holds the configuration settings
        private readonly DiscordRelay plugin; // Updated reference to DiscordRelay
        private readonly TimedBool notifSentMessageTimer = new(3.0f); // Timer for notifications

        public ConfigWindow(DiscordRelay plugin) : base( // Constructor accepting the main plugin instance
            "DiscordRelay Configuration", // Window title
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
        {
            this.plugin = plugin;
            Configuration = DiscordRelay.Configuration; // Accessing the static configuration
        }

        public void Dispose() { }

        private void DrawDiscordDMConfig()
        {
            // Checkbox to enable or disable the Discord DM bot
            var enableDiscordBot = Configuration.EnableDiscordBot;
            if (ImGui.Checkbox("Enable Discord DM Bot", ref enableDiscordBot))
            {
                Configuration.EnableDiscordBot = enableDiscordBot; // Update the configuration
                Configuration.Save(); // Save the changes

                if (enableDiscordBot)
                {
                    Service.PluginLog.Debug("Starting Discord DM bot...");
                    if (plugin.DiscordDMDelivery == null) // Check for null before creating a new instance
                    {
                        plugin.DiscordDMDelivery = new DiscordDMDelivery();
                    }

                    plugin.DiscordDMDelivery.StartListening(); // Start the delivery service
                }
                else
                {
                    Service.PluginLog.Debug("Stopping Discord DM bot...");
                    plugin.DiscordDMDelivery?.StopListening(); // Safely call StopListening
                }
            }

            // Instructions for setting up the Discord DM bot
            ImGui.TextWrapped("To set up the Discord DM bot, type `hello` in a direct message to the bot in Discord. The bot will respond with interactive buttons to register, show, or remove your credentials.");

            // Input fields for User Token and User Secret Key
            var userToken = Configuration.DiscordUserToken;
            if (ImGui.InputText("User Token", ref userToken, 2048u))
            {
                Configuration.DiscordUserToken = userToken; // Update token in configuration
            }

            var userSecretKey = Configuration.UserSecretKey;
            if (ImGui.InputText("User Secret Key", ref userSecretKey, 2048u))
            {
                Configuration.UserSecretKey = userSecretKey; // Update secret key in configuration
            }

            // Reminder to save the configuration after entering the values
            ImGui.TextColored(new Vector4(0.9f, 0.9f, 0.1f, 1.0f), "Remember to save your configuration after entering the Token and Secret Key.");

            // Test notification button
            if (ImGui.Button("Send test notification"))
            {
                notifSentMessageTimer.Start(); // Start the notification timer
                MasterDelivery.Deliver("Test notification", "If you received this, DiscordRelay is configured correctly."); // Send a test notification
            }

            if (notifSentMessageTimer.Value) // Check if notification has been sent
            {
                ImGui.SameLine();
                ImGui.Text("Notification sent!");
            }

            // Save configuration changes
            if (ImGui.Button("Save Configuration"))
            {
                Configuration.Save(); // Call save method to persist changes
            }
        }

        public override void Draw()
        {
            DrawDiscordDMConfig(); // Draw the configuration window content
        }
    }
}
