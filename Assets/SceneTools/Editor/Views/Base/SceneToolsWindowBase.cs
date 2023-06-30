using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Views.Base
{
    internal abstract class SceneToolsWindowBase : EditorWindow
    {
        private const string GlobalStyleSheetName = "SceneToolsMain";

        public abstract float MinWidth { get; }
        public abstract float MinHeight { get; }
        public abstract string WindowName { get; }
        public abstract string VisualTreeName { get; }
        public abstract string StyleSheetName { get; }

        private StyleSheet _theme;


        protected void InitWindow(Texture2D overrideIcon = null)
        {
            minSize = new Vector2(MinWidth, MinHeight);
            // TODO: support dynamic theme
            titleContent = new GUIContent(WindowName, overrideIcon ? overrideIcon : Icons.GetSceneToolIcon(true));

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

            _theme = ThemesService.GetSelectedTheme();
            
            var globalStyleSheet = AssetDatabaseUtils.FindAndLoadStyleSheet(GlobalStyleSheetName);
            var styleSheet = AssetDatabaseUtils.FindAndLoadStyleSheet(StyleSheetName);
            
            rootVisualElement.styleSheets.Add(_theme);
            rootVisualElement.styleSheets.Add(globalStyleSheet);
            rootVisualElement.styleSheets.Add(styleSheet);
            InitGui();
        }

        protected virtual void OnEnable()
        {
            ThemesService.ThemeChanged += RefreshTheme;
        }

        protected virtual void OnDisable()
        {
            ThemesService.ThemeChanged -= RefreshTheme;
        }

        protected abstract void InitGui();

        protected void RefreshTheme(StyleSheet theme)
        {
            if (_theme == theme)
            {
                return;
            }
            
            if(_theme != null)
            {
                rootVisualElement.styleSheets.Remove(_theme);
            }

            _theme = theme;
            rootVisualElement.styleSheets.Add(_theme);
        }
    }
}