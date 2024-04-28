using UnityEngine.UIElements;

namespace HeadWindCSS.Domains.UI.Elements
{
    using Extensions.UI.Elements;
    public class Button: UnityEngine.UIElements.Button
    {
        public string Variant { get; set; }
        
        public string VariantSize { get; set; }
        
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
            UxmlStringAttributeDescription _variantAttr = new (){ name = "variant", defaultValue = "" };
            UxmlStringAttributeDescription _sizeAttr = new (){ name = "size", defaultValue = "" };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                
                if (ve is Button btn)
                {
                    btn.text = "Button";
                    btn.Variant = _variantAttr.GetValueFromBag(bag, cc); // ve.GetVariant(bag, cc);
                    btn.VariantSize = _sizeAttr.GetValueFromBag(bag, cc);  // ve.GetSize(bag, cc);
                }

                ve.SetupVariant(
                    typeVariant: "buttonVariants",
                    attributes: new() { _variantAttr, _sizeAttr },
                    bag: bag, 
                    cc: cc);
            }
        }
    }
}