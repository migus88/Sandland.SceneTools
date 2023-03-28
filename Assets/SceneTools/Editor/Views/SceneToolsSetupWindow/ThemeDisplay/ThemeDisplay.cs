using System;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Views
{
    internal class ThemeDisplay : RadioButton, IDisposable
    {
        public event Action<AssetFileInfo> Selected;
        
        private readonly AssetFileInfo _themeInfo;
        
        public ThemeDisplay(AssetFileInfo themeInfo) : base()
        {
            _themeInfo = themeInfo;
            
            var visualTree = AssetDatabaseUtils.FindAndLoadVisualTreeAsset(nameof(ThemeDisplay));
            visualTree.CloneTree(this);
            
            AddToClassList("sandland-theme-button");

            var mainStyleSheet = AssetDatabaseUtils.FindAndLoadStyleSheet(nameof(ThemeDisplay));
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(themeInfo.Path);
            
            styleSheets.Add(mainStyleSheet);
            styleSheets.Add(styleSheet);
            
            label = themeInfo.Name;

            this.RegisterValueChangedCallback(OnValueChanged);
        }

        private void OnValueChanged(ChangeEvent<bool> evt)
        {
            if (!evt.newValue)
            {
                return;
            }

            Selected?.Invoke(_themeInfo);
        }

        public void Dispose()
        {
            this.UnregisterValueChangedCallback(OnValueChanged);
        }
    }
}