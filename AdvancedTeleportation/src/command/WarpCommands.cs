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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AdvancedTeleportation.src.storable;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems.Chat;

    public class WarpCommands : IChatCommandHandler
    {
        [ChatCommand("warp", "Warp Base Command; usage: '/warp help'")]
        public static void WarpCommand(User user, string arg0 = "", string arg1 = "")
        {
            arg0 = arg0.ToLower();
            arg1 = arg1.ToLower();

            switch (arg0)
            {
                case "help":
                    //Print Warp help-page
                    PrintWarpHelp(user);
                    break;
                case "set":
                    //Sets a new warp or overrides an existing one
                    SetWarp(user, arg1);
                    break;
                case "tp":
                    if (!AdvancedTeleportationPlugin.Instance.PermissionService.CheckPermission(user, "warp.teleport.cmd"))
                        return;

                    //Teleports the player to the respective warp point
                    TeleportWarp(user, arg1);
                    break;
                case "remove":
                    //Removes an existing warp
                    RemoveWarp(user, arg1);
                    break;
                case "delete":
                    //Removes an existing warp
                    RemoveWarp(user, arg1);
                    break;
                case "list":
                    //Lists all existing homes for the user
                    ListWarps(user);
                    break;
                default:
                    //Teleports the player to the respective warp point
                    TeleportWarp(user, arg0);
                    break;
            }
        }

        [ChatCommand("setwarp", "Sets a new warp point or overrides an existing one")]
        public static void SetWarpCommand(User user, string name)
        {
            name = name.ToLower();
            SetWarp(user, name);
        }

        [ChatCommand("removewarp", "Removes an existing warp point")]
        public static void RemoveWarpCommand(User user, string name)
        {
            name = name.ToLower();
            RemoveWarp(user, name);
        }

        [ChatCommand("warps", "Lists all existing warps")]
        public static void WarpsCommand(User user)
        {
            ListWarps(user);
        }

        /**
         * Prints Warp-Command help to the respective user
         */
        public static void PrintWarpHelp(User user)
        {
            if (!AdvancedTeleportationPlugin.Instance.PermissionService.CheckPermission(user, "warp.help"))
                return;

            //makeshift solution... TODO: Something more fancy
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--=[ HomeCommands - Warp Help ]=--</color></b>", user, false);
            if(AdvancedTeleportationPlugin.Instance.PermissionService.HasPermission(user, "advtp.help"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp help</color> - <color=#DCDCDC>Shows help-page for Advanced Teleportation Commands</color>", user, false);
            if (AdvancedTeleportationPlugin.Instance.PermissionService.HasPermission(user, "warp.help"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp help</color> - <color=#DCDCDC>Shows help-page for all warp commands</color>", user, false);
            if (AdvancedTeleportationPlugin.Instance.PermissionService.HasPermission(user, "warp.teleprt.cmd"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp <name></color> - <color=#DCDCDC>Teleports you to a warp point</color>", user, false);
            if (AdvancedTeleportationPlugin.Instance.PermissionService.HasPermission(user, "warp.set"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/setwarp <name></color> - <color=#DCDCDC>Sets a new warp point or overrides an existing one</color>", user, false);
            if (AdvancedTeleportationPlugin.Instance.PermissionService.HasPermission(user, "warp.remove"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp remove,<name></color> - <color=#DCDCDC>Removes an existing warp point</color>", user, false);
            if (AdvancedTeleportationPlugin.Instance.PermissionService.HasPermission(user, "warp.list"))
                ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp list</color> - <color=#DCDCDC>Lists all existing warps</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>-----------------=[ page  1/1 ]=-----------------</color></b>", user, false);

        }

        /**
         * Teleports a user to the respective war´p point
         */
        public static void TeleportWarp(User user, string name)
        {
            if (AdvancedTeleportationPlugin.Instance.WarpsStorage.GetWarp(name) == null)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("'" + name + "' isn't a known warp!");
                return;
            }
            user.Player.SetPosition(AdvancedTeleportationPlugin.Instance.WarpsStorage.GetPosition(name));
            user.Player.SendTemporaryMessageAlreadyLocalized("Warping to '" + name + "'...");
        }

        /**
        * Adds a new warp or overrides an exsting one
        */
        public static void SetWarp(User user, string name)
        {
            if (!AdvancedTeleportationPlugin.Instance.PermissionService.CheckPermission(user, "warp.set"))
                return;

            AdvancedTeleportationPlugin.Instance.WarpsStorage.SetPosition(name, user.Player.Position);
            user.Player.SendTemporaryMessageAlreadyLocalized("Warp '" + name + "' has been sucessfully set to '" + user.Player.Position + "'!");
        }

        /**
         * Removes a warp point
         */
        public static void RemoveWarp(User user, string name)
        {
            if (!AdvancedTeleportationPlugin.Instance.PermissionService.CheckPermission(user, "warp.remove"))
                return;

            if (AdvancedTeleportationPlugin.Instance.WarpsStorage.GetWarp(name) == null)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("'" + name + "' isn't a known warp!");
                return;
            }

            AdvancedTeleportationPlugin.Instance.WarpsStorage.Remove(name);
            user.Player.SendTemporaryMessageAlreadyLocalized("Warp '" + name + "' has been sucessfully removed!");
        }

        /**
         * Prints all existing warps to the respective user
         */
        public static void ListWarps(User user)
        {
            if (!AdvancedTeleportationPlugin.Instance.PermissionService.CheckPermission(user, "warp.list"))
                return;

            string results = "";

            if (AdvancedTeleportationPlugin.Instance.WarpsStorage.GetContent().Count < 1)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("No warps have been set yet!");
                return;
            }

            IDictionary<string, object> warps = AdvancedTeleportationPlugin.Instance.WarpsStorage.GetContent();

            user.Player.SendTemporaryMessageAlreadyLocalized("Existing warps:");
            bool first = true;
            foreach (KeyValuePair<string, object> pair in AdvancedTeleportationPlugin.Instance.WarpsStorage.GetContent())
            {
                if (!first) results += ", ";
                results += pair.Key;
                first = false;
            }
            user.Player.SendTemporaryMessageAlreadyLocalized(results);
        }

        //-------------------
        // WARP SIGN SECTION
        //-------------------

        public static void CallWarpSign(Player player, String text)
        {
            if (!AdvancedTeleportationPlugin.Instance.PermissionService.CheckPermission(player.User, "warp.teleport.sign"))
                return;

            if (string.IsNullOrWhiteSpace(text))
                return;

            string[] lines = text.Split(new[] { "<br>" }, StringSplitOptions.None);

            if (lines.Length < 2 || !lines[0].Equals("[WARP]") || lines[1].Contains(' '))
                return;

            TeleportWarp(player.User, lines[1]);
        }
    }
}