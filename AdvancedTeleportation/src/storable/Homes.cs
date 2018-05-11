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
}
