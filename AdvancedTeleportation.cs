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

        [ChatCommand("Sets a new warp point or overrides an existing one", ChatAuthorizationLevel.Admin)]
        public static void SetWarp(User user, string name)
        {
            Initialize();

            name = name.ToLower();

            warps.Add(name, user.Player.Position);
            ClassSerializer<Warps>.Serialize(filePath, "warps.json", warps);
            user.Player.SendTemporaryMessage("Warp '" + name + "' has been sucessfully set to '" + user.Player.Position + "'!");
        }

        [ChatCommand("Removes an existing warp point", ChatAuthorizationLevel.Admin)]
        public static void RemoveWarp(User user, string name)
        {
            Initialize();

            name = name.ToLower();

            if (!warps.Exists(name))
            {
                user.Player.SendTemporaryError("'" + name + "' isn't a known warp!");
                return;
            }
            warps.Remove(name);
            ClassSerializer<Warps>.Serialize(filePath, "warps.json", warps);
            user.Player.SendTemporaryMessage("Warp '" + name + "' has been sucessfully removed!");
        }

        [ChatCommand("Teleports you to a warp point", ChatAuthorizationLevel.User)]
        public static void Warp(User user, string name)
        {
            Initialize();

            name = name.ToLower();

            if (!warps.Exists(name))
            {
                user.Player.SendTemporaryError("'" + name + "' isn't a known warp!");
                return;
            }
            user.Player.SetPosition(warps.Get(name));
            user.Player.SendTemporaryMessage("Warping to '" + name + "'...");
        }

        [ChatCommand("Lists all existing warps", ChatAuthorizationLevel.User)]
        public static void Warps(User user)
        {
            Initialize();

            String results = "";

            if (warps.IsEmpty())
            {
                user.Player.SendTemporaryError("No warps have been set yet!");
                return;
            }

            user.Player.SendTemporaryMessage("Existing warps:");
            bool first = true;
            foreach (KeyValuePair<string, Dictionary<string, float>> pair in warps.warps)
            {
                if (!first) results += ", ";
                results += pair.Key;
                first = false;
            }
            user.Player.SendTemporaryMessage(results);
        }

        [ChatCommand("Sets your personal home to your current position", ChatAuthorizationLevel.User)]
        public static void SetHome(User user)
        {
            Initialize();

            homes.Add(user.SteamId, user.Player.Position);
            ClassSerializer<Homes>.Serialize(filePath, "homes.json", homes);
            user.Player.SendTemporaryMessage("Your home has been successfully set to '" + user.Player.Position + "'!");
        }

        [ChatCommand("Teleports you to your home", ChatAuthorizationLevel.User)]
        public static void Home(User user)
        {
            Initialize();

            if(oldHomes.ContainsKey(user.Player.ID))
            {
                if (!homes.Exists(user.SteamId))
                {
                    homes.Add(user.SteamId, oldHomes[user.Player.ID]);
                    ClassSerializer<Homes>.Serialize(filePath, "homes.json", homes);
                }

                oldHomes.Remove(user.Player.ID);
                OldFileHelper.SaveToFile(oldHomes.ToDictionary(item => item.Key.ToString(), item => item.Value), filePath, "homes.txt");

                if (oldHomes.Count <= 0)
                    File.Delete(filePath + "homes.txt");
            }

            if (!homes.Exists(user.SteamId))
            {
                user.Player.SendTemporaryError("Your home wasn't set yet!"+user.Player.ID);
                return;
            }
            user.Player.SetPosition(homes.Get(user.SteamId));
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