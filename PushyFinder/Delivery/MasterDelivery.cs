using System.Collections.Generic;
using Dalamud.Utility;

namespace DiscordRelay.Delivery
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

            // Only add Discord DM delivery if enabled and active
            if (DiscordRelay.Configuration.EnableDiscordBot) // Update to use DiscordRelay
            {
                var discordDelivery = new DiscordDMDelivery(); // Instantiate DiscordDMDelivery
                if (discordDelivery.IsActive)
                {
                    _deliveries.Add(discordDelivery);
                    Service.PluginLog.Debug("DiscordDMDelivery added to active deliveries.");
                }
                else
                {
                    Service.PluginLog.Debug("DiscordDMDelivery is not active and will not be added.");
                }
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
