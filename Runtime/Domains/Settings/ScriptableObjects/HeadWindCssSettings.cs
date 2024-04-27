using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace HeadWindCSS.Domains.Settings.ScriptableObjects
{
    using Serialization;
    using ServiceProviders.Authority;
    
    using ThemeSetting = Serialization.SerializableDictionary<string, string>;
    
    [CreateAssetMenu(fileName = "HeadWindCssSettings", menuName = "HeadWindCSS/Settings/HeadWindCSS settings", order = 1)]
    public class HeadWindCssSettings : ScriptableObject
    {
        public const string CONFIG_NAME = "com.vamidicreations.headwindcss.settings";
        
        public const string ButtonVariant = "buttonVariants";

        public ReadOnlyDictionary<string, ClassVariant> Variants => new(variants);
        
        private static HeadWindCssSettings _instance;
        
        [SerializeField, Tooltip("Variants")] 
        private SerializableDictionary<string, ClassVariant> variants = new()
        {
            { // Example for Buttons
                ButtonVariant, new ClassVariant {
                    baseClasses = "font-bold rounded-lg",
                    variants = new ()
                    {
                        {
                            "variant", new SerializableDictionary<string, string>
                            {
                                { "default", "text-primary-500" },
                                { "primary", "bg-indigo-600 text-white" }, 
                            }
                        },
                        {
                            "size", new SerializableDictionary<string, string>()
                            {
                                { "sm", "h-8" },
                                { "md", "h-10" }
                            }
                        }
                    }
                }
            }
        };
        
        [SerializeField, Tooltip("Theme settings")]
        private SerializableDictionary<string, ThemeSetting> theme = new()
        {
            { "colors", new ThemeSetting()
                {
                    { "primary", "#1fb6ff" },
                }
            },
            {
                "fontFamily", new SerializableDictionary<string, string>
                {
                    { "sans", "Graphik, sans-serif" },
                    { "serif", "Merriweather, serif" }
                }  
            },
            {
                "extend", new ThemeSetting()
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

        private List<string> _dynamicValues = new();
        
        private void AddDynamicValue(string value)
        {
            _dynamicValues.Add(value);
        }
    }
}