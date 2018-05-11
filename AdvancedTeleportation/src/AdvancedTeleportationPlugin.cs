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
 * Version: 2.0.0
 * ------------------------------------
 **/

using System.Collections.Generic;
using System.IO;
using AdvancedTeleportation.storable;
using Asphalt;
using Asphalt.Api.Event;
using Asphalt.Api.Util;
using Asphalt.Service;
using Asphalt.Service.Permissions;
using Asphalt.Storeable;
using Eco.Core.Plugins.Interfaces;
using Eco.Shared.Math;
using Eco.Shared.Utils;

namespace AdvancedTeleportation
{
    [AsphaltPlugin]
    public class AdvancedTeleportationPlugin : IModKitPlugin, IDisableable
    {
        private bool disabled;
        public static string VERSION = "v.2.0.0";

        [Inject]
        public static IPermissionService PermissionService { get; set; }

        [Inject]
        [DefaultValues(nameof(GetDefaultUserSettings))]
        [StorageLocation("user_settings")]
        public static IUserStorageCollection UserSettings { get; set; }

        [Inject]
        [StorageLocation("storage/warps")]
        public static IStorage WarpsStorage { get; set; }

        [Inject]
        [StorageLocation("storage/homes/")]
        public static IUserStorageCollection HomesStorage { get; set; }

        public static Warps OldWarpsStorage { get; set; }
        public static Homes OldHomesStorage { get; set; }

        public static Dictionary<string, Vector3> BackPos { get; set; }

        public void OnEnable()
        {
            if (IsDisabled())
                return;

            OldWarpsStorage = ClassSerializer<Warps>.Deserialize(Path.Combine(ServiceHelper.GetServerPluginFolder(this.GetType()), "storage", "old_warps.json"));
            OldHomesStorage = ClassSerializer<Homes>.Deserialize(Path.Combine(ServiceHelper.GetServerPluginFolder(this.GetType()), "storage", "old_homes.json"));

            BackPos = new Dictionary<string, Vector3>();

            EventManager.RegisterListener(new AdvTpEventListener());
        }

        public override string ToString()
        {
            return "AdvancedTeleportation";
        }

        public static KeyDefaultValue[] GetDefaultUserSettings()
        {
            return new KeyDefaultValue[]
            {
                new KeyDefaultValue("home-limit", 5)
            };
        }

        [Inject]
        public static DefaultPermission[] GetDefaultPermissions()
        {
            return new DefaultPermission[]
            {
                new DefaultPermission("advtp.help", PermissionGroup.User),
                new DefaultPermission("advtp.reload", PermissionGroup.Admin),
                new DefaultPermission("back", PermissionGroup.User),
                new DefaultPermission("warp.help", PermissionGroup.User),
                new DefaultPermission("warp.set", PermissionGroup.Admin),
                new DefaultPermission("warp.remove", PermissionGroup.Admin),
                new DefaultPermission("warp.teleport.cmd", PermissionGroup.User),
                new DefaultPermission("warp.teleport.sign", PermissionGroup.User),
                new DefaultPermission("warp.list", PermissionGroup.User),
                new DefaultPermission("home.help", PermissionGroup.User),
                new DefaultPermission("home.set", PermissionGroup.User),
                new DefaultPermission("home.remove", PermissionGroup.User),
                new DefaultPermission("home.teleport", PermissionGroup.User),
                new DefaultPermission("home.list", PermissionGroup.User),
                new DefaultPermission("home.setlimit", PermissionGroup.Admin),
                new DefaultPermission("home.ignorelimit", PermissionGroup.Admin)
            };
        }

        public string GetStatus()
        {
            return $"[{VERSION}] Running...";
        }

        public void Disable(string reason)
        {
            Log.WriteError($"Disabling {this.ToString()}...\nReason: {reason}");
            SetDisabled(true);
        }

        public void SetDisabled(bool disabled)
        {
            this.disabled = disabled;
        }

        public bool IsDisabled()
        {
            return disabled;
        }
    }
}