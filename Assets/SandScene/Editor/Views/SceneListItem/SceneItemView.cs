using System;
using SandScene.Editor.Common.Utils;
using SandScene.Editor.SandScene.Editor.Common.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace SandScene.Editor.Views
{
    public class SceneItemView : VisualElement, IDisposable 
    {
        private readonly Label _sceneNameLabel;
        private readonly Button _navigateToSceneButton;
        private AssetFileInfo _sceneInfo;
        
        public SceneItemView()
        {
            var visualTree = AssetDatabaseUtils.FindAndLoadVisualTreeAsset("SceneItemView");
            visualTree.CloneTree(this);

            _sceneNameLabel = this.Q<Label>("scene-name-label");
            _navigateToSceneButton = this.Q<Button>("scene-navigate-button");
            _navigateToSceneButton.clicked += OnNavigateToSceneButtonClicked;
        }

        public void Init(AssetFileInfo info)
        {
            _sceneInfo = info;
            _sceneNameLabel.text = _sceneInfo.Name;
            
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

        private void OnNavigateToSceneButtonClicked()
        {
            EditorSceneManager.OpenScene(_sceneInfo.Path);
        }

        public void Dispose()
        {
            _navigateToSceneButton.clicked -= OnNavigateToSceneButtonClicked;
        }
    }
}