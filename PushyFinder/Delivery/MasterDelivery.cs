using System.Collections.Generic;

namespace PushyFinder.Delivery;

internal interface IDelivery
{
    public bool IsActive { get; }
    public void Deliver(string title, string text);
}

public static class MasterDelivery
{
    private static readonly List<IDelivery> Deliveries = new List<IDelivery>();

    static MasterDelivery()
    {
        Deliveries.Add(new PushoverDelivery());
        Deliveries.Add(new NtfyDelivery());
        Deliveries.Add(new DiscordDelivery());

        if (Plugin.Configuration.EnableTelegramBot)
        {
            var telegramDelivery = new TelegramDelivery();
            if (telegramDelivery.IsActive)
                Deliveries.Add(telegramDelivery);
        }
    }

    public static void Deliver(string title, string text)
    {
        foreach (var delivery in Deliveries)
            if (delivery.IsActive)
                delivery.Deliver(title, text);
    }
}
