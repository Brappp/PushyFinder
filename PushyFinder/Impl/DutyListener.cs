using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using DiscordRelay.Delivery; // Updated namespace
using DiscordRelay.Util; // Updated namespace

namespace DiscordRelay.Impl // Updated namespace
{
    public class DutyListener
    {
        public static void On()
        {
            Service.PluginLog.Debug("DutyListener On");
            Service.ClientState.CfPop += OnDutyPop;
        }

        public static void Off()
        {
            Service.PluginLog.Debug("DutyListener Off");
            Service.ClientState.CfPop -= OnDutyPop;
        }

        private static void OnDutyPop(ContentFinderCondition e)
        {
            if (!DiscordRelay.Configuration.EnableForDutyPops) // Updated reference
                return;

            if (!CharacterUtil.IsClientAfk())
                return;

            var dutyName = e.RowId == 0 ? "Duty Roulette" : e.Name.ToDalamudString().TextValue;
            MasterDelivery.Deliver("Duty pop", $"Duty registered: '{dutyName}'.");
        }
    }
}
