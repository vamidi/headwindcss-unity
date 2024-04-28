using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using HeadWindCSS.Domains.Serialization;
using HeadWindCSS.Domains.Settings.ScriptableObjects;
using HeadWindCSS.Domains.Settings.Theme;
using UnityEngine;

namespace HeadWindCSS.Domains.ServiceProviders
{
    public class UxmlHelper : IService
    {
        private HeadWindCssSettings _headWindCssSettings;
        
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

        public UxmlHelper()
        {
            Initialize();
        }
        
        protected void Initialize()
        {
            _headWindCssSettings = GetHeadWindCssSettings();
        }
        
        internal virtual HeadWindCssSettings GetHeadWindCssSettings()
        {
            return HeadWindCssSettings.Load();
        }

        /// <summary>
        /// Parse the class properties from the attributes inside the UXML
        /// </summary>
        /// <param name="properties"></param>
        public async Task<IEnumerable<string>> ParseClassProperties(string properties)
        {
            var parsedProperties = "";
            var styleSheet = "";

            var dynamicProperties = await ParseDynamicProperties(properties);
            parsedProperties += dynamicProperties.Key;
            styleSheet += dynamicProperties.Value;
            
            var dynamicColors = await ParseDynamicColors(properties);
            parsedProperties += dynamicColors.Key;
            styleSheet += dynamicColors.Value;
            
            Debug.Log($"parsedProperties: {parsedProperties}, styleSheet: {styleSheet}");

            return parsedProperties.Trim().Split(" ");
        }

        internal async Task<KeyValuePair<string, string>> ParseDynamicProperties(string properties)
        {
            var matches = Regex.Matches(properties, @".+?-\[(.*?)\]");
            if (matches.Count <= 0)
            {
                return new KeyValuePair<string, string>();
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
                var propPrefixAndValue = prefixKey + "_" + withoutHash + "_ ";

                var styleSheetPropValue = "." + prefixKey + "_" + withoutHash + "_ {" + prefixValue + ": " + propValue +
                                          ";} ";
                styleSheet += styleSheetPropValue;
                parsedProperties += propPrefixAndValue;
                
                _headWindCssSettings.AddDynamicValue(propPrefixAndValue, styleSheetPropValue);

                /*foreach (var prefix in _prefixes)
                {
                    foreach (var prop in props)
                    {
                        if (!prop.Contains($"{prefix.Key}[")) continue;

                        Debug.Log($"prop: {prop}");


                    }
                }*/
            }

            return new KeyValuePair<string, string>(parsedProperties.Trim(), styleSheet.Trim());
        }

        internal async Task<KeyValuePair<string, string>> ParseDynamicColors(string properties)
        {
            var parsedProperties = "";
            var styleSheet = "";
            
            // See if matching suffix matches the theme settings
            if(_headWindCssSettings.Theme.TryGetValue("colors", out var themeColors))
            {
                foreach (var colors in themeColors)
                {
                    // for example text-primary-500
                    var regexCheck = $"(\\w+)-{colors.Key}-?(\\w+)?"; // $"(.*-{pair.Key}.*)";
                    var matches = Regex.Matches(properties, regexCheck);
                    
                    if (matches.Count <= 0)
                    {
                        continue;
                    }
                    
                    var themeSetting = colors.Value;

                    foreach (Match match in matches)
                    {
                        Debug.Log("match: " + match.Value);
                        
                        string matchPrefix = match.Value.Substring(0, match.Value.IndexOf("-", StringComparison.Ordinal));
                        var prefixKey = $"{matchPrefix}-".Trim();
                        
                        // The prefix value is the property name in css such color: 
                        if (!_prefixes.TryGetValue(prefixKey, out var prefixValue)) continue;
                        
                        // if the match is found, and the value has two occurrences of the dash
                        // we need a theme setting object for it to be parsed
                        int count = match.Value.Count(f => f == '-');
                        
                        // if we have one dash we can only have a value themesetting
                        if (count == 1 && themeSetting.Type == ThemeSettingType.Value)
                        {
                            // text-
                            var prop = prefixKey + colors.Key;
                            
                            // bg-
                            // #fff
                            // .text-primary {background-color: #fff;}   
                            var styleSheetPropValue =
                                "." + colors.Key + "{" + prefixValue + ": " + colors.Value.Value + ";} "; 
                            styleSheet += styleSheetPropValue;
                            parsedProperties += prop + " ";
                            
                            _headWindCssSettings.AddDynamicValue(prop, styleSheetPropValue);

                        }
                        else if (count == 2 && themeSetting.Type == ThemeSettingType.Object)
                        {
                            // Get last part of the match value. for example text-primary-900. We need the 900 value
                            string matchSuffix = match.Value.Substring(match.Value.LastIndexOf("-", StringComparison.Ordinal) + 1);

                            if (themeSetting is ThemeSettingObject themeSettingObject &&
                                themeSettingObject.Value.TryGetValue(matchSuffix, out var themeSettingValue))
                            {
                                // text-primary-500
                                var propValue =
                                    match.Value.Substring(match.Value.IndexOf("-", StringComparison.Ordinal) + 1);
                                var propPrefixAndValue = prefixKey + propValue + " ";

                                // text-primary-500
                                // #fff
                                // .text-primary-500 {background-color: #fff;}
                                var styleSheetPropValue = "." + match.Value + "{" + prefixValue + ": " +
                                                          themeSettingValue + ";} "; 
                                styleSheet += styleSheetPropValue;
                                parsedProperties += match.Value;
                                
                                _headWindCssSettings.AddDynamicValue(match.Value, styleSheetPropValue);
                            }
                            else 
                            {
                                throw new ArgumentException(
                                    $"Property {match.Value} could not be found in the theme settings"
                                );
                            }
                        }
                        else
                        {
                            Debug.Log("theme settings type: " + nameof(themeSetting.Type));
                            throw new ArgumentException("Invalid type or class not found.", nameof(themeSetting.Type));
                        }
                    }
                }
            }
            
            return new KeyValuePair<string, string>(parsedProperties.Trim(), styleSheet.Trim());
        }
    }
}