using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HeadWindCSS.Domains.Extensions.UI.Elements;
using HeadWindCSS.Domains.Serialization;
using HeadWindCSS.Domains.Settings.ScriptableObjects;
using HeadWindCSS.Domains.Settings.Theme;

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
        
        public void Initialize()
        {
            _headWindCssSettings = GetHeadWindCssSettings();
        }
        
        internal virtual HeadWindCssSettings GetHeadWindCssSettings()
        {
            return HeadWindCssSettings.Load();
        }
        
        public async Task<KeyValuePair<List<string>, List<string>>> NewParseClassProperties(string properties)
        {
            return await Task.Run(() =>
            {
                var dynamicProperties = ParseDynamicProperties(properties);
                var dynamicColors = ParseDynamicColors(properties);
                var parsedProperties = dynamicProperties.Key.Concat(dynamicColors.Key);
                var styleSheet = dynamicProperties.Value.Concat(dynamicColors.Value);

                return new KeyValuePair<List<string>, List<string>>(parsedProperties.ToList(), styleSheet.ToList());
            });
        }
        

        /// <summary>
        /// Parse the class properties from the attributes inside the UXML
        /// </summary>
        /// <param name="properties"></param>
        public IEnumerable<string> ParseClassProperties(string properties)
        {
            var dynamicProperties = ConvertUxmlDynamicClassProperties(properties);
            var dynamicColors = ParseDynamicColors(properties);
            var parsedProperties = dynamicProperties.Concat(dynamicColors.Key);

            return parsedProperties;
        }

        private IEnumerable<string> ConvertUxmlDynamicClassProperties(string properties)
        {
            List<string> parsedProperties = new List<string>();
            
            var matches = Regex.Matches(properties, @".+?-\[(.*?)\]");
            
            if (matches.Count <= 0)
            {
                return parsedProperties;
            }

            foreach (Match match in matches)
            {
                // Get prefix and value
                var matchPrefix = match.Value.Substring(0, match.Value.IndexOf("-", StringComparison.Ordinal));
                var matchPropValue = match.Value.Substring(match.Value.IndexOf("-", StringComparison.Ordinal) + 1);
                var prefixKey = $"{matchPrefix}-".Trim();
                
                // bg-
                // #fff
                // .bg-_fff_ {background-color: #fff;}
                var propValue = matchPropValue.Replace("[", string.Empty).Replace("]", string.Empty);
                var withoutHash = propValue.Replace("#", string.Empty);
                var propPrefixAndValue = prefixKey + "_" + withoutHash + "_";

                // if the dynamic property is found in the settings already just return the value
                if (_headWindCssSettings.DynamicProperties.ContainsKey(propPrefixAndValue))
                {
                    continue;
                }
                
                parsedProperties.Add(propPrefixAndValue);
            }

            return parsedProperties;
        }

        /// <summary>
        /// Parse dynamic properties to a list of properties and a stylesheet
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private KeyValuePair<List<string>, List<string>> ParseDynamicProperties(string properties)
        {
            List<string> parsedProperties = new List<string>();
            List<string> styleSheet = new List<string>();

            var matches = Regex.Matches(properties, @".+?-\[(.*?)\]");
            
            if (matches.Count <= 0)
            {
                return new KeyValuePair<List<string>, List<string>>(parsedProperties, styleSheet);
            }
            
            foreach (Match match in matches)
            {
                // Get prefix and value
                var matchPrefix = match.Value.Substring(0, match.Value.IndexOf("-", StringComparison.Ordinal));
                var matchPropValue = match.Value.Substring(match.Value.IndexOf("-", StringComparison.Ordinal) + 1);
                var prefixKey = $"{matchPrefix}-".Trim();

                // bg-
                // #fff
                // .bg-_fff_ {background-color: #fff;}
                var propValue = matchPropValue.Replace("[", string.Empty).Replace("]", string.Empty);
                var withoutHash = propValue.Replace("#", string.Empty);
                var propPrefixAndValue = prefixKey + "_" + withoutHash + "_";

                if (!_prefixes.TryGetValue(prefixKey, out var prefixValue)) continue;

                var styleSheetPropValue = "." + prefixKey + "_" + withoutHash + "_ {" + prefixValue + ": " + propValue +
                                          ";}";
                parsedProperties.Add(propPrefixAndValue);
                styleSheet.Add(styleSheetPropValue);
            }

            return new KeyValuePair<List<string>, List<string>>(parsedProperties, styleSheet);
        }

        /// <summary>
        /// Parse dynamic colors to a list of properties and a stylesheet
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal KeyValuePair<List<string>, List<string>> ParseDynamicColors(string properties)
        {
            var parsedProperties = new List<string>();
            var styleSheet = new List<string>();
            
            // See if matching suffix matches the theme settings
            if(_headWindCssSettings.Theme.TryGetValue("colors", out var themeColors))
            {
                foreach (var colors in themeColors)
                {
                    // for example text-primary-500
                    var regexCheck = $"(\\w+):?(\\w+)-{colors.Key}-?(\\w+)?"; // $"(.*-{pair.Key}.*)";
                    var matches = Regex.Matches(properties, regexCheck);
                    
                    if (matches.Count <= 0)
                    {
                        continue;
                    }
                    
                    var themeSetting = colors.Value;

                    foreach (Match match in matches)
                    {
                        var pseudo = HasPseudoClass(match.Value) ? String.Copy(match.Value).Substring(0, match.Value.IndexOf(":", StringComparison.Ordinal)) : "";
                        // Debug.Log("Psuedo: " + pseudo);

                        var index = HasPseudoClass(match.Value)
                        ? match.Value.IndexOf(":", StringComparison.Ordinal)
                        : 0;
                        
                        // Debug.Log("length: " + match.Value.Length);
                        // Debug.Log("index: " + match.Value.IndexOf(":", StringComparison.Ordinal));
                        
                        string matchPrefix = match.Value.Substring(index,match.Value.IndexOf("-", StringComparison.Ordinal));
                        var prefixKey = $"{matchPrefix}-".Trim();
                        
                        // Debug.Log("prefix: " + matchPrefix);
                        
                        // The prefix value is the property name in css such color: 
                        if (!_prefixes.TryGetValue(prefixKey, out var prefixValue)) continue;
                        
                        // if the match is found, and the value has two occurrences of the dash
                        // we need a theme setting object for it to be parsed
                        int count = match.Value.Count(f => f == '-');
                        
                        // if we have one dash we can only have a value themesetting
                        if (count == 1 && themeSetting.Type == ThemeSettingType.Value)
                        {
                            // text-
                            var prop = pseudo + prefixKey + colors.Key;
                            
                            // Debug.Log(prop);
                            
                            // bg-
                            // #fff
                            // .text-primary {background-color: #fff;}   
                            var styleSheetPropValue =
                                "." + prop + "{" + prefixValue + ": " + colors.Value.Value.ToRGBHex() + ";}"; 
                            styleSheet.Add(styleSheetPropValue);
                            parsedProperties.Add(prop);
                        }
                        else if (count == 2 && themeSetting.Type == ThemeSettingType.Object)
                        {
                            // Get last part of the match value. for example text-primary-900. We need the 900 value
                            string matchSuffix = match.Value.Substring(match.Value.LastIndexOf("-", StringComparison.Ordinal) + 1);

                            if (themeSetting is ThemeSettingObject themeSettingObject &&
                                themeSettingObject.Value.TryGetValue(matchSuffix, out var themeSettingValue))
                            {
                                // text-primary-500
                                // var propValue =
                                //     match.Value.Substring(match.Value.IndexOf("-", StringComparison.Ordinal) + 1);
                                // var propPrefixAndValue = prefixKey + propValue + " ";

                                // text-primary-500
                                // #fff
                                // .text-primary-500 {background-color: #fff;}
                                var styleSheetPropValue = "." + match.Value + "{" + prefixValue + ": " +
                                                          themeSettingValue.ToRGBHex() + ";}"; 
                                styleSheet.Add(styleSheetPropValue);
                                parsedProperties.Add(match.Value);
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
                            throw new ArgumentException("Invalid type or class not found for key", themeSetting.Key);
                        }
                    }
                }
            }
            
            return new KeyValuePair<List<string>, List<string>>(parsedProperties, styleSheet);
        }
        
        private bool HasPseudoClass(string property)
        {
            return property.Contains(":");
        }
        
        private string ConvertClassToDynamicProperty()
        {
            return "";
        }
    }
}