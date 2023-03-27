using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Views
{
    internal class FavoritesButton : VisualElement
    {
        private static Texture2D Icon => _icon ??= EditorGUIUtility.IconContent("Favorite On Icon").image as Texture2D;
        private static Texture2D _icon;
        
        public bool IsFavorite { get; private set; }

        private Image _starImage;
        private AssetFileInfo _fileInfo;
        private bool _isUiInitialized = false;

        public void Init(AssetFileInfo info)
        {
            _fileInfo = info;
            
            InitializeUI();

            var isFavorite = _fileInfo.IsFavorite();
            SetState(isFavorite);
        }

        private void InitializeUI()
        {
            if (_isUiInitialized)
            {
                return;
            }

            _starImage = new Image
            {
                image = Icon
            };
            Add(_starImage);

            this.AddManipulator(new Clickable(OnClick));

            _isUiInitialized = true;
        }

        private void OnClick()
        {
            SetState(!IsFavorite);

            if (IsFavorite)
            {
                _fileInfo.AddToFavorites();
            }
            else
            {
                _fileInfo.RemoveFromFavorites();
            }
        }

        public void SetState(bool isFavorite)
        {
            IsFavorite = isFavorite;
            _starImage.tintColor = IsFavorite ? Color.yellow : Color.gray;
        }

        public new class UxmlFactory : UxmlFactory<FavoritesButton, UxmlTraits>
        {
        }
    }
}