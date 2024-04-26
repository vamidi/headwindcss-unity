using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HeadWindCSS.Editor.Domains.Importers
{
    using SettingsProviders.Authority;

    public sealed class SettingsLoader : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
 
        public void OnPreprocessBuild(BuildReport report)
        {
            var settings = HeadWindCssSettingsProvider.CurrentSettings;
            var settingsType = settings.GetType();
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            // Removes all references from SceneLoaderSettings.
            preloadedAssets.RemoveAll(matchedSettings => matchedSettings.GetType() == settingsType);
            // Adds the Current SceneLoaderSettings.
            preloadedAssets.Add(settings);
 
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }
    }
}