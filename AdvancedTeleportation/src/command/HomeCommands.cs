/** 
 * Copyright (c) 2018 [Kronox]
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * ------------------------------------
 * Created by Kronox on April 14, 2018
 * ------------------------------------
 **/

namespace AdvancedTeleportation.command
{
    using System.Collections.Generic;
    using AdvancedTeleportation.Settings;
    using AdvancedTeleportation.storable;
    using Asphalt.Api.Util;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems.Chat;
    using Eco.Shared.Utils;

    public class HomeCommands : IChatCommandHandler
    {

        [ChatCommand("home", "Teleports you to your home", ChatAuthorizationLevel.User)]
        public static void HomeCommand(User user, string arg0 = "", string arg1 = "", string arg2 = "")
        {
            string arg0_l = arg0.ToLower();
            string arg1_l = arg1.ToLower();
            string arg2_l = arg2.ToLower();

            switch (arg0)
            {
                case "help":
                    //Prints Home help-page
                    PrintHomeHelp(user);
                    break;
                case "set":
                    //Sets a new home or overrides an existing one
                    SetHome(user, arg1_l);
                    break;
                case "tp":
                    //Teleports the player to the respective home point
                    TeleportHome(user, arg1_l);
                    break;
                case "remove":
                    //Removes an existing home
                    RemoveHome(user, arg1_l);
                    break;
                case "delete":
                    //Removes an existing home
                    RemoveHome(user, arg1_l);
                    break;
                case "list":
                    //Lists all existing homes for the user
                    ListHomes(user);
                    break;
                case "setlimit":
                    //Lists all existing homes for the user
                    SetLimit(user, arg1, arg2_l);
                    break;
                default:
                    //Teleports the player to the respective home point
                    TeleportHome(user, arg0_l);
                    break;
            }
        }

        [ChatCommand("sethome", "Sets your personal home to your current position", ChatAuthorizationLevel.User)]
        public static void SetHomeCommand(User user, string name = "")
        {
            name = name.ToLower();
            SetHome(user, name);
        }

        /**
         * Prints Warp-Command help to the respective user
         */
        public static void PrintHomeHelp(User user)
        {
            if (!AdvancedTeleportationPlugin.Mod.GetPermissionsService().CheckPermission(user, "home.help"))
                return;

            //makeshift solution... TODO: Something more fancy
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--=[ HomeCommands - Home Help ]=--</color></b>", user, false);
            if (AdvancedTeleportationPlugin.Mod.GetPermissionsService().HasPermission(user, "advtp.help"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp help</color> - <color=#DCDCDC>Shows help-page for HomeCommands</color>", user, false);
            if (AdvancedTeleportationPlugin.Mod.GetPermissionsService().HasPermission(user, "home.help"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home help</color> - <color=#DCDCDC>Shows help-page for all home commands</color>", user, false);
            if (AdvancedTeleportationPlugin.Mod.GetPermissionsService().HasPermission(user, "home.teleport"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home [name]</color> - <color=#DCDCDC>Teleports you to your home</color>", user, false);
            if (AdvancedTeleportationPlugin.Mod.GetPermissionsService().HasPermission(user, "home.set"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/sethome <name></color> - <color=#DCDCDC>Sets a new home or overrides an existing one</color>", user, false);
            if (AdvancedTeleportationPlugin.Mod.GetPermissionsService().HasPermission(user, "home.remove"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home remove,<name></color> - <color=#DCDCDC>Removes an existing home</color>", user, false);
            if (AdvancedTeleportationPlugin.Mod.GetPermissionsService().HasPermission(user, "home.list"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home list</color> - <color=#DCDCDC>Lists all of your homes</color>", user, false);
            if (AdvancedTeleportationPlugin.Mod.GetPermissionsService().HasPermission(user, "home.setlimit"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home setlimit,<Player>,<amount></color> - <color=#DCDCDC>Lists all of your homes</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>-----------------=[ page  1/1 ]=-----------------</color></b>", user, false);
        }

        /**
         *  Teleports the player to the respective home point
         *  # default home: name = "home"
         */
        public static void TeleportHome(User user, string id)
        {
            if (!AdvancedTeleportationPlugin.Mod.GetPermissionsService().CheckPermission(user, "home.teleport"))
                return;

            string name = "home";
            if (id.Equals("home"))
                id = "";
            if (!id.Equals(""))
            {
                name = id;
                id = "-" + id;
            }

            //home not found
            if (!AdvancedTeleportationPlugin.Homes.Exists(user.SlgId + id))
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("This home wasn't set yet!");
                return;
            }

            //teleportation
            user.Player.SetPosition(AdvancedTeleportationPlugin.Homes.Get(user.SlgId + id));
            user.Player.SendTemporaryMessageAlreadyLocalized("Returning to '" + name + "'...");
        }

        /**
         *  Sets the home point '@name' for the respective user
         *  # default home: name = "home"
         */
        public static void SetHome(User user, string id)
        {
            if (!AdvancedTeleportationPlugin.Mod.GetPermissionsService().CheckPermission(user, "home.set"))
                return;

            int limit = ((TpSettings)AdvancedTeleportationPlugin.Mod.GetSettingsService().GetReadOnlySettings(typeof(TpSettings), user)).GetHomeLimit();
            Log.WriteLine("Limit:"+limit);
            if(!user.IsAdmin && limit != -1 && AdvancedTeleportationPlugin.Homes.GetHomesForSLGID(user.SlgId).Count >= limit)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("You have reached the limit of '" + limit + "' homes! Delete one first ('/home remove,<name>').");
                return;
            }

            string name = "home";
            if (id.Equals("home"))
                id = "";
            if (!id.Equals(""))
            {
                name = id;
                id = "-" + id;
            }

            AdvancedTeleportationPlugin.Homes.Add(user.SlgId + id, user.Player.Position);
            ClassSerializer<Homes>.Serialize(AdvancedTeleportationPlugin.filePath, "homes.json", AdvancedTeleportationPlugin.Homes);
            user.Player.SendTemporaryMessageAlreadyLocalized("Your home '" + name + "' has been successfully set to '" + user.Player.Position + "'!");
        }

        /**
         *  Sets the home limit for the respective user
         *  # default limit: 5
         */
        public static void SetLimit(User user, string name, string sLimit)
        {
            if (!AdvancedTeleportationPlugin.Mod.GetPermissionsService().CheckPermission(user, "home.setlimit"))
                return;

            User target = UserManager.FindUserByName(name);
            if (target == null)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("The User '" + name + "' couldn't be found!");
                return;
            }

            int limit;

            try
            {
                limit = int.Parse(sLimit);
            }
            catch
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("'" + sLimit + "' is not a valid home-limit! (-1 -> no limit)");
                return;
            }

            ((TpSettings)AdvancedTeleportationPlugin.Mod.GetSettingsService().GetSettings(typeof(TpSettings), target)).SetHomeLimit(limit);
            
            user.Player.SendTemporaryMessageAlreadyLocalized("" + name + "'s home limit was sucessfully set to '" + sLimit + "'!");
        }

        /**
         *  Removes the home point '@name' for the respective user
         *  # default home: name = "home"
         */
        public static void RemoveHome(User user, string id)
        {
            if (!AdvancedTeleportationPlugin.Mod.GetPermissionsService().CheckPermission(user, "home.remove"))
                return;

            string name = "home";
            if (id.Equals("home"))
                id = "";
            if (!id.Equals(""))
            {
                name = id;
                id = "-" + id;
            }

            //home not found
            if (!AdvancedTeleportationPlugin.Homes.Exists(user.SlgId + id))
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("This home wasn't set yet!");
                return;
            }

            AdvancedTeleportationPlugin.Homes.Remove(user.SlgId + id);
            ClassSerializer<Homes>.Serialize(AdvancedTeleportationPlugin.filePath, "homes.json", AdvancedTeleportationPlugin.Homes);
            user.Player.SendTemporaryMessageAlreadyLocalized("Your home '" + name + "' has been successfully removed!");
        }

        /**
         *  Lists all existing homepoints for the user
         */
        public static void ListHomes(User user)
        {
            if (!AdvancedTeleportationPlugin.Mod.GetPermissionsService().CheckPermission(user, "home.list"))
                return;

            Dictionary<string, Dictionary<string, float>> userHomes = AdvancedTeleportationPlugin.Homes.GetHomesForSLGID(user.SlgId);

            //no homes set yet
            if (userHomes.Count < 1)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("You haven't set any homes yet!");
                return;
            }

            //list header
            user.Player.SendTemporaryMessageAlreadyLocalized(user.Name + "'s homes:");

            //list body
            string results = "";
            bool first = true;
            foreach (KeyValuePair<string, Dictionary<string, float>> pair in userHomes)
            {
                if (!first) results += ", ";

                if (pair.Key.Equals(user.SlgId))
                    results += "home";
                else
                    results += pair.Key.Replace(user.SlgId + "-", "");

                first = false;
            }
            user.Player.SendTemporaryMessageAlreadyLocalized(results);
        }
    }
}