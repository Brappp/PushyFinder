using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Dalamud.Utility;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace DiscordRelay.Delivery
{
    public class DiscordDMDelivery : IDelivery
    {
        private static readonly string BOT_SERVICE_URL = "https://relay.wahapp.com"; // Local testing URL

        public bool IsActive => DiscordRelay.Configuration.EnableDiscordBot &&
                                !string.IsNullOrWhiteSpace(DiscordRelay.Configuration.DiscordUserToken) &&
                                !string.IsNullOrWhiteSpace(DiscordRelay.Configuration.UserSecretKey);

        public void Deliver(string title, string text)
        {
            if (!IsActive)
            {
                Service.PluginLog.Error("Discord DM bot is not enabled, or user token/secret key is missing.");
                return;
            }

            Task.Run(() => DeliverAsync(title, text));
        }

        private async void DeliverAsync(string title, string text)
        {
            if (string.IsNullOrWhiteSpace(DiscordRelay.Configuration.DiscordUserToken) ||
                string.IsNullOrWhiteSpace(DiscordRelay.Configuration.UserSecretKey))
            {
                Service.PluginLog.Error("User is not fully configured with the Discord bot. Ensure both the token and secret key are set in the plugin.");
                return;
            }

            var apiUrl = $"{BOT_SERVICE_URL}/send";
            var nonce = Guid.NewGuid().ToString(); // Generate a unique nonce for this request

            var args = new Dictionary<string, string>
            {
                { "user_token", DiscordRelay.Configuration.DiscordUserToken },
                { "title", title },
                { "text", text },
                { "nonce", nonce },
                { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() } // Use current UTC timestamp
            };

            // Concatenate message data for hashing
            var messageData = $"{DiscordRelay.Configuration.DiscordUserToken}{title}{text}{nonce}{args["timestamp"]}";
            args["hash"] = GenerateHmacHash(messageData, DiscordRelay.Configuration.UserSecretKey);

            Service.PluginLog.Debug("Attempting to send data to Discord bot service...");
            try
            {
                await apiUrl.PostJsonAsync(args);
                Service.PluginLog.Debug("Data sent successfully to Discord bot service.");
            }
            catch (FlurlHttpException e)
            {
                Service.PluginLog.Error($"Failed to forward message to Discord bot service: '{e.Message}'");
                Service.PluginLog.Error($"Status: {e.StatusCode}, Response Body: {await e.GetResponseStringAsync()}");
            }
            catch (Exception e)
            {
                Service.PluginLog.Error($"Unexpected error: '{e.Message}'");
                Service.PluginLog.Error($"{e.StackTrace}");
            }
        }

        // HMAC hash generation with the user-specific secret key
        private string GenerateHmacHash(string message, string userSecretKey)
        {
            byte[] key = Encoding.UTF8.GetBytes(userSecretKey);
            using (var hmac = new HMACSHA256(key))
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public void StartListening()
        {
            Service.PluginLog.Debug("DiscordDMDelivery is now listening for requests.");
        }

        public void StopListening()
        {
            Service.PluginLog.Debug("DiscordDMDelivery has stopped listening.");
        }
    }
}
