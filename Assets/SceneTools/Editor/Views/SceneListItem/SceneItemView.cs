using System;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Views
{
    internal class SceneItemView : VisualElement, IDisposable
    {
        private static Texture2D Icon => _icon ??= EditorGUIUtility.IconContent("SceneAsset Icon").image as Texture2D;
        private static Texture2D _icon;

        private readonly Image _iconImage;
        private readonly FavoritesButton _favoritesButton;
        private readonly LinkButton _button;

        private AssetFileInfo _sceneInfo;

        public SceneItemView()
        {
            var visualTree = AssetDatabaseUtils.FindAndLoadVisualTreeAsset("SceneItemView");
            visualTree.CloneTree(this);

            _iconImage = this.Q<Image>("scene-icon");
            _button = this.Q<LinkButton>("scene-button");
            _favoritesButton = this.Q<FavoritesButton>("favorites-button");

            _iconImage.image = Icon;
            _button.Clicked += OnOpenSceneButtonClicked;
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

            _iconImage.AddManipulator(new Clickable(OnIconClick));
        }

        private void OnIconClick()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(_sceneInfo.Path);
        }

        public void Init(AssetFileInfo info)
        {
            _sceneInfo = info;
            _button.text = _sceneInfo.Name;
            _favoritesButton.Init(_sceneInfo);

            ResetInlineStyles();
        }

        private void ResetInlineStyles()
        {
            // ListView sets inline attributes that we want to control from UCSS
            style.height = StyleKeyword.Null;
            style.flexGrow = StyleKeyword.Null;
            style.flexShrink = StyleKeyword.Null;
            style.marginBottom = StyleKeyword.Null;
            style.marginTop = StyleKeyword.Null;
            style.paddingBottom = StyleKeyword.Null;
        }

        private void OnOpenSceneButtonClicked()
        {
            EditorSceneManager.OpenScene(_sceneInfo.Path);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            Dispose();
        }

        public void Dispose()
        {
            UnregisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            _button.Clicked -= OnOpenSceneButtonClicked;
        }
    }
}