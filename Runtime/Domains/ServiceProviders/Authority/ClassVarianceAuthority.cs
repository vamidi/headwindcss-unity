using System;
using System.Collections.Generic;
using HeadWindCSS.Domains.Serialization;
using HeadWindCSS.Domains.Settings.ScriptableObjects;
using UnityEngine;

namespace HeadWindCSS.Domains.ServiceProviders.Authority
{
    using Variant = String;
    using VariantConfig = ClassVariant;
    /**
     * config.variants.Add("buttonVariants", new ClassVariant {
     *      baseClasses = "flex justify-center items-center",
     *      variants = {
     *          { "default", 
     *      }
     *
     * 
     * config.variants.Add("primary", new ClassVariant {
     *      baseClasses = "bg-red-500 text-primary-500 text-indigo-500 font-bold rounded-lg",
     *      classes = "bg-red-500 text-primary-500 text-indigo-500 font-bold rounded-lg"
     * });
     * 
     */
    
    [Serializable]
    public class ClassVariant
    {
        /// <summary>
        /// These base classes are the classes that automatically will
        /// be added to the visual element
        /// </summary>
        public string baseClasses;
        
        /// <summary>
        /// 
        /// </summary>
        public SerializableDictionary<Variant, 
            SerializableDictionary<string, string>> variants = new();
        
        /// <summary>
        /// Component variants are sets that define the visual style of a component
        /// for example if the visual element has a type of primary and a size of md do certain things
        /// This can be defined in code like
        /// compoundVariants = {
        ///     ["primary", "md"], "bg-red-500 text-primary-500 text-indigo-500 font-bold rounded-lg"
        /// }
        /// When these condition are met then we will add the class names to the visual element
        /// </summary>
        public List<CompoundVariant> compoundVariants = new();
    }
    
    [Serializable]
    public struct CompoundVariant
    {
        public Variant[] conditions;
        public Variant classNames;
    }
    
    /*
     * singleton to handle
     */
    public class ClassVarianceAuthority : IService
    {
        private HeadWindCssSettings _headWindCssSettings;

        public void Initialize()
        {
            _headWindCssSettings = GetHeadWindCssSettings();
        }
        
        internal HeadWindCssSettings GetHeadWindCssSettings()
        {
            return HeadWindCssSettings.Load();
        }
        
        // private readonly SerializableDictionary<Variant, ClassVariant> _variants = new()
        // {
        //     { // Buttons
        //         ButtonVariant, new ClassVariant {
        //         baseClasses = "font-bold rounded-lg",
        //         variants = new ()
        //         {
        //             {
        //                 "variant", new SerializableDictionary<string, string>
        //                 {
        //                     { "default", "text-primary-500" },
        //                     { "primary", "bg-indigo-600 text-white" },
        //                 }
        //             },
        //             {
        //                 "size", new SerializableDictionary<string, string>()
        //                 {
        //                     { "sm", "h-8" },
        //                     { "md", "h-10" }
        //                 }
        //             }
        //         }}
        //     }
        // };

        public string GetVariant(Variant typeVariant, Variant variant, Variant valueVariant)
        {
            if (_headWindCssSettings.Variants.TryGetValue(typeVariant, out var type))
            {
                string classes = type.baseClasses;
                
                if (type.variants.TryGetValue(variant, out var variantConfig))
                {
                    if (variantConfig.TryGetValue(valueVariant, out var variantClasses))
                    {
                        classes += " " + variantClasses;
                    }
                }

                return classes;
            }

            return String.Empty;
        }
        
        /**
         * TODO add unit test
         * 
         */
        /// <summary>
        /// Add class variant authority to the dictionary
        /// This will style visual elements automatically 
        /// example
        /// Cva("buttonVariant", "flex justify-center items-center", { "variants", { "default", "text-primary-500", "text-red-500", "font-bold" } });
        /// </summary>
        public void Cva(Variant variant, VariantConfig config)
        {
            if(!_headWindCssSettings.Variants.TryAdd(variant, config))
            {
                throw new Exception("Variant already exists");
            }
            
            // if(standardClasses.Length == 0)
            // {
            //     standardClassVariants.Add(variant, new ClassVariant { classes = standardClasses });
            // }
            //
            // if (!variants.ContainsKey(variant))
            // {
            //     variants.Add(variant, new ClassVariant { classes = variantClasses });
            // }
            //
            // if (!compoundVariants.ContainsKey(variant))
            // {
            //     compoundVariants.Add(variant, new ClassVariant { classes = compoundVariantClasses });
            // }
        }
    }
}