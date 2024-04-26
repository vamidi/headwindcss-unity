using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeadWindCSS.Domains.Resources
{
	internal static class UIResourceHelper
	{
		const string ResourceRoot = "Packages/com.vamidicreations.headwindcss/Runtime/Domains/Resources";
		const string TemplateRoot = "Packages/com.vamidicreations.headwindcss/Runtime/Domains/UI/Templates";
		const string StyleRoot = "Packages/com.vamidicreations.headwindcss/Runtime/Domains/UI/Styles";

		public static string GetStyleSheetPath(string filename) => $"{StyleRoot}/{filename}.uss";

		public static string GetResourcePath(string filename) => $"{ResourceRoot}/{filename}.asset";

		static string TemplatePath(string filename) => $"{TemplateRoot}/{filename}.uxml";

		public static VisualTreeAsset GetTemplateAsset(string templateFilename)
		{
			var path = TemplatePath(templateFilename);

			var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
			if (asset == null)
				throw new FileNotFoundException("Failed to load UI Template at path " + path);
			return asset;
		}

		public static TemplateContainer GetTemplate(string templateFilename)
		{
			return GetTemplateAsset(templateFilename).CloneTree();
		}

		public static StyleSheet GetStyleAsset(string styleFileName)
		{
			var path = GetStyleSheetPath(styleFileName);

			var asset = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);

			if (asset == null)
				throw new FileNotFoundException("Failed to load UI Styles at path " + path);
			return asset;
		}

		public static Object GetObject(string objectFilename)
		{
			var path = GetResourcePath(objectFilename);

			var asset = AssetDatabase.LoadAssetAtPath<Object>(path);

			if (asset == null)
				throw new FileNotFoundException("Failed to load UI Template at path " + path);
			return asset;
		}
	}
}