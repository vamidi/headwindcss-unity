using HeadWindCSS.Domains.ServiceProviders.Authority;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HeadWindCSS.Editor.Domains.Installers
{
    using HeadWindCSS.Domains.ServiceProviders;

    [InitializeOnLoad]
    public class GlobalInstaller
    {
        static GlobalInstaller()
        {
            Debug.Log("Setting up DI");
            // Initialize default service locator.
            ServiceLocator.Initialize();
            
            ServiceLocator.Current.Register(new ClassVarianceAuthority());
            ServiceLocator.Current.Register(new UxmlHelper());

            // Debug.Log(ServiceLocator.Current.Get<UxmlHelper>());

        }

        [DidReloadScripts]
        static void OnScriptReloaded()
        {
            Debug.Log("Reloading scripts");
        }
    }
}