﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Quad64.src.JSON
{
    class BehaviorNameFile
    {
        private static uint parseUInt(string str)
        {
            bool isHex = false;
            if (str.StartsWith("0x"))
            {
                str = str.Substring(2);
                isHex = true;
            }
            else if (str.StartsWith("$"))
            {
                str = str.Substring(1);
                isHex = true;
            }
            if (!isHex)
                return uint.Parse(str);
            else
                return uint.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }

        public static void writeObjectCombosFile(string filename)
        {
            Globals.objectComboEntries.Sort((x, y) => string.Compare(x.Name, y.Name));

            JArray array = new JArray();
            foreach (ObjectComboEntry oce in Globals.objectComboEntries)
            {
                JObject entry = new JObject();
                entry["Name"] = oce.Name;
                entry["Behavior"] = "0x" + oce.Behavior.ToString("X8");
                array.Add(entry);
            }

            JObject o = new JObject();
            o["BehaviorNames"] = array;

            File.WriteAllText(filename, o.ToString());
        }

        private static bool checkValidEntry(JObject entry)
        {
            return (entry["Name"] != null && entry["Behavior"] != null);
        }

        public static void parseBehaviorNames(string filename)
        {
            if (File.Exists(filename))
            {
                string json = File.ReadAllText(filename);
                JObject o = JObject.Parse(json);
                if (o["BehaviorNames"] != null)
                {
                    JArray array = (JArray)o["BehaviorNames"];
                    foreach(JToken token in array.Children())
                    {
                        JObject entry = (JObject)token;
                        if (checkValidEntry(entry))
                        {
                            string name = entry["Name"].ToString();
                            string Behavior_s = entry["Behavior"].ToString();
                            uint Behavior = parseUInt(Behavior_s);
                            BehaviorNameEntry bne = new BehaviorNameEntry(name, Behavior);
                            Globals.behaviorNameEntries.Add(bne);
                        }
                    }
                }
            }
        }
    }
}
