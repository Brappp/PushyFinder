using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using DiscordRelay.Impl; // Updated namespace
using DiscordRelay.Util; // Updated namespace
using DiscordRelay.Windows; // Updated namespace
using DiscordRelay.Delivery; // Updated namespace

namespace DiscordRelay // Updated namespace
{
    public sealed class DiscordRelay : IDalamudPlugin // Updated class name
    {
        public string Name => "DiscordRelay"; // Updated plugin name
        private const string CommandName = "/discordrelay"; // Updated command name

        private IDalamudPluginInterface PluginInterface { get; init; }
        public static Configuration Configuration { get; private set; } // Static configuration property
        public DiscordDMDelivery? DiscordDMDelivery { get; set; } // Instance for Discord DM delivery

        public WindowSystem WindowSystem = new("DiscordRelay"); // Updated window name
        private ConfigWindow ConfigWindow { get; init; }

        public DiscordRelay(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<Service>(); // Create service instance

            PluginInterface = pluginInterface;

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);

            ConfigWindow = new ConfigWindow(this); // Initialize the configuration window
            WindowSystem.AddWindow(ConfigWindow);

            Service.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens the configuration window."
            });

            PluginInterface.UiBuilder.Draw += DrawUI; // Register the UI draw method
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI; // Register the config UI opening method

            // Initialize Discord DM bot if enabled
            if (Configuration.EnableDiscordBot)
            {
                DiscordDMDelivery = new DiscordDMDelivery();
                if (DiscordDMDelivery != null && DiscordDMDelivery.IsActive)
                    DiscordDMDelivery.StartListening(); // Start listening for Discord DM delivery
            }

            CrossWorldPartyListSystem.Start(); // Start cross-world party list system
            PartyListener.On(); // Start party listener
            DutyListener.On(); // Start duty listener
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows(); // Clean up UI windows
            ConfigWindow.Dispose(); // Dispose of the configuration window

            CrossWorldPartyListSystem.Stop(); // Stop cross-world party list system
            PartyListener.Off(); // Stop party listener
            DutyListener.Off(); // Stop duty listener

            Service.CommandManager.RemoveHandler(CommandName); // Remove command handler

            // Stop and disable Discord DM bot if enabled
            if (Configuration.EnableDiscordBot)
            {
                DiscordDMDelivery?.StopListening(); // Stop listening if DM delivery is active
                Configuration.EnableDiscordBot = false; // Disable the bot in configuration
                Configuration.Save(); // Save changes to configuration
            }
        }

        private void OnCommand(string command, string args)
        {
            if (args == "debugOnlineStatus") // Handle debug command
            {
                Service.ChatGui.Print($"OnlineStatus ID = {Service.ClientState.LocalPlayer!.OnlineStatus.Id}");
                return;
            }

            ConfigWindow.IsOpen = true; // Open the configuration window
        }

        private void DrawUI() // Method to draw the UI
        {
            WindowSystem.Draw(); // Draw the window system
        }

        public void DrawConfigUI() // Method to open the configuration UI
        {
            ConfigWindow.IsOpen = true; // Set the configuration window to open
        }
    }
}
