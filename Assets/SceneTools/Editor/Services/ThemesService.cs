using System;
using System.Linq;
using Sandland.SceneTool.Editor.Common.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Services
{
    internal static class ThemesService
    {
        public const string DefaultThemeStyleSheetName = "sandland-default";
        
        private const string SelectedThemePathKey = "sandland-selected-theme-guid";

        public static event Action<StyleSheet> ThemeChanged;
        
        public static string SelectedThemePath
        {
            get => EditorPrefs.GetString(SelectedThemePathKey, DefaultThemePath);
            set
            {
                EditorPrefs.SetString(SelectedThemePathKey, value);
                ThemeChanged?.Invoke(GetSelectedTheme());
            }
        }

        // This will throw an exception if not found
        public static string DefaultThemePath =>
            AssetDatabaseUtils.FindAssets<StyleSheet>(DefaultThemeStyleSheetName).First().Path;

        public static StyleSheet GetSelectedTheme()
        {
            var selectedTheme = AssetDatabase.LoadAssetAtPath<StyleSheet>(SelectedThemePath);
            if (selectedTheme != null)
            {
                return selectedTheme;
            }

            SelectedThemePath = DefaultThemePath;
            return GetSelectedTheme();
        }
    }
}