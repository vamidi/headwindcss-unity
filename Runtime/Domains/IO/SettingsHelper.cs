using System;
using System.IO;

using UnityEngine;

namespace HeadWindCSS.Domains.IO
{
    internal static class SettingsHelper
    {
        // C:/MyWork/UnityProject/ProjectSettings
        internal static readonly string SettingsFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "../ProjectSettings"));
        
        internal static T GetOrCreateSettings<T>(string asset) where T : ScriptableObject
        {
            var destination = $"{SettingsFolder}/{asset}";
            
            Debug.Log(destination);
            
            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }
            
            var settings = ResourceHelper.GetAsset<T>(destination);
#if UNITY_EDITOR
            if (settings == null)
            {
                settings = ResourceHelper.CreateAsset<T>(destination);
            }
#endif
            if (settings == null)
            {
                throw new ArgumentException("Settings cannot be null", nameof(settings));
            }

            return settings;
        }
    }
}