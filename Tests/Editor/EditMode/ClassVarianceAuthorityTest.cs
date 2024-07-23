using System;

using Moq;
using NUnit.Framework;
using UnityEngine;

namespace HeadWindCSS.Tests.Editor.EditMode
{
    using Domains.Settings.ScriptableObjects;

    using Domains.Serialization;
    using Domains.ServiceProviders.Authority;

    public class ClassVarianceAuthorityTest
    {
        public const string ButtonVariant = "buttonVariants";

        private ClassVarianceAuthority _authority;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<ClassVarianceAuthority>();
            mock.Setup(x => x.GetHeadWindCssSettings()).Returns(
                ScriptableObject.CreateInstance<HeadWindCssSettings>()    
            );
            _authority = mock.Object;
            _authority.Initialize();
        }

        [Test]
        public void AlreadyAddedTest()
        {
            AddButtonVariant();
            Assert.Throws<Exception>(() => _authority.Cva(ButtonVariant, new ClassVariant
            {
                baseClasses = "font-bold rounded-lg",
                variants = new()
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
            }));
        }
        
        [Test]
        public void TestAddVariantClasses()
        {
            AddButtonVariant();
            var properties = _authority.GetVariant(
                typeVariant: ButtonVariant,
                variant: "variant",
                valueVariant: "primary"
            );
            
            Assert.AreEqual("font-bold rounded-lg bg-indigo-600 text-white", properties);
            
            properties = _authority.GetVariant(
                typeVariant: ButtonVariant,
                variant: "variant",
                valueVariant: "secondary"
            );
            
            Assert.AreEqual("font-bold rounded-lg bg-red-600 text-white", properties);
        }
        
        [Test]
        public void TestAddVariantWithSize()
        {
            AddButtonVariant();
            var properties = _authority.GetVariant(
                typeVariant: ButtonVariant,
                variant: "size",
                valueVariant: "md"
            );
            
            Debug.Log(properties);
            
            Assert.AreEqual("font-bold rounded-lg h-10", properties);
        }

        private void AddButtonVariant()
        {
            var variant = new ClassVariant
            {
                baseClasses = "font-bold rounded-lg",
                variants = new()
                {
                    {
                        "variant", new SerializableDictionary<string, string>
                        {
                            { "default", "text-primary-500" },
                            { "primary", "bg-indigo-600 text-white" },
                            { "secondary", "bg-red-600 text-white" }
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
            };
            
            _authority.Cva(ButtonVariant, variant);
        }
    }
}