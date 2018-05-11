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
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems.Chat;
    using Eco.Shared.Math;
    using System;
    using System.Collections.Generic;

    public class AdvTpCommands : IChatCommandHandler
    {

        [ChatCommand("advtp", "HomeCommands Base Command; usage: '/advtp help'")]
        public static void AdvtpCommand(User user, string arg0 = "")
        {
            switch (arg0)
            {
                case "help":
                    PrintHelp(user);
                    break;
                case "reload":
                    Reload(user);
                    break;
                case "convertoldfiles":
                    ConvertOldFiles(user);
                    break;
                default:
                    PrintHelp(user);
                    break;
            }
        }

        [ChatCommand("back", "Teleports you back to your last location before you got teleported.")]
        public static void BackCommand(User user)
        {
            if (!AdvancedTeleportationPlugin.PermissionService.CheckPermission(user, "back"))
                return;

            if (!AdvancedTeleportationPlugin.BackPos.ContainsKey(user.SlgId))
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("You haven't been teleported yet since the last server restart...");
                return;
            }

            Vector3 backPos = AdvancedTeleportationPlugin.BackPos[user.SlgId];

            user.Player.SetPosition(backPos);
            user.Player.SendTemporaryMessageAlreadyLocalized("Teleporting back...");
        }

        /**
         * Prints AdvTp help to the respective user
         */
        public static void PrintHelp(User user)
        {
            if (!AdvancedTeleportationPlugin.PermissionService.CheckPermission(user, "advtp.help"))
                return;

            //makeshift solution... TODO: Something more fancy
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--=[ AdvancedTeleportation Help ]=--</color></b>", user, false);
            if (AdvancedTeleportationPlugin.PermissionService.CheckPermission(user, "advtp.help"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp help</color> - <color=#DCDCDC>Shows help-page for HomeCommands</color>", user, false);
            if (AdvancedTeleportationPlugin.PermissionService.CheckPermission(user, "warp.help"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp help</color> - <color=#DCDCDC>Shows help-page for all warp commands</color>", user, false);
            if (AdvancedTeleportationPlugin.PermissionService.CheckPermission(user, "home.help"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home help</color> - <color=#DCDCDC>Shows help-page for all home commands</color>", user, false);
            if (AdvancedTeleportationPlugin.PermissionService.CheckPermission(user, "advtp.reload"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp reload</color> - <color=#DCDCDC>Reloads all config and storage files</color>", user, false);
            if(AdvancedTeleportationPlugin.PermissionService.CheckPermission(user, "back"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/back</color> - <color=#DCDCDC>Teleports you back to your last location before you got teleported</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--------------=[ page  1/1 ]=--------------</color></b>", user, false);

        }

        public static void Reload(User user)
        {
            if (!AdvancedTeleportationPlugin.PermissionService.CheckPermission(user, "advtp.reload"))
                return;

            user.Player.SendTemporaryMessageAlreadyLocalized("Reloading Permissions Service...");
            try
            {
                AdvancedTeleportationPlugin.PermissionService.Reload();
                user.Player.SendTemporaryMessageAlreadyLocalized("...Complete!");
            }
            catch (Exception e)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("...Failed!");
#if DEBUG
                throw e;
#endif
            }
            user.Player.SendTemporaryMessageAlreadyLocalized("Reloading User Settings...");
            try
            {
                AdvancedTeleportationPlugin.UserSettings.Reload();
                user.Player.SendTemporaryMessageAlreadyLocalized("...Complete!");
            }
            catch (Exception e)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("...Failed!");
#if DEBUG
                throw e;
#endif
            }
            user.Player.SendTemporaryMessageAlreadyLocalized("Reloading Homes...");
            try
            {
                AdvancedTeleportationPlugin.HomesStorage.Reload();
                user.Player.SendTemporaryMessageAlreadyLocalized("...Complete!");
            }
            catch (Exception e)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("...Failed!");
#if DEBUG
                throw e;
#endif
            }
            user.Player.SendTemporaryMessageAlreadyLocalized("Reloading Warps...");
            try
            {
                AdvancedTeleportationPlugin.WarpsStorage.Reload();
                user.Player.SendTemporaryMessageAlreadyLocalized("...Complete!");
            }
            catch (Exception e)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("...Failed!");
#if DEBUG
                throw e;
#endif
            }
        }

        public static void ConvertOldFiles(User user)
        {
            user.Player.SendTemporaryMessageAlreadyLocalized("Converting old warps...");
            int warpCount = 0;
            foreach(KeyValuePair<string, Dictionary<string, float>> warp in AdvancedTeleportationPlugin.OldWarpsStorage.warps)
            {
                if (AdvancedTeleportationPlugin.WarpsStorage.Get(warp.Key) != null)
                    continue;

                AdvancedTeleportationPlugin.WarpsStorage.Set(warp.Key, warp.Value);
                warpCount++;
            }
            user.Player.SendTemporaryMessageAlreadyLocalized($"... {warpCount} warps converted!");

            user.Player.SendTemporaryMessageAlreadyLocalized("Converting old homes...");
            int homeCount = 0;
            foreach (KeyValuePair<string, Dictionary<string, float>> home in AdvancedTeleportationPlugin.OldHomesStorage.homes)
            {
                string[] parts = home.Key.Split(new char[] { '-' }, 2);
                User l_user = UserManager.FindUserBySlgId(parts[0]);
                string name = "home";
                if (parts.Length > 1)
                    name = parts[1];

                if (AdvancedTeleportationPlugin.HomesStorage.GetStorage(user).Get(name) != null)
                    continue;

                AdvancedTeleportationPlugin.HomesStorage.GetStorage(user).Set(name, home.Value);
                homeCount++;
            }
            user.Player.SendTemporaryMessageAlreadyLocalized($"... {homeCount} homes converted!");
        }
    }
}