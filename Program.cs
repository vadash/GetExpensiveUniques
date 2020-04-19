using System;
using System.Collections.Generic;
using System.IO;
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

        private const int cutoff = 10;
        
        public static void Main(string[] args)
        {
            var result = "";

            foreach (var url in uniquesUrls)
            {
                using (var wc = new WebClient())
                {
                    var json = wc.DownloadString(url);
                    var o = JObject.Parse(json);
                    foreach (var line in o?["lines"])
                    {
                        if (int.TryParse((string)line?["links"], out var links) &&
                            links == 6)
                        {
                            continue;
                        }
                        
                        if (int.TryParse((string)line?["chaosValue"], out var chaosValue) &&
                            chaosValue >= cutoff)
                        {
                            result += "\"" + (string)line?["name"] + "\"," + Environment.NewLine;
                        }
                    }
                }                
            }
            
            File.WriteAllText("expensive.txt", result);
        }
    }
}