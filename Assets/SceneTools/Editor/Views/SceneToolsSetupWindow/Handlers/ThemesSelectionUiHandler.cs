using System;
using System.Collections.Generic;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Views.Handlers
{
    public class ThemesSelectionUiHandler : ISceneToolsSetupUiHandler
    {
        private readonly RadioButtonGroup _root;
        private readonly ThemeDisplay[] _themeDisplays;

        private string _selectedThemePath;
        
        public ThemesSelectionUiHandler(VisualElement root)
        {
            _root = root.Q<RadioButtonGroup>("theme-selection-group");
            var styleSheets = AssetDatabaseUtils.FindAssets<StyleSheet>("l:Sandland-theme");
            _themeDisplays = new ThemeDisplay[styleSheets.Length];
            _selectedThemePath = ThemesService.SelectedThemePath;

            for (var i = 0; i < styleSheets.Length; i++)
            {
                var styleSheetInfo = styleSheets[i];
                
                var themeDisplay = new ThemeDisplay(styleSheetInfo);
                _themeDisplays[i] = themeDisplay;

                if (styleSheetInfo.Path == _selectedThemePath)
                {
                    themeDisplay.SetValueWithoutNotify(true);
                }
                
                _root.Add(themeDisplay);
            }
        }

        private void OnThemeSelected(AssetFileInfo info)
        {
            _selectedThemePath = info.Path;
        }

        public void SubscribeToEvents()
        {
            foreach (var themeDisplay in _themeDisplays)
            {
                themeDisplay.Selected += OnThemeSelected;
            }
        }

        public void UnsubscribeFromEvents()
        {
            foreach (var themeDisplay in _themeDisplays)
            {
                themeDisplay.Selected -= OnThemeSelected;
            }
        }

        public void Apply()
        {
            ThemesService.SelectedThemePath = _selectedThemePath;
        }
    }
}