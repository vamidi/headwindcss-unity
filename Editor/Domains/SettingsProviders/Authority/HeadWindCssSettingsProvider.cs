using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeadWindCSS.Editor.Domains.SettingsProviders.Authority
{
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
 
            var invalidSettings = CurrentSettings == null;
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
    }
}