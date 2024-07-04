using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HeadWindCSS.Domains.ServiceProviders;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeadWindCSS.Editor.Domains.SettingsProviders.Authority
{
    using HeadWindCSS.Domains.IO;
    using HeadWindCSS.Domains.Settings.ScriptableObjects;

    public class HeadWindCssSettingsProvider: SettingsProvider
    {
        internal const string SettingsPath = "Project/HeadWindCSS Package";
        
        private string searchContext;
        private VisualElement rootElement;
        
        /// <summary>
        /// The current SceneLoaderSettings used for this project.
        /// <para>
        /// This instance will be available in editor while playing and in any other player builds.
        /// To do so, this CurrentSettings needs to be an asset to be added to the Preloaded Assets
        /// list during the build process.
        /// </para>
        /// <para>See how this is done on <see cref="HeadWindCSS.Editor.Domains.Importers.SettingsLoader"/>.</para>
        /// </summary>
        public static HeadWindCssSettings CurrentSettings
        {
            get
            {
                EditorBuildSettings.TryGetConfigObject(HeadWindCssSettings.CONFIG_NAME, out HeadWindCssSettings settings);
                return settings;
            }
            set
            {
                var remove = value == null;
                if (remove)
                {
                    EditorBuildSettings.RemoveConfigObject(HeadWindCssSettings.CONFIG_NAME);
                }
                else
                {
                    EditorBuildSettings.AddConfigObject(HeadWindCssSettings.CONFIG_NAME, value, overwrite: true);
                }
            }
        }
        
        HeadWindCssSettingsProvider() : base(SettingsPath, SettingsScope.Project)
        {
            label = "HeadWindCSS";
            // CurrentSettings = FindSceneLoaderSettings();
            keywords = GetSearchKeywordsFromGUIContentProperties<HeadWindCssSettings>();

            if (!DynamicUSSExists())
            {
                SaveParsedProperties("");
            }
            
            // activateHandler is called when the user clicks on the Settings item in the Settings window.
            // activateHandler = (_, rootElement) =>
            // {
            //
            // };
        }
        
        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
            var provider = new HeadWindCssSettingsProvider();

            // Automatically extract all keywords from the Styles.
            // provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }
        
        public static void Open()
        {
            SettingsService.OpenProjectSettings(SettingsPath);
        }
        
        /// <summary>
        /// If the user creates their own settings you can override it by creating
        /// one.
        /// </summary>
        /// <returns></returns>
        private static HeadWindCssSettings FindSceneLoaderSettings()
        {
            var filter = $"t:{nameof(HeadWindCssSettings)}";
            var guids = AssetDatabase.FindAssets(filter);
            var hasGuids = guids.Length > 0;
            var path = hasGuids ? AssetDatabase.GUIDToAssetPath(guids[0]) : string.Empty;
 
            return AssetDatabase.LoadAssetAtPath<HeadWindCssSettings>(path);
        }
        
        private static HeadWindCssSettings SaveSceneLoaderAsset()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                title: "Save Scene Loader Settings", defaultName: "DefaultHeadWindCssSettings", extension: "asset",
                message: "Please enter a filename to save the projects Scene Loader settings.");
            var invalidPath = string.IsNullOrEmpty(path);
            if (invalidPath) return null;
 
            var settings = ScriptableObject.CreateInstance<HeadWindCssSettings>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
 
            Selection.activeObject = settings;
            return settings;
        }
        
        public override void OnGUI(string searchContext)
        {
            DrawCurrentSettingsGUI();
            EditorGUILayout.Space();
 
            var invalidSettings = CurrentSettings== null;
            if (invalidSettings) DisplaySettingsCreationGUI();
            else base.OnGUI(searchContext);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            
        }
        
        private void DrawCurrentSettingsGUI()
        {
            EditorGUI.BeginChangeCheck();
 
            EditorGUI.indentLevel++;
            var settings = EditorGUILayout.ObjectField("Current Settings", CurrentSettings,
                typeof(HeadWindCssSettings), allowSceneObjects: false) as HeadWindCssSettings;
            if (settings) DrawCurrentSettingsMessage();
            EditorGUI.indentLevel--;
 
            var newSettings = EditorGUI.EndChangeCheck();
            if (newSettings)
            {
                CurrentSettings = settings;
                RefreshEditor();
            }
        }
 
        private void RefreshEditor() => base.OnActivate(searchContext, rootElement);
        
        private void DrawCurrentSettingsMessage()
        {
            const string message = "This is the current Scene Loader Settings and " +
                                   "it will be automatically included into any builds.";
            EditorGUILayout.HelpBox(message, MessageType.Info, wide: true);
            
            if (GUILayout.Button("Parse dynamic values from XML"))
            {
                // find all uxml documents
                var awaiter = GrabPropertiesFromUxml(ResourceHelper.GetAssetsOfExtension("uxml")).GetAwaiter();
                awaiter.OnCompleted(() =>
                {
                    var dynamicUSS = ResourceHelper.GetAsset<StyleSheet>(SettingsHelper.USSPath, true);

                    if (dynamicUSS)
                    {
                        EditorUtility.SetDirty(dynamicUSS);
                        AssetDatabase.SaveAssets();
                        
                        EditorUtility.DisplayDialog("Dynamic Properties Parsed",
                            "The dynamic properties have been parsed and saved to the dynamic stylesheet.",
                            "Ok");
                    }                    
                });
            }
        }
        
        private void DisplaySettingsCreationGUI()
        {
            const string message = "You have no Scene Loader Settings. Would you like to create one?";
            EditorGUILayout.HelpBox(message, MessageType.Info, wide: true);
            var openCreationdialog = GUILayout.Button("Create");
            if (openCreationdialog)
            {
                CurrentSettings = SaveSceneLoaderAsset();
            }
        }
        
        private bool DynamicUSSExists()
        {
            return File.Exists(SettingsHelper.USSPath);
        }

        private async Task GrabPropertiesFromUxml(string[] files)
        {
            if(files.Length == 0) return;

            List<string> allProperties = new();
            foreach (var file in files)
            {
                var text = await File.ReadAllTextAsync(file);
                
                // Get all class properties
                var classRegexCheck = "class=\"([^\"]+)\"";
                var matches = Regex.Matches(text, classRegexCheck);
                
                if(matches.Count == 0) continue;
                
                foreach (Match match in matches)
                {
                    var properties = match.Value.Substring(match.Value.IndexOf("=", StringComparison.Ordinal) + 1).Replace("\"", "");
                    var parsedProperties = await ServiceLocator.Current.Get<UxmlHelper>().NewParseClassProperties(properties);
                    
                    for (var i = 0; i < parsedProperties.Key.Count; i++)
                    {
                        var propPrefixAndValue = parsedProperties.Key[i];
                        var styleSheetPropValue = parsedProperties.Value[i];
                        
                        // Add properties to the settings
                        Debug.Log($"propPrefixAndValue: {propPrefixAndValue}, styleSheetPropValue: {styleSheetPropValue}");
                        CurrentSettings.AddDynamicValue(propPrefixAndValue.Trim(), styleSheetPropValue.Trim());
                    }
                }
            }
            
            // TODO Grab the properties from the settings that are defined in the theme
            
            // Save the parsed properties to the dynamic stylesheet 
            var stylesheet = "";
            foreach (var dynamicProperty in CurrentSettings.DynamicProperties)
            {
                stylesheet += dynamicProperty.Value + Environment.NewLine;
            }
                
            await SaveParsedProperties(stylesheet);
        }
        
        /// <summary>
        /// Save the parsed properties to the file
        /// </summary>
        /// <param name="properties"></param>
        private Task SaveParsedProperties(string properties)
        {
            if (!Directory.Exists(SettingsHelper.USSFolderPath))
            {
                Directory.CreateDirectory(SettingsHelper.USSFolderPath);
            }

            if (!File.Exists(SettingsHelper.USSPath))
            {
                ResourceHelper.CreateAsset<StyleSheet>(SettingsHelper.USSPath);
            }

            return File.WriteAllTextAsync(SettingsHelper.USSPath, properties);
            // File.WriteAllText(USSPath, properties);
        }
    }
}