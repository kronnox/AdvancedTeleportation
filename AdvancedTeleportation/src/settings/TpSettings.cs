using Asphalt;
using Asphalt.Service.Settings;

namespace AdvancedTeleportation.Settings
{
    public class TpSettings : CustomSettingsFile
    {
        public TpSettings(AsphaltMod mod, string fileName) : base(mod, fileName) { }

        public TpSettings(AsphaltMod mod) : base(mod) { }

        public override string GetSettingsName()
        {
            return "teleportation";
        }

        public int GetHomeLimit()
        {
            return this.GetInt("home-limit");
        }

        public void SetHomeLimit(int limit)
        {
            this.SetInt("home-limit", limit);
            Store();
        }
    }
}
