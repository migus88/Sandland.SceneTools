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
        private const string FavoriteClassName = "favorite";
        
        public bool IsFavorite { get; private set; }

        //private Image _starImage;
        private AssetFileInfo _fileInfo;

        public FavoritesButton()
        {
            this.AddManipulator(new Clickable(OnClick));
        }

        public void Init(AssetFileInfo info)
        {
            _fileInfo = info;
            var isFavorite = _fileInfo.IsFavorite();
            SetState(isFavorite);
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
            
            if (IsFavorite)
            {
                AddToClassList(FavoriteClassName);
            }
            else
            {
                RemoveFromClassList(FavoriteClassName);
            }
        }

        public new class UxmlFactory : UxmlFactory<FavoritesButton, UxmlTraits>
        {
        }
    }
}