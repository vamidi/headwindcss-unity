using HeadWindCSS.Domains.ServiceProviders.Authority;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeadWindCSS.Domains.UI.Elements
{
    using Extensions.UI.Elements;
    public class Button: UnityEngine.UIElements.Button
    {
        // private static readonly CustomStyleProperty<string> m_ColorPrimary900 = new CustomStyleProperty<string>("--test");

        // public Button()
        // {
        //     RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
        // }
        //
        // private void OnCustomStyleResolved(CustomStyleResolvedEvent evt)
        // {
        //     
        //     if (evt.customStyle.TryGetValue(m_ColorPrimary900, out var colorPrimary900))
        //     {
        //         // Do something with the color.
        //         Debug.Log($"got it! {colorPrimary900}");
        //     }
        // }
        
        public new class UxmlFactory : UxmlFactory<Button, UxmlTraits> {}

        public new class UxmlTraits : UnityEngine.UIElements.Button.UxmlTraits
        {
            protected readonly UxmlStringAttributeDescription StatusAttr = new (){ name = "variant", defaultValue = "primary" };
            protected readonly UxmlStringAttributeDescription SizeAttr = new (){ name = "size", defaultValue = "md" };
            protected readonly UxmlStringAttributeDescription ClassAttr = new (){ name = "class" };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                
                // ve.AddVariantClasses(ClassVarianceAuthority.ButtonVariant, StatusAttr, bag, cc);
                // ve.AddVariantClasses(ClassVarianceAuthority.ButtonVariant, SizeAttr, bag, cc);
                ve.AddVariantClasses(ClassVarianceAuthority.ButtonVariant, ClassAttr, bag, cc);

                var btn = ve as UnityEngine.UIElements.Button;

                if (btn == null)
                {
                    return;
                }
                
                btn.text = "Button";
                
            }
        }
    }
}