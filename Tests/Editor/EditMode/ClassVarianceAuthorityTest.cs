
using System;
using NUnit.Framework;

namespace HeadWindCSS.Tests.Editor.EditMode
{
    using Domains.Serialization;
    using Domains.ServiceProviders.Authority;

    public class ClassVarianceAuthorityTest
    {
        protected ClassVarianceAuthority Authority;

        [SetUp]
        public void Setup()
        {
            Authority = new ClassVarianceAuthority();
        }

        [Test]
        public void TestAddVariantClasses()
        {
            // TODO move to setup
            Assert.Throws<Exception>(() => Authority.Cva(ClassVarianceAuthority.ButtonVariant, new ClassVariant
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
            
            Authority.GetVariant(
                typeVariant: ClassVarianceAuthority.ButtonVariant,
                variant: "variant",
                valueVariant: "default"
            );
            // Assert.AreEqual("font-bold rounded-lg bg-indigo-600 text-white", classes);
        }
    }
}