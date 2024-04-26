using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HeadWindCSS.Domains.IO;
using HeadWindCSS.Domains.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeadWindCSS.Domains.ServiceProviders
{
    public class UxmlHelper : IService
    {
        internal static readonly string USSFolderPath = $"{Application.dataPath}/Settings/HeadWindCSS";
        internal static readonly string USSPath = $"{USSFolderPath}/{HeadWindDynamicCssFileName}.uss";

        internal const string HeadWindDynamicCssFileName = "dynamic";
        
        private readonly SerializableDictionary<string, string> _prefixes = new()
        {
            { "bg-", "background-color" },
            { "text-", "color" },
            { "border-", "border-color" },
            { "ring-", "box-shadow" },
            { "divide-", "border-color" },
            { "from-", "background-color" },
            { "via-", "background-color" },
            { "to-", "background-color" },
            { "placeholder-", "color" },
            { "divide-opacity-", "border-opacity" },
            { "bg-opacity-", "background-opacity" },
            { "text-opacity-", "color-opacity" },
            { "border-opacity-", "border-opacity" },
            { "ring-opacity-", "box-shadow-opacity" },
            { "ring-offset-", "box-shadow-offset" },
            { "ring-offset-opacity-", "box-shadow-offset-opacity" },
            { "space-x-", "margin-left" },
            { "space-y-", "margin-top" },
            { "divide-x-", "border-left" },
            { "divide-y-", "border-top" },
            { "space-x-reverse-", "margin-right" },
            { "space-y-reverse-", "margin-bottom" },
            { "divide-x-reverse-", "border-right" },
            { "divide-y-reverse-", "border-bottom" },
            { "rounded-", "border-radius" },
            { "rounded-t-", "border-top-left-radius" },
            { "rounded-r-", "border-top-right-radius" },
            { "rounded-b-", "border-bottom-right-radius" },
            { "rounded-l-", "border-bottom-left-radius" },
            { "border-t-", "border-top-width" },
            { "border-r-", "border-right-width" },
            { "border-b-", "border-bottom-width" },
            { "border-l-", "border-left-width" }
        };

        /// <summary>
        /// Parse the class properties from the attributes inside the UXML
        /// </summary>
        /// <param name="properties"></param>
        public async Task<IEnumerable<string>> ParseClassProperties(string properties)
        {
            var matches = Regex.Matches(properties, @".+?-\[(.*?)\]");
            if (matches.Count <= 0)
            {
                return new List<string>();
            }
            
            string parsedProperties = "";
            string styleSheet = "";
            foreach (Match match in matches)
            {
                // Get prefix and value
                string matchPrefix = match.Value.Substring(0, match.Value.IndexOf("-", StringComparison.Ordinal));
                string matchPropValue = match.Value.Substring(match.Value.IndexOf("-", StringComparison.Ordinal) + 1);

                var prefixKey = $"{matchPrefix}-".Trim();

                if (!_prefixes.TryGetValue(prefixKey, out var prefixValue)) continue;
                
                // bg-
                // #fff
                // .bg-_fff_ {background-color: #fff;}
                var propValue = matchPropValue.Replace("[", string.Empty).Replace("]", string.Empty);
                var withoutHash = propValue.Replace("#", string.Empty);
                var propPrefix = prefixKey + "_" + withoutHash + "_ ";

                styleSheet += "." + prefixKey + "_" + withoutHash + "_ {" + prefixValue + ": " + propValue + ";} ";
                parsedProperties += propPrefix;

                /*foreach (var prefix in _prefixes)
                {
                    foreach (var prop in props)
                    {
                        if (!prop.Contains($"{prefix.Key}[")) continue;

                        Debug.Log($"prop: {prop}");


                    }
                }*/
            }
            
            // bg-[#fff] --> .bg-\[#fff\] { background-color: #fff; }
            Debug.Log($"parsedProperties: {parsedProperties}, styleSheet: {styleSheet}");
            
            await SaveParsedProperties(styleSheet);

            return parsedProperties.Trim().Split(" ");
        }
        
        /// <summary>
        /// Save the parsed properties to the file
        /// </summary>
        /// <param name="properties"></param>
        private Task SaveParsedProperties(string properties)
        {
            if (!Directory.Exists(USSFolderPath))
            {
                Directory.CreateDirectory(USSFolderPath);
            }

            if (!File.Exists(USSPath))
            {
                ResourceHelper.CreateAsset<StyleSheet>(USSPath);
            }

            return File.WriteAllTextAsync(USSPath, properties);
            // File.WriteAllText(USSPath, properties);
        }
    }
}