using System;
using System.Collections.Generic;
using System.Linq;
using SandScene.Editor.Common.Utils;
using SandScene.Editor.SandScene.Editor.Common.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SandScene.Editor.Views
{
    internal class SceneSelectorWindow : EditorWindow
    {
        private const string WindowName = "Scene Selector";
        private const string KeyboardShortcut = " %g";
        private const string WindowMenuItem = MenuItems.Root + WindowName + KeyboardShortcut;

        private AssetFileInfo[] _sceneInfos;
        private AssetFileInfo[] _filteredSceneInfos;

        private ListView _sceneList;
        private TextField _searchField;

        [MenuItem(WindowMenuItem)]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneSelectorWindow>();
            window.titleContent = new GUIContent(WindowName);
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

        private void Init()
        {
            _sceneInfos = AssetDatabaseUtils.FindScenes();
            _filteredSceneInfos = _sceneInfos;

            _sceneList = rootVisualElement.Q<ListView>("scenes-list");
            _sceneList.makeItem = () => new SceneItemView();
            _sceneList.bindItem = InitListItem;
            _sceneList.itemsSource = _filteredSceneInfos;

            _searchField = rootVisualElement.Q<TextField>("scenes-search");
            _searchField.RegisterValueChangedCallback(OnSearchValueChanged);
            _searchField.Focus();
        }

        private void OnSearchValueChanged(ChangeEvent<string> @event)
        {
            var filter = @event.newValue;

            _filteredSceneInfos = string.IsNullOrWhiteSpace(filter)
                ? _sceneInfos
                : _sceneInfos.Where(s => s.Name.ToUpper().Contains(filter.ToUpper())).ToArray();
            _sceneList.itemsSource = _filteredSceneInfos;
            _sceneList.Rebuild();
        }

        private void InitListItem(VisualElement element, int dataIndex)
        {
            var sceneView = (SceneItemView)element;
            sceneView.Init(_filteredSceneInfos[dataIndex]);
        }
    }
}