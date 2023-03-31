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
        public const float FixedHeight = 100;
        
        private readonly Image _iconImage;
        private readonly FavoritesButton _favoritesButton;
        private readonly Label _button;
        private readonly Label _typeLabel;
        private readonly VisualElement _textWrapper;
        private readonly Clickable _clickManipulator;

        private AssetFileInfo _sceneInfo;

        public SceneItemView()
        {
            var visualTree = AssetDatabaseUtils.FindAndLoadVisualTreeAsset("SceneItemView");
            visualTree.CloneTree(this);

            _iconImage = this.Q<Image>("scene-icon");
            _button = this.Q<Label>("scene-button");
            _favoritesButton = this.Q<FavoritesButton>("favorites-button");
            _typeLabel = this.Q<Label>("scene-type-label");
            _textWrapper = this.Q<VisualElement>("scene-text-wrapper");

            _clickManipulator = new Clickable(OnOpenSceneButtonClicked);
            _textWrapper.AddManipulator(_clickManipulator);
            
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

            _iconImage.AddManipulator(new Clickable(OnIconClick));
        }

        private void OnIconClick()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(_sceneInfo.Path);
        }

        public void Init(SceneInfo info)
        {
            _sceneInfo = info;
            _button.text = _sceneInfo.Name;
            _favoritesButton.Init(_sceneInfo);
            _typeLabel.text = info.ImportType.ToDescription();

            // TODO: Support dynamic themes
            _iconImage.image = Icons.GetSceneIcon(true);

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
        }
    }
}