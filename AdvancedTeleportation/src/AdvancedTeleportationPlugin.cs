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
 * Version: 1.3.1
 * ------------------------------------
 **/

using System;
using System.Collections.Generic;
using AdvancedTeleportation.Settings;
using AdvancedTeleportation.storable;
using Asphalt;
using Asphalt.Api.Event;
using Asphalt.Api.Util;
using Asphalt.Service.Permissions;
using Eco.Shared.Math;

namespace AdvancedTeleportation
{
    public class AdvancedTeleportationPlugin : AsphaltMod
    {
        public static AsphaltMod Mod { get; private set; }

        public static readonly string filePath = "Mods/AdvancedTeleportation/save/";

        public static Warps Warps { get; set; }
        public static Homes Homes { get; set; }

        public static Dictionary<string, Vector3> BackPos { get; set; }

        public override void OnEnable()
        {
            Mod = this;

            Warps = ClassSerializer<Warps>.Deserialize(filePath, "warps.json");
            Homes = ClassSerializer<Homes>.Deserialize(filePath, "homes.json");

            BackPos = new Dictionary<string, Vector3>();

            EventManager.RegisterListener(new AdvTpEventListener());
        }

        public override string ToString()
        {
            return "AdvancedTeleportation";
        }

        public override List<Type> GetCustomSettings()
        {
            return new List<Type>()
            {
                typeof(TpSettings)
            };
        }

        public override List<Permission> GetPermissions()
        {
            return new List<Permission>()
            {
                new Permission("advtp.help", PermissionGroup.User),
                new Permission("advtp.reloadconfig", PermissionGroup.Admin),
                new Permission("advtp.reloadpermissions", PermissionGroup.Admin),
                new Permission("back", PermissionGroup.User),
                new Permission("warp.help", PermissionGroup.User),
                new Permission("warp.set", PermissionGroup.Admin),
                new Permission("warp.remove", PermissionGroup.Admin),
                new Permission("warp.teleport.cmd", PermissionGroup.User),
                new Permission("warp.teleport.sign", PermissionGroup.User),
                new Permission("warp.list", PermissionGroup.User),
                new Permission("home.help", PermissionGroup.User),
                new Permission("home.set", PermissionGroup.User),
                new Permission("home.remove", PermissionGroup.User),
                new Permission("home.teleport", PermissionGroup.User),
                new Permission("home.list", PermissionGroup.User),
                new Permission("home.setlimit", PermissionGroup.Admin)
            };
        }
    }
}