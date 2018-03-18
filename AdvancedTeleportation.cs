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
 * Version: 1.0.3
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
    using Eco.Shared.Math;

    public class AdvancedTeleportation : IChatCommandHandler
    {

        private static bool initialized = false;

        private static Dictionary<string, Vector3> warps = new Dictionary<string, Vector3>();
        private static Dictionary<int, Vector3> homes = new Dictionary<int, Vector3>();

        static private void Initialize()
        {
            if (!initialized)
            {
                warps = FileHelper.ReadFromFile("warps.txt");
                homes = FileHelper.ReadFromFile("homes.txt").ToDictionary(item => int.Parse(item.Key), item => item.Value);
                initialized = true;
            }
        }

        [ChatCommand("Sets a new warp point or overrides an existing one", ChatAuthorizationLevel.Admin)]
        public static void SetWarp(User user, string name)
        {
            Initialize();

            name = name.ToLower();

            if (warps.ContainsKey(name))
                warps.Remove(name);
            warps.Add(name, user.Player.Position);
            FileHelper.SaveToFile(warps, "warps.txt");
            user.Player.SendTemporaryMessage("Warp '" + name + "' has been sucessfully set to '" + user.Player.Position + "'!");
        }

        [ChatCommand("Removes an existing warp point", ChatAuthorizationLevel.Admin)]
        public static void RemoveWarp(User user, string name)
        {
            Initialize();

            name = name.ToLower();

            if (!warps.ContainsKey(name))
            {
                user.Player.SendTemporaryError("'" + name + "' isn't a known warp!");
                return;
            }
            warps.Remove(name);
            FileHelper.SaveToFile(warps, "warps.txt");
            user.Player.SendTemporaryMessage("Warp '" + name + "' has been sucessfully removes!");
        }

        [ChatCommand("Teleports you to a warp point", ChatAuthorizationLevel.User)]
        public static void Warp(User user, string name)
        {
            Initialize();

            name = name.ToLower();

            if (!warps.ContainsKey(name))
            {
                user.Player.SendTemporaryError("'" + name + "' isn't a known warp!");
                return;
            }
            user.Player.SetPosition(warps[name]);
            user.Player.SendTemporaryMessage("Warping to '" + name + "'...");
        }

        [ChatCommand("Lists all existing warps", ChatAuthorizationLevel.User)]
        public static void Warps(User user)
        {
            Initialize();

            String results = "";

            if (warps.Count < 1)
            {
                user.Player.SendTemporaryError("No warps have been set yet!");
                return;
            }

            user.Player.SendTemporaryMessage("Existing warps:");
            foreach (KeyValuePair<string, Vector3> pair in warps)
            {
                if (warps.Count > 1 && results.Length > 0) results += ", ";
                results += pair.Key;
            }
            user.Player.SendTemporaryMessage(results);
        }

        [ChatCommand("Sets your personal home to your current position", ChatAuthorizationLevel.User)]
        public static void SetHome(User user)
        {
            if (homes.ContainsKey(user.Player.ID))
                homes.Remove(user.Player.ID);
            homes.Add(user.Player.ID, user.Player.Position);
            FileHelper.SaveToFile(homes.ToDictionary(item => item.Key.ToString(), item => item.Value), "homes.txt");
            user.Player.SendTemporaryMessage("Your home has been successfully set to '" + user.Player.Position + "'!");
        }

        [ChatCommand("Teleports you to your home", ChatAuthorizationLevel.User)]
        public static void Home(User user)
        {
            if (!homes.ContainsKey(user.Player.ID))
            {
                user.Player.SendTemporaryError("Your home wasn't set yet!");
                return;
            }
            user.Player.SetPosition(homes[user.Player.ID]);
            user.Player.SendTemporaryMessage("Returning to 'home'...");
        }

        public static void CallWarpSign(Player player, String text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            string[] lines = text.Split(new[] { "<br>" }, StringSplitOptions.None);

            if (lines.Length < 2 || !lines[0].Equals("[WARP]") || lines[1].Contains(' '))
                return;

            Eco.Mods.Kronox.AdvancedTeleportation.Warp(player.User, lines[1]);
        }
    }


    public static class FileHelper
    {
        private static string filePath = "Mods/Kronox/AdvancedTeleportation/";

        public static void SaveToFile(Dictionary<string, Vector3> dic, string fileName)
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

        public static Dictionary<string, Vector3> ReadFromFile(string fileName)
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