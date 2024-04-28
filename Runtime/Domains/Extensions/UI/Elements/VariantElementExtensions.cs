using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeadWindCSS.Domains.Extensions.UI.Elements
{
    using IO;
    using Resources;
    using ServiceProviders;
    using ServiceProviders.Authority;

    public static class VariantElementExtensions
    {
        internal const string HeadWindCss = "Headwindcss";
        
        internal static readonly UxmlStringAttributeDescription ClassAttr = new (){ name = "class" };
        
        // public static string GetVariant(this VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        // {
        //     return VariantAttr.GetValueFromBag(bag, cc);
        // }
        //
        // public static string GetSize(this VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        // {
        //     return VariantAttr.GetValueFromBag(bag, cc);
        // }
        
        public static void SetupVariant(
            this VisualElement ve, 
            string typeVariant, List<UxmlStringAttributeDescription> attributes, 
            IUxmlAttributes bag, CreationContext cc
        ) {
// #if UNITY_EDITOR
//             // Load styles from the editor resources
//             // Failed to load UI Styles at path Packages/com.vamidicreations.headwindcss/Runtime/Domains/UI/Styles/headwindcss.uss
//             var dynamicStyleSheet = ResourceHelper.GetAsset<StyleSheet>(SettingsHelper.USSPath, true);
// #else
//                 // Load styles from the runtime resources
//             var dynamicStyleSheet = UIResourceHelper.GetStyleAsset(UxmlHelper.HeadWindDynamicCssFileName);
// #endif
//             
//             // unload the dynamic style first
//             if (dynamicStyleSheet && ve.styleSheets.Contains(dynamicStyleSheet))
//             {
//                 ve.styleSheets.Remove(dynamicStyleSheet);
//             }
            
#if UNITY_EDITOR
            // Load styles from the editor resources
            // Failed to load UI Styles at path Packages/com.vamidicreations.headwindcss/Runtime/Domains/UI/Styles/headwindcss.uss
            var stylesheet = UIResourceHelper.GetStyleAsset(HeadWindCss);
#else
                // Load styles from the runtime resources
                var stylesheet = UIResourceHelper.GetStyleAsset("headwindcss");
#endif
            // Unity is already tracking if USS has been added to the styleSheets
            ve.styleSheets.Add(stylesheet);

            foreach (var attribute in attributes)
            {
                ve.AddVariantClasses(typeVariant, attribute, bag, cc);
            }
            
            AssignDynamicProperties(ve, ClassAttr.GetValueFromBag(bag, cc));
#if UNITY_EDITOR
            // Load styles from the editor resources
            // Failed to load UI Styles at path Packages/com.vamidicreations.headwindcss/Runtime/Domains/UI/Styles/headwindcss.uss
            var dynamicStyleSheet = ResourceHelper.GetAsset<StyleSheet>(SettingsHelper.USSPath, true);
#else
                // Load styles from the runtime resources
            var dynamicStyleSheet = UIResourceHelper.GetStyleAsset(UxmlHelper.HeadWindDynamicCssFileName);
#endif

            if (!dynamicStyleSheet)
            {
                return;
            }
            
            // re-add the dynamic style
            ve.styleSheets.Add(dynamicStyleSheet);
        }
        
        private  static void AddVariantClasses(
            this VisualElement ve,
            string typeVariant, UxmlStringAttributeDescription attribute, 
            IUxmlAttributes bag, CreationContext cc
        ) {
            var classProperties = ServiceLocator.Current.Get<ClassVarianceAuthority>()
                .GetVariant(
                    typeVariant: typeVariant,
                    variant: attribute.name,
                    valueVariant: attribute.GetValueFromBag(bag, cc)
                );
            
            // Add the variant classes to the element
            var classes = classProperties.Split(' ');
            foreach (var c in classes)
            {
                ve.AddToClassList(c);
            }
        }

        private static async void AssignDynamicProperties(VisualElement ve, string properties)
        {
            var uxmlHelper = ServiceLocator.Current.Get<UxmlHelper>();
                
            var dynamicProperties = await uxmlHelper.ParseClassProperties(properties);
            foreach (var prop in dynamicProperties)
            {
                ve.AddToClassList(prop);
            }
        }
    }
}