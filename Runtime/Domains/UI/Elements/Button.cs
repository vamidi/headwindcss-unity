using HeadWindCSS.Domains.ServiceProviders.Authority;
using UnityEngine.UIElements;

namespace HeadWindCSS.Domains.UI.Elements
{
    using Extensions.UI.Elements;
    public class Button: UnityEngine.UIElements.Button
    {
        public string Variant { get; set; }
        
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
            protected readonly UxmlStringAttributeDescription StatusAttr = new (){ name = "variant", defaultValue = "" };
            protected readonly UxmlStringAttributeDescription SizeAttr = new (){ name = "size", defaultValue = "" };
            protected readonly UxmlStringAttributeDescription ClassAttr = new (){ name = "class" };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is Button btn)
                {
                    btn.text = "Button";
                    btn.Variant = StatusAttr.GetValueFromBag(bag, cc);
                }
                
                ve.AddVariantClasses(ClassVarianceAuthority.ButtonVariant, StatusAttr, bag, cc);
                ve.AddVariantClasses(ClassVarianceAuthority.ButtonVariant, SizeAttr, bag, cc);
                ve.AddVariantClasses(ClassVarianceAuthority.ButtonVariant, ClassAttr, bag, cc);

            }
        }
    }
}