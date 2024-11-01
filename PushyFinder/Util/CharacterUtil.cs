using DiscordRelay.Util; // Ensure this is included for any dependencies

namespace DiscordRelay.Util
{
    public static class CharacterUtil
    {
        public static bool IsClientAfk()
        {
            // Check if AFK status is ignored in configuration
            if (DiscordRelay.Configuration.IgnoreAfkStatus)
                return true;

            // Ensure the client is logged in and local player is available
            if (!Service.ClientState.IsLoggedIn ||
                Service.ClientState.LocalPlayer == null)
                return false;

            // 17 = AFK, 18 = Camera Mode (catches idle camera and gpose)
            return Service.ClientState.LocalPlayer.OnlineStatus.Id is 17 or 18;
        }
    }
}
