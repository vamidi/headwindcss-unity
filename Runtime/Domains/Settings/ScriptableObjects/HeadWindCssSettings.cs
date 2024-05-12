using System.Collections.Generic;
using HeadWindCSS.Domains.Extensions.UI.Elements;
using UnityEngine;

namespace HeadWindCSS.Domains.Settings.ScriptableObjects
{
    using Theme;
    using Serialization;
    using ServiceProviders.Authority;
    
    [CreateAssetMenu(fileName = "HeadWindCssSettings", menuName = "HeadWindCSS/Settings/HeadWindCSS settings", order = 1)]
    public class HeadWindCssSettings : ScriptableObject
    {
        public const string CONFIG_NAME = "com.vamidicreations.headwindcss.settings";
        
        internal SerializableDictionary<string, ClassVariant> Variants => variants;

        internal SerializableDictionary<string, SerializableDictionary<string, ThemeSetting>> Theme => theme;
        
        internal SerializableDictionary<string, string> DynamicProperties => dynamicValues;
        
        private static HeadWindCssSettings _instance;
        
        [SerializeField, Tooltip("Variants")] 
        private SerializableDictionary<string, ClassVariant> variants = new();
        
        [SerializeField, Tooltip("Theme settings")]
        private SerializableDictionary<string, SerializableDictionary<string, ThemeSetting>> theme = new()
        {
            { "colors", new ()
                {
                    // Persona3R style primary color (for testing purposes)
                    { "primary", new ThemeSetting(
                        type: ThemeSettingType.Value,
                        key: "primary",
                        value: ColorHelper.ParseHtmlColor("#181c52")                   
                    )},
                    { "secondary", new ThemeSetting(
                        ThemeSettingType.Value,
                        key: "secondary",
                        value: ColorHelper.ParseHtmlColor("#0756f1")
                    )}, 
                    { "alternate", new ThemeSetting(
                        ThemeSettingType.Value,
                        key: "alternate",
                        value: Color.white
                    )},
                }
            },
            {
                "fontFamily", new SerializableDictionary<string, ThemeSetting>
                {
                    // { "sans", "Graphik, sans-serif" },
                    // { "serif", "Merriweather, serif" }
                }  
            },
            {
                "extend", new SerializableDictionary<string, ThemeSetting>()
                {
                    // spacing = new SerializableDictionary<string, string>
                    // {
                    //     { "8xl", "96rem" },
                    //     { "9xl", "128rem" }
                    // },
                    // borderRadius = new SerializableDictionary<string, string>
                    // {
                    //     { "4xl", "2rem" }
                    // }    
                }
            }
        };
        
        public static HeadWindCssSettings Load()
        {
            if (_instance) return _instance;
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject(CONFIG_NAME, out _instance);
#else
            // Loads from the memory.
            instance = FindObjectOfType<SceneLoaderSettings>();
#endif
            return _instance;
        }

        private bool _dirty;
        
        [SerializeField]
        private SerializableDictionary<string, string> dynamicValues = new();
        
        internal void AddDynamicValue(string key, string value)
        {
            if (dynamicValues.ContainsKey(key))
            {
                return;
            }
            
            dynamicValues.Add(key, value);
            _dirty = true;
        }
        
        internal SerializableDictionary<string, ThemeSetting> GetThemeSetting(string themeKey)
        {
            return theme.GetValueOrDefault(themeKey);
        }
    }
}