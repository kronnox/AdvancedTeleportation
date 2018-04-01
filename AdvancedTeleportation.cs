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
 * Created by Kronox on March 18, 2018
 * Version: 1.2.1
 * ------------------------------------
 **/

namespace Eco.Mods.Kronox
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems.Chat;
    using Eco.Mods.Kronox.config;
    using Eco.Mods.Kronox.util;
    using Eco.Shared.Math;

    public class AdvancedTeleportation : IChatCommandHandler
    {

        private static bool initialized = false;

        private static readonly string filePath = "Mods/Kronox/AdvancedTeleportation/";

        private static Warps warps = new Warps();
        private static Homes homes = new Homes();
        private static Dictionary<int, Vector3> oldHomes = new Dictionary<int, Vector3>();

        private static void Initialize()
        {
            if (!initialized)
            {
                warps = ClassSerializer<Warps>.Deserialize(filePath, "warps.json");
                if (File.Exists(filePath + "warps.txt"))
                {
                    foreach (var pair in OldFileHelper.ReadFromFile(filePath, "warps.txt").ToDictionary(item => item.Key, item => item.Value))
                        if(!warps.Exists(pair.Key))
                            warps.Add(pair.Key, pair.Value);

                    ClassSerializer<Warps>.Serialize(filePath, "warps.json", warps);
                    File.Delete(filePath + "warps.txt");
                }


                homes = ClassSerializer<Homes>.Deserialize(filePath, "homes.json");
                if (File.Exists(filePath + "homes.txt"))
                {
                    oldHomes = OldFileHelper.ReadFromFile(filePath, "homes.txt").ToDictionary(item => int.Parse(item.Key), item => item.Value);

                    if (oldHomes.Count <= 0)
                        File.Delete(filePath + "homes.txt");
                }
                
                initialized = true;
            }
        }

        //-------------------------------
        // ADVANCEDTELEPORTATION SECTION
        //-------------------------------

        [ChatCommand("advtp", "AdvancedTeleportation Base Command; usage: '/advtp help'", ChatAuthorizationLevel.Admin)]
        public static void AdvtpCommand(User user, string arg0 = "")
        {
            switch (arg0)
            {
                case "help":
                    PrintHelp(user);
                    break;
                case "reloadconfig":
                    ReloadConfig(user);
                    break;
                default:
                    PrintHelp(user);
                    break;
            }
        }

        /**
         * Prints AdvancedTeleportation help to the respective user
         */
        public static void PrintHelp(User user)
        {
            //makeshift solution... TODO: Something more fancy
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--=[ AdvancedTeleportation Help ]=--</color></b>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp help</color> - <color=#DCDCDC>Shows help-page for AdvancedTeleportation</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp help</color> - <color=#DCDCDC>Shows help-page for all warp commands</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home help</color> - <color=#DCDCDC>Shows help-page for all home commands</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp reloadconfig</color> - <color=#DCDCDC>Reloads the configuration-file</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--------------=[ page  1/1 ]=--------------</color></b>", user, false);

        }

        public static void ReloadConfig(User user)
        {
            user.Player.SendTemporaryMessageAlreadyLocalized("Reloading AdvancedTeleportation config file...");
            ConfigManager.Instance.LoadConfigFile();
            user.Player.SendTemporaryMessageAlreadyLocalized("...Complete!");
        }

        //--------------
        // WARP SECTION
        //--------------

        [ChatCommand("warp", "Warp Base Command; usage: '/warp help'", ChatAuthorizationLevel.User)]
        public static void WarpCommand(User user, string arg0 = "", string arg1 = "")
        {
            arg0 = arg0.ToLower();
            arg1 = arg1.ToLower();

            Initialize();

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

        [ChatCommand("setwarp", "Sets a new warp point or overrides an existing one", ChatAuthorizationLevel.Admin)]
        public static void SetWarpCommand(User user, string name)
        {
            Initialize();
            name = name.ToLower();
            SetWarp(user, name);
        }

        [ChatCommand("removewarp", "Removes an existing warp point", ChatAuthorizationLevel.Admin)]
        public static void RemoveWarpCommand(User user, string name)
        {
            Initialize();
            name = name.ToLower();
            RemoveWarp(user, name);
        }

        [ChatCommand("warps", "Lists all existing warps", ChatAuthorizationLevel.User)]
        public static void WarpsCommand(User user)
        {
            Initialize();
            ListWarps(user);
        }

        /**
         * Prints Warp-Command help to the respective user
         */
        public static void PrintWarpHelp(User user)
        {
            //makeshift solution... TODO: Something more fancy
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--=[ AdvancedTeleportation - Warp Help ]=--</color></b>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp help</color> - <color=#DCDCDC>Shows help-page for AdvancedTeleportation</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp help</color> - <color=#DCDCDC>Shows help-page for all warp commands</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp <name></color> - <color=#DCDCDC>Teleports you to a warp point</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=red>/setwarp <name></color> - <color=#DCDCDC>Sets a new warp point or overrides an existing one</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=red>/warp remove,<name></color> - <color=#DCDCDC>Removes an existing warp point</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/warp list</color> - <color=#DCDCDC>Lists all existing warps</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>-----------------=[ page  1/1 ]=-----------------</color></b>", user, false);

        }

        /**
         * Teleports a user to the respective war´p point
         */
        public static void TeleportWarp(User user, string name)
        {
            if (!warps.Exists(name))
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("'" + name + "' isn't a known warp!");
                return;
            }
            user.Player.SetPosition(warps.Get(name));
            user.Player.SendTemporaryMessageAlreadyLocalized("Warping to '" + name + "'...");
        }

        /**
        * Adds a new warp or overrides an exsting one
        */
        public static void SetWarp(User user, string name)
        {
            if(!user.IsAdmin)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("You don't have the permission to do that! Rank needed: Admin");
            }

            warps.Add(name, user.Player.Position);
            ClassSerializer<Warps>.Serialize(filePath, "warps.json", warps);
            user.Player.SendTemporaryMessageAlreadyLocalized("Warp '" + name + "' has been sucessfully set to '" + user.Player.Position + "'!");
        }

        /**
         * Removes a warp point
         */
        public static void RemoveWarp(User user, string name)
        {
            if (!user.IsAdmin)
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("You don't have the permission to do that! Rank needed: Admin");
            }

            if (!warps.Exists(name))
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("'" + name + "' isn't a known warp!");
                return;
            }

            warps.Remove(name);
            ClassSerializer<Warps>.Serialize(filePath, "warps.json", warps);
            user.Player.SendTemporaryMessageAlreadyLocalized("Warp '" + name + "' has been sucessfully removed!");
        }

        /**
         * Prints all existing warps to the respective user
         */
        public static void ListWarps(User user)
        {
            string results = "";

            if (warps.IsEmpty())
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("No warps have been set yet!");
                return;
            }

            user.Player.SendTemporaryMessageAlreadyLocalized("Existing warps:");
            bool first = true;
            foreach (KeyValuePair<string, Dictionary<string, float>> pair in warps.warps)
            {
                if (!first) results += ", ";
                results += pair.Key;
                first = false;
            }
            user.Player.SendTemporaryMessageAlreadyLocalized(results);
        }

        //---------------
        // HOMES SECTION
        //---------------

        [ChatCommand("home", "Teleports you to your home", ChatAuthorizationLevel.User)]
        public static void HomeCommand(User user, string arg0 = "", string arg1 = "")
        {
            arg0 = arg0.ToLower();
            arg1 = arg1.ToLower();

            Initialize();
            ConvertHomes(user);

            switch (arg0)
            {
                case "help":
                    //Prints Home help-page
                    PrintHomeHelp(user);
                    break;
                case "set":
                    //Sets a new home or overrides an existing one
                    SetHome(user, arg1);
                    break;
                case "tp":
                    //Teleports the player to the respective home point
                    TeleportHome(user, arg1);
                    break;
                case "remove":
                    //Removes an existing home
                    RemoveHome(user, arg1);
                    break;
                case "delete":
                    //Removes an existing home
                    RemoveHome(user, arg1);
                    break;
                case "list":
                    //Lists all existing homes for the user
                    ListHomes(user);
                    break;
                default:
                    //Teleports the player to the respective home point
                    TeleportHome(user, arg0);
                    break;
            }
        }

        [ChatCommand("sethome", "Sets your personal home to your current position", ChatAuthorizationLevel.User)]
        public static void SetHomeCommand(User user, string name = "")
        {
            Initialize();
            name = name.ToLower();
            SetHome(user, name);
        }

        /**
         * Prints Warp-Command help to the respective user
         */
        public static void PrintHomeHelp(User user)
        {
            //makeshift solution... TODO: Something more fancy
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>--=[ AdvancedTeleportation - Home Help ]=--</color></b>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/advtp help</color> - <color=#DCDCDC>Shows help-page for AdvancedTeleportation</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home help</color> - <color=#DCDCDC>Shows help-page for all home commands</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home [name]</color> - <color=#DCDCDC>Teleports you to your home</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/sethome <name></color> - <color=#DCDCDC>Sets a new home or overrides an existing one</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home remove,<name></color> - <color=#DCDCDC>Removes an existing home</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<color=white>/home list</color> - <color=#DCDCDC>Lists all of your homes</color>", user, false);
            ChatManager.ServerMessageToPlayerAlreadyLocalized("<b><color=white>-----------------=[ page  1/1 ]=-----------------</color></b>", user, false);
        }

        /**
         *  Converts all old home points of the user to the new file system
         */
        public static void ConvertHomes(User user)
        {
            if (oldHomes.ContainsKey(user.Player.ID))
            {
                if (!homes.Exists(user.SlgId))
                {
                    homes.Add(user.SlgId, oldHomes[user.Player.ID]);
                    ClassSerializer<Homes>.Serialize(filePath, "homes.json", homes);
                }

                oldHomes.Remove(user.Player.ID);
                OldFileHelper.SaveToFile(oldHomes.ToDictionary(item => item.Key.ToString(), item => item.Value), filePath, "homes.txt");

                if (oldHomes.Count <= 0)
                    File.Delete(filePath + "homes.txt");
            }
        }

        /**
         *  Teleports the player to the respective home point
         *  # default home: name = "home"
         */
        public static void TeleportHome(User user, string id)
        {
            string name = "home";
            if (id.Equals("home"))
                id = "";
            if (!id.Equals(""))
            {
                name = id;
                id = "-" + id;
            }

            //home not found
            if (!homes.Exists(user.SlgId + id))
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("This home wasn't set yet!");
                return;
            }

            //teleportation
            user.Player.SetPosition(homes.Get(user.SlgId + id));
            user.Player.SendTemporaryMessageAlreadyLocalized("Returning to '" + name + "'...");
        }

        /**
         *  Sets the home point '@name' for the respective user
         *  # default home: name = "home"
         */
        public static void SetHome(User user, string id)
        {
            if(!user.IsAdmin && !ConfigManager.Instance.GetInt("home-limit").Equals(-1) && homes.GetHomesForSLGID(user.SlgId).Count >= ConfigManager.Instance.GetInt("home-limit"))
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("You have reached the limit of '" + ConfigManager.Instance.GetInt("home-limit") + "' homes! Delete one first ('/home remove,<name>').");
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

            homes.Add(user.SlgId + id, user.Player.Position);
            ClassSerializer<Homes>.Serialize(filePath, "homes.json", homes);
            user.Player.SendTemporaryMessageAlreadyLocalized("Your home '" + name + "' has been successfully set to '" + user.Player.Position + "'!");
        }

        /**
         *  Removes the home point '@name' for the respective user
         *  # default home: name = "home"
         */
        public static void RemoveHome(User user, string id)
        {
            string name = "home";
            if (id.Equals("home"))
                id = "";
            if (!id.Equals(""))
            {
                name = id;
                id = "-" + id;
            }

            //home not found
            if (!homes.Exists(user.SlgId + id))
            {
                user.Player.SendTemporaryErrorAlreadyLocalized("This home wasn't set yet!");
                return;
            }

            homes.Remove(user.SlgId + id);
            ClassSerializer<Homes>.Serialize(filePath, "homes.json", homes);
            user.Player.SendTemporaryMessageAlreadyLocalized("Your home '" + name + "' has been successfully removed!");
        }

        /**
         *  Lists all existing homepoints for the user
         */
        public static void ListHomes(User user)
        {
            Dictionary<string, Dictionary<string, float>> userHomes = homes.GetHomesForSLGID(user.SlgId);

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

        //-------------------
        // WARP SIGN SECTION
        //-------------------

        public static void CallWarpSign(Player player, String text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            string[] lines = text.Split(new[] { "<br>" }, StringSplitOptions.None);

            if (lines.Length < 2 || !lines[0].Equals("[WARP]") || lines[1].Contains(' '))
                return;

            Eco.Mods.Kronox.AdvancedTeleportation.TeleportWarp(player.User, lines[1]);
        }
    }

    public class Warps
    {
        public Dictionary<string, Dictionary<string, float>> warps = new Dictionary<string, Dictionary<string, float>>();

        public Warps()
        {

        }

        public void Add(string name, Vector3 pos)
        {
            if (this.Exists(name))
                warps.Remove(name);

            Dictionary<string, float> sPos = new Dictionary<string, float>();
            sPos.Add("x", pos.X);
            sPos.Add("y", pos.Y);
            sPos.Add("z", pos.Z);

            warps.Add(name, sPos);
        }

        public void Remove(string name)
        {
            warps.Remove(name);
        }

        public Vector3 Get(string name)
        {
            return new Vector3(warps[name]["x"], warps[name]["y"], warps[name]["z"]);
        }

        public bool Exists(string name)
        {
            return warps.ContainsKey(name);
        }

        public bool IsEmpty()
        {
            return warps.Count <= 0;
        }
    }


    public class Homes
    {
        public Dictionary<string, Dictionary<string, float>> homes = new Dictionary<string, Dictionary<string, float>>();

        public Homes()
        {

        }

        public void Add(string name, Vector3 pos)
        {
            if (this.Exists(name))
                homes.Remove(name);

            Dictionary<string, float> sPos = new Dictionary<string, float>();
            sPos.Add("x", pos.X);
            sPos.Add("y", pos.Y);
            sPos.Add("z", pos.Z);

            homes.Add(name, sPos);
        }

        public void Remove(string name)
        {
            homes.Remove(name);
        }

        public Vector3 Get(string name)
        {
            return new Vector3(homes[name]["x"], homes[name]["y"], homes[name]["z"]);
        }

        public bool Exists(string name)
        {
            return homes.ContainsKey(name);
        }

        public bool IsEmpty()
        {
            return homes.Count <= 0;
        }

        public Dictionary<string, Dictionary<string, float>> GetHomesForSLGID(string id)
        {
            return homes.Where(x => x.Key.StartsWith(id)).ToDictionary(x => x.Key, x => x.Value);
        }
    }


    public static class OldFileHelper
    {

        public static void SaveToFile(Dictionary<string, Vector3> dic, string filePath, string fileName)
        {
            if (dic.Count < 1)
                return;

            //clear content
            File.WriteAllText(filePath+fileName, "");

            //Serealize Dicctionary
            using (StreamWriter file = File.AppendText(filePath + fileName))
            {

                foreach (KeyValuePair<string, Vector3> pair in dic)
                {
                    file.WriteLine(
                        String.Format(
                            "{0}, {1}, {2}, {3}",
                            pair.Key,
                            pair.Value.x.ToString(),
                            pair.Value.y.ToString(),
                            pair.Value.z.ToString()
                        )
                    );
                }
            }
        }

        public static Dictionary<string, Vector3> ReadFromFile(string filePath, string fileName)
        {
            Dictionary<string, Vector3> dic = new Dictionary<string, Vector3>();

            //Create Directory and File if needed
            Directory.CreateDirectory(filePath);
            FileStream fs = new FileStream(filePath+fileName, FileMode.OpenOrCreate);
            fs.Close();

            //Deserialize content
            var lines = File.ReadLines(filePath+fileName);
            foreach (var line in lines)
            {
                if (line.Length < 1) continue;

                string[] data = line.Split(new[] { ", " }, StringSplitOptions.None);
                Vector3 vec = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                dic.Add(data[0], vec);
            }

            return dic;
        }
    }
}