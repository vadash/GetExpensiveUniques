using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace GetExpensiveUniques
{
    internal class Program
    {
        private static List<string> uniquesUrls = new List<string>()
        {
            @"https://poe.ninja/api/data/itemoverview?league=Delirium&type=UniqueJewel&language=en",
            @"https://poe.ninja/api/data/itemoverview?league=Delirium&type=UniqueFlask&language=en",
            @"https://poe.ninja/api/data/itemoverview?league=Delirium&type=UniqueWeapon&language=en",
            @"https://poe.ninja/api/data/itemoverview?league=Delirium&type=UniqueArmour&language=en",
            @"https://poe.ninja/api/data/itemoverview?league=Delirium&type=UniqueAccessory&language=en",
        };

        private const float cutoff = 10f;

        public static void Main(string[] args)
        {
            var result = new HashSet<string>();

            foreach (var url in uniquesUrls)
            {
                using (var wc = new WebClient())
                {
                    var json = wc.DownloadString(url);
                    var o = JObject.Parse(json);
                    foreach (var line in o?["lines"])
                    {
                        if (int.TryParse((string) line?["links"], out var links) &&
                            links == 6)
                        {
                            continue;
                        }

                        if (float.TryParse((string) line?["chaosValue"], out var chaosValue) &&
                            chaosValue <= cutoff)
                        {
                            result.Add((string) line?["name"]);
                        }
                    }
                }
            }

            var result2 = result.ToList();
            result2.Sort();
            var result3 = result2.Aggregate("",
                (current, line) => current + "\"" + line + "\"," + Environment.NewLine);


            File.WriteAllText("cheap.txt", result3);
        }
    }
}