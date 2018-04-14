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

using Eco.Shared.Math;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedTeleportation.storable
{
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
}
