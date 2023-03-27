using Sandland.SceneTool.Editor.Common.Utils;
using UnityEditor;
using UnityEngine;

namespace Sandland.SceneTool.Editor.Views
{
    internal abstract class SceneToolsWindowBase : EditorWindow
    {
        private const string DefaultThemeStyleSheetName = "sandland-default-theme";
        private const string GlobalStyleSheetName = "SceneToolsMain";

        public abstract float MinWidth { get; }
        public abstract float MinHeight { get; }
        public abstract string WindowName { get; }
        public abstract string VisualTreeName { get; }
        public abstract string StyleSheetName { get; }


        protected void InitWindow(Texture2D overrideIcon = null)
        {
            minSize = new Vector2(MinWidth, MinHeight);
            titleContent = new GUIContent(WindowName, overrideIcon ? overrideIcon : Icons.SceneToolIcon);

            if (docked)
            {
                return;
            }

            var editorPos = EditorGUIUtility.GetMainWindowPosition();
            var x = editorPos.x + editorPos.width * 0.5f - MinWidth * 0.5f;
            var y = editorPos.y + editorPos.height * 0.5f - MinHeight * 0.5f;

            position = new Rect(x, y, MinWidth, MinHeight);
        }


        public virtual void CreateGUI()
        {
            var visualTree = AssetDatabaseUtils.FindAndLoadVisualTreeAsset(VisualTreeName);
            visualTree.CloneTree(rootVisualElement);

            var defaultTheme = AssetDatabaseUtils.FindAndLoadStyleSheet(DefaultThemeStyleSheetName);
            var globalStyleSheet = AssetDatabaseUtils.FindAndLoadStyleSheet(GlobalStyleSheetName);
            var styleSheet = AssetDatabaseUtils.FindAndLoadStyleSheet(StyleSheetName);
            rootVisualElement.styleSheets.Add(defaultTheme);
            rootVisualElement.styleSheets.Add(globalStyleSheet);
            rootVisualElement.styleSheets.Add(styleSheet);
            InitGui();
        }

        protected abstract void InitGui();
    }
}