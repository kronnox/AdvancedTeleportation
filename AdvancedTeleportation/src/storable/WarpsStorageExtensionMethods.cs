using Eco.Core.Serialization;
using Eco.Shared.Math;
using System.Collections.Generic;

namespace AdvancedTeleportation.src.storable
{
    public static class WarpsStorageExtensionMethods
    {
        public static Dictionary<string, float> GetWarp(this Asphalt.Storeable.IStorage storage, string warp)
        {
            try
            {
                return (Dictionary<string, float>)storage.Get(warp);
            }
            catch
            {
                return SerializationUtils.DeserializeJson<Dictionary<string, float>>(storage.Get(warp).ToString());
            }
        }

        public static Vector3 GetPosition(this Asphalt.Storeable.IStorage storage, string warp)
        {
            Dictionary<string, float> values = GetWarp(storage, warp);
            Vector3 pos = new Vector3(values["x"], values["y"], values["z"]);
            return pos;
        }

        public static void SetPosition(this Asphalt.Storeable.IStorage storage, string warp, Vector3 pos)
        {
            Dictionary<string, float> warpDic = new Dictionary<string, float>();
            warpDic.Add("x", pos.X);
            warpDic.Add("y", pos.Y);
            warpDic.Add("z", pos.Z);

            storage.Set(warp, warpDic);
        }
    }
}
