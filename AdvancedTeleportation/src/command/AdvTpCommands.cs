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

    public class AdvTpCommands : IChatCommandHandler
    {

        [ChatCommand("advtp", "HomeCommands Base Command; usage: '/advtp help'", ChatAuthorizationLevel.Admin)]
        public static void AdvtpCommand(User user, string arg0 = "")
        {
            switch (arg0)
            {
                case "help":
                    PrintHelp(user);
                    break;
                case "reloadsettings":
                    ReloadSettings(user);
                    break;
                default:
                    PrintHelp(user);
                    break;
            }
        }

        [ChatCommand("back", "Teleports you back to your last location before you got teleported.", ChatAuthorizationLevel.Admin)]
        public static void BackCommand(User user)
        {
            if (!AdvancedTeleportationPlugin.Mod.GetPermissionsService().CheckPermission(user, "back"))
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
            if (!AdvancedTeleportationPlugin.Mod.GetPermissionsService().CheckPermission(user, "advtp.help"))
                return;

            //makeshift solution... TODO: Something more fancy
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--=[ AdvancedTeleportation Help ]=--</color></b>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp help</color> - <color=#DCDCDC>Shows help-page for HomeCommands</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp help</color> - <color=#DCDCDC>Shows help-page for all warp commands</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home help</color> - <color=#DCDCDC>Shows help-page for all home commands</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp reloadsettings</color> - <color=#DCDCDC>Reloads all settings-files</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/back</color> - <color=#DCDCDC>Teleports you back to your last location before you got teleported</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--------------=[ page  1/1 ]=--------------</color></b>", user, false);

        }

        public static void ReloadSettings(User user)
        {
            if (!AdvancedTeleportationPlugin.Mod.GetPermissionsService().CheckPermission(user, "advtp.reloadconfig"))
                return;

            user.Player.SendTemporaryMessageAlreadyLocalized("Reloading settings files...");
            AdvancedTeleportationPlugin.Mod.GetSettingsService().Reload();
            user.Player.SendTemporaryMessageAlreadyLocalized("...Complete!");
        }
    }
}