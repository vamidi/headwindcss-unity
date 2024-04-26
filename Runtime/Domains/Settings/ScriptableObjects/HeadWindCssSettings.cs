using System.Collections.Generic;
using UnityEngine;

namespace HeadWindCSS.Domains.Settings.ScriptableObjects
{
    using Serialization;
    using ServiceProviders.Authority;
    
    [CreateAssetMenu(fileName = "HeadWindCssSettings", menuName = "HeadWindCSS/Settings/HeadWindCSS settings", order = 1)]
    public class HeadWindCssSettings : ScriptableObject
    {
        public const string CONFIG_NAME = "com.vamidicreations.headwindcss.settings";
        
        public const string ButtonVariant = "buttonVariants";
        
        [SerializeField, Tooltip("Variants")] 
        private SerializableDictionary<string, ClassVariant> variants = new()
        {
            { // Buttons
                ButtonVariant, new ClassVariant {
                    baseClasses = "font-bold rounded-lg",
                    variants = new ()
                    {
                        {
                            "variant", new SerializableDictionary<string, string>
                            {
                                { "default", "text-primary-500" },
                                { "primary", "text-white" }, // bg-indigo-600 
                            }
                        },
                        {
                            "size", new SerializableDictionary<string, string>()
                            {
                                { "sm", "h-8" },
                                { "md", "h-10" }
                            }
                        }
                    }}
            }
        };

        private List<string> _dynamicValues = new();

    }
}