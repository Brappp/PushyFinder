using System.Collections.Generic;
using Dalamud.Utility;

namespace PushyFinder.Delivery
{
    internal interface IDelivery
    {
        bool IsActive { get; }
        void Deliver(string title, string text);
    }

    public static class MasterDelivery
    {
        private static List<IDelivery> _deliveries = new List<IDelivery>();

        public static void InitializeDeliveries()
        {
            _deliveries.Clear();

            // Add the default deliveries
            _deliveries.Add(new PushoverDelivery());
            _deliveries.Add(new NtfyDelivery());

            // Add Discord delivery if enabled in configuration
            if (Plugin.Configuration.EnableDiscordBot)
            {
                var discordDelivery = new DiscordDMDelivery();
                if (discordDelivery.IsActive)
                    _deliveries.Add(discordDelivery);
                Service.PluginLog.Debug("DiscordDMDelivery added to active deliveries.");
            }

            // Add Telegram delivery if enabled in configuration
            if (Plugin.Configuration.EnableTelegramBot)
            {
                var telegramDelivery = new TelegramDelivery();
                if (telegramDelivery.IsActive)
                    _deliveries.Add(telegramDelivery);
                Service.PluginLog.Debug("TelegramDelivery added to active deliveries.");
            }
        }

        public static void Deliver(string title, string text)
        {
            InitializeDeliveries(); // Re-initialize deliveries to ensure updated config

            foreach (var delivery in _deliveries)
            {
                if (delivery.IsActive)
                {
                    Service.PluginLog.Debug($"Sending '{title}' to delivery type: {delivery.GetType().Name}");
                    delivery.Deliver(title, text);
                }
                else
                {
                    Service.PluginLog.Debug($"Delivery type {delivery.GetType().Name} is inactive and will not send.");
                }
            }
        }
    }
}
