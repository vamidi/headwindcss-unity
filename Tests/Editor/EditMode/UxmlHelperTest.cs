using System.Threading.Tasks;
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
                        { "900", "#181c52" }
                    }
                ));
            }
            
            _classVarianceAuthority = new Mock<ClassVarianceAuthority>();
            _classVarianceAuthority.Setup(x => x.GetHeadWindCssSettings()).Returns(headWindCssSettings);
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
            });
            
            _uxmlHelper = new Mock<UxmlHelper>();
            _uxmlHelper.Setup(x => x.GetHeadWindCssSettings()).Returns(headWindCssSettings);
        }

        [Test]
        public async Task ParseDynamicColors()
        {
            var pair = await _uxmlHelper.Object.ParseDynamicColors("bg-primary text-secondary text-example-900");

            // Test the class names
            Assert.AreEqual("bg-primary text-secondary text-example-900", pair.Key);

            // Test the actual stylesheet
            Assert.AreEqual(
                ".primary{background-color: #181c52;} .secondary{color: #0756f1;} .text-example-900{color: #181c52;}", 
                pair.Value
            );
        }
    }
}