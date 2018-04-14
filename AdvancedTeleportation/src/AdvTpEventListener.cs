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

using AdvancedTeleportation.command;
using Asphalt.Api.Event;
using Asphalt.Api.Event.PlayerEvents;
using Eco.Gameplay.Components;
using Eco.Mods.TechTree;
using Eco.Shared.Items;

namespace AdvancedTeleportation
{
    public class AdvTpEventListener
    {
        [EventHandler]
        public void OnPlayerTeleport(PlayerTeleportEvent evt)
        {
            if (AdvancedTeleportationPlugin.BackPos.ContainsKey(evt.Player.User.SlgId))
                AdvancedTeleportationPlugin.BackPos.Remove(evt.Player.User.SlgId);

            AdvancedTeleportationPlugin.BackPos.Add(evt.Player.User.SlgId, evt.Player.Position);
        }

        [EventHandler]
        public void OnPlayerLogout(PlayerLogoutEvent evt)
        {
            if (AdvancedTeleportationPlugin.BackPos.ContainsKey(evt.User.SlgId))
                AdvancedTeleportationPlugin.BackPos.Remove(evt.User.SlgId);
        }

        [EventHandler]
        public void OnPlayerInteract(PlayerInteractEvent evt)
        {
            if (!evt.Context.HasTarget)
                return;

            if (!typeof(WoodSignObject).IsAssignableFrom(evt.Context.Target.GetType()))
                return;

            if (!evt.Context.Method.Equals(InteractionMethod.Right))
                return;

            WoodSignObject sign = (WoodSignObject)evt.Context.Target;

            WarpCommands.CallWarpSign(evt.Context.Player, sign.GetComponent<CustomTextComponent>().Text);
        }
    }
}
