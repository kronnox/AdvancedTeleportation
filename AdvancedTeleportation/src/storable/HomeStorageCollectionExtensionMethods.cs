using Asphalt.Storeable;
using Eco.Core.Serialization;
using Eco.Gameplay.Players;
using Eco.Shared.Math;
using System.Collections.Generic;

namespace AdvancedTeleportation.src.storable
{
    public static class HomeStorageCollectionExtensionMethods
    { 
        public static IDictionary<string, object> GetHomes(this IUserStorageCollection storage, User user)
        {          
            return storage.GetStorage(user).GetContent();  
        }

        public static Dictionary<string, float> GetHome(this IUserStorageCollection storage, User user, string home)
        {
            try
            {
                return (Dictionary<string, float>) storage.GetStorage(user).Get(home);
            }
            catch
            {
                return SerializationUtils.DeserializeJson<Dictionary<string, float>>(storage.GetStorage(user).Get(home).ToString());
            }
        }

        public static Vector3 GetPosition(this IUserStorageCollection storage, User user, string home)
        {
            Dictionary<string, float> values = GetHome(storage, user, home);
            Vector3 pos = new Vector3(values["x"], values["y"], values["z"]);
            return pos;
        }

        public static void SetPosition(this IUserStorageCollection storage, User user, string home, Vector3 pos)
        {
            Dictionary<string, float> homeDic = new Dictionary<string, float>();
            homeDic.Add("x", pos.X);
            homeDic.Add("y", pos.Y);
            homeDic.Add("z", pos.Z);

            storage.GetStorage(user).Set(home, homeDic);
        }

        public static void RemoveHome(this IUserStorageCollection storage, User user, string home)
        {
            storage.GetStorage(user).Remove(home);
        }
    }
}
