using System;
using System.Collections.Generic;
using System.Linq;
using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Sandland.SceneTool.Editor.Common.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Sandland.SceneTool.Editor.Views
{
    internal class SceneSelectorWindow : EditorWindow
    {
        private const string WindowName = "Scene Selector";
        private const string KeyboardShortcut = " %g";
        private const string WindowMenuItem = MenuItems.Tools.Root + WindowName + KeyboardShortcut;
        private const float MinWidth = 400;
        private const float MinHeight = 400;
        
        private static Texture2D Icon => _icon ??= EditorGUIUtility.IconContent("d_UnityLogo").image as Texture2D;
        private static Texture2D _icon;

        private AssetFileInfo[] _sceneInfos;
        private AssetFileInfo[] _filteredSceneInfos;

        private ListView _sceneList;
        private TextField _searchField;

        [MenuItem(WindowMenuItem)]
        public static void ShowWindow()
        {
            if (!SceneSelectorService.HasConfig)
            {
                SceneSelectorService.UI.AskToCreateConfig();
            }

            var window = GetWindow<SceneSelectorWindow>();
            window.minSize = new Vector2(MinWidth, MinHeight);
            window.titleContent = new GUIContent(WindowName, Icon);
            window._searchField?.Focus();
        }

        public void CreateGUI()
        {
            var visualTree = AssetDatabaseUtils.FindAndLoadVisualTreeAsset("SceneSelectorWindow");
            visualTree.CloneTree(rootVisualElement);

            var styleSheet = AssetDatabaseUtils.FindAndLoadStyleSheet("SceneSelectorWindow");
            rootVisualElement.styleSheets.Add(styleSheet);
            Init();
        }

        private void OnEnable()
        {
            FavoritesService.FavoritesChanged += OnFavoritesChanged;
        }

        private void OnDisable()
        {
            FavoritesService.FavoritesChanged -= OnFavoritesChanged;
        }

        private void Init()
        {
            _sceneInfos = AssetDatabaseUtils.FindScenes();
            _filteredSceneInfos = GetFilteredSceneInfos();

            _sceneList = rootVisualElement.Q<ListView>("scenes-list");
            _sceneList.makeItem = () => new SceneItemView();
            _sceneList.bindItem = InitListItem;
            _sceneList.itemsSource = _filteredSceneInfos;

            _searchField = rootVisualElement.Q<TextField>("scenes-search");
            _searchField.RegisterValueChangedCallback(OnSearchValueChanged);
            _searchField.Focus();
            _searchField.SelectAll();
        }

        private void OnSearchValueChanged(ChangeEvent<string> @event) => RebuildItems(@event.newValue);

        private void OnFavoritesChanged() => RebuildItems();

        private void RebuildItems(string filter = null)
        {
            _filteredSceneInfos = GetFilteredSceneInfos(filter);
            _sceneList.itemsSource = _filteredSceneInfos;
            _sceneList.Rebuild();
        }

        private AssetFileInfo[] GetFilteredSceneInfos(string filter = null)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return _sceneInfos
                    .OrderByFavorites()
                    .ToArray();
            }

            return _sceneInfos
                .Where(s => s.Name.ToUpper().Contains(filter.ToUpper()))
                .OrderByFavorites()
                .ToArray();
        }

        private void InitListItem(VisualElement element, int dataIndex)
        {
            var sceneView = (SceneItemView)element;
            sceneView.Init(_filteredSceneInfos[dataIndex]);
        }
    }
}