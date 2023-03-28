using System.Linq;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Views
{
    internal class SceneSelectorWindow : SceneToolsWindowBase
    {
        private const string WindowNameInternal = "Scene Selector";
        private const string KeyboardShortcut = " %g";
        private const string WindowMenuItem = MenuItems.Tools.Root + WindowNameInternal + KeyboardShortcut;

        public override float MinWidth => 400;
        public override float MinHeight => 600;
        public override string WindowName => WindowNameInternal;
        public override string VisualTreeName => nameof(SceneSelectorWindow);
        public override string StyleSheetName => nameof(SceneSelectorWindow);


        private SceneInfo[] _sceneInfos;
        private SceneInfo[] _filteredSceneInfos;

        private ListView _sceneList;
        private TextField _searchField;

        [MenuItem(WindowMenuItem)]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneSelectorWindow>();
            window.InitWindow();
            window._searchField?.Focus();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            FavoritesService.FavoritesChanged += OnFavoritesChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            FavoritesService.FavoritesChanged -= OnFavoritesChanged;
        }

        protected override void InitGui()
        {
            _sceneInfos = AssetDatabaseUtils.FindScenes();
            _filteredSceneInfos = GetFilteredSceneInfos();

            _sceneList = rootVisualElement.Q<ListView>("scenes-list");
            _sceneList.makeItem = () => new SceneItemView();
            _sceneList.bindItem = InitListItem;
            _sceneList.fixedItemHeight = SceneItemView.FixedHeight;
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

        private SceneInfo[] GetFilteredSceneInfos(string filter = null)
        {
            var isFilterEmpty = string.IsNullOrWhiteSpace(filter);
            
            return _sceneInfos
                .Where(s => isFilterEmpty || s.Name.ToUpper().Contains(filter.ToUpper()))
                .OrderByFavorites()
                .ThenByDescending(s => s.ImportType == SceneImportType.BuildSettings)
                .ThenByDescending(s => s.ImportType == SceneImportType.Addressables)
                .ToArray();
        }

        private void InitListItem(VisualElement element, int dataIndex)
        {
            var sceneView = (SceneItemView)element;
            sceneView.Init(_filteredSceneInfos[dataIndex]);
        }
    }
}