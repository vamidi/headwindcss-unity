using HeadWindCSS.Domains.Extensions.UI.Elements;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace HeadWindCSS.Tests.Editor.EditMode
{
    using Domains.Settings.Theme;
    using Domains.ServiceProviders;
    using Domains.ServiceProviders.Authority;
    using Domains.Settings.ScriptableObjects;

    public class UxmlHelperTest
    {
        Mock<ClassVarianceAuthority> _classVarianceAuthority;
        Mock<UxmlHelper> _uxmlHelper;
        
        [SetUp]
        public void Setup()
        {
            var headWindCssSettings = ScriptableObject.CreateInstance<HeadWindCssSettings>(); 

            if(headWindCssSettings.Theme.TryGetValue("colors", out var colorTheme))
            {
                colorTheme.Add("example", new ThemeSettingObject(
                    type: ThemeSettingType.Object,
                    key: "example",
                    value: new()
                    {
                        { "900", ColorHelper.ParseHtmlColor("#181c52") }
                    }
                ));
            }
            
            _classVarianceAuthority = new Mock<ClassVarianceAuthority>();
            _classVarianceAuthority.Setup(x => x.GetHeadWindCssSettings()).Returns(headWindCssSettings);
            _classVarianceAuthority.Object.Initialize();
            _classVarianceAuthority.Object.Cva(
                // Example for Buttons
                ClassVarianceAuthorityTest.ButtonVariant, new ClassVariant {
                baseClasses = "font-bold rounded-lg",
                variants = new ()
                {
                    {
                        "variant", new ()
                        {
                            { "default", "text-primary-500" },
                            { "primary", "bg-indigo-600 text-white" }, 
                        }
                    },
                    {
                        "size", new ()
                        {
                            { "sm", "h-8" },
                            { "md", "h-10" }
                        }
                    }
                }
            }
            );
            
            _uxmlHelper = new Mock<UxmlHelper>();
            _uxmlHelper.Setup(x => x.GetHeadWindCssSettings()).Returns(headWindCssSettings);
            _uxmlHelper.Object.Initialize();
        }

        [Test]
        public void ParseDynamicColors()
        {
            var pair = _uxmlHelper.Object.ParseDynamicColors("bg-primary text-secondary text-example-900");

            // There should be 3 classes
            Assert.AreEqual(pair.Key.Count, 3);
            
            // Test the class names
            Assert.AreEqual(pair.Key[0], "bg-primary");
            Assert.AreEqual(pair.Key[1], "text-secondary");
            Assert.AreEqual(pair.Key[2], "text-example-900");

            // Test the actual stylesheet
            Assert.AreEqual(pair.Value.Count, 3);
            Assert.AreEqual(pair.Value[0], ".bg-primary{background-color: #181C52;}");
            Assert.AreEqual(pair.Value[1], ".text-secondary{color: #0756F1;}");
            Assert.AreEqual(pair.Value[2], ".text-example-900{color: #181C52;}");
        }
    }
}