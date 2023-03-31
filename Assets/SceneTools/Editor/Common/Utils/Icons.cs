using System;
using Sandland.SceneTool.Editor.Common.Data;
using UnityEditor;
using UnityEngine;

namespace Sandland.SceneTool.Editor.Common.Utils
{
    internal static class Icons
    {
        private const string IconsRoot = "Sandland/Images/";
        
        public static Texture2D BrightSceneToolIcon => _brightSceneToolIcon ??=
            Resources.Load<Texture2D>($"{IconsRoot}tool_icon_bright"); 
        public static Texture2D DarkSceneToolIcon => _darkSceneToolIcon ??=
            Resources.Load<Texture2D>($"{IconsRoot}tool_icon_dark");
        public static Texture2D BrightSceneIcon => _brightSceneIcon ??=
            Resources.Load<Texture2D>($"{IconsRoot}scene_icon_bright"); 
        public static Texture2D DarkSceneIcon => _darkSceneIcon ??=
            Resources.Load<Texture2D>($"{IconsRoot}scene_icon_dark"); 
        
        private static Texture2D _brightSceneToolIcon;
        private static Texture2D _darkSceneToolIcon;
        private static Texture2D _brightSceneIcon;
        private static Texture2D _darkSceneIcon;

        public static Texture2D GetSceneIcon(bool isDarkTheme) => isDarkTheme ? BrightSceneIcon : DarkSceneIcon;
        public static Texture2D GetSceneToolIcon(bool isDarkTheme) => isDarkTheme ? BrightSceneToolIcon : DarkSceneToolIcon;

    }
}