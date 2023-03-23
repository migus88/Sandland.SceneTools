using System;
using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Sandland.SceneTool.Editor.Common.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sandland.SceneTool.Editor.Views
{
    public class FavoritesButton : VisualElement
    {
        private static Texture2D Icon => _icon ??= EditorGUIUtility.IconContent("Favorite On Icon").image as Texture2D;
        private static Texture2D _icon;
        
        private Image _starImage;
        private AssetFileInfo _fileInfo;
    
        public bool IsFavorite { get; private set; }
        
        public void Init(AssetFileInfo info)
        {
            _fileInfo = info;
            
            _starImage = new Image
            {
                image = Icon
            };
            Add(_starImage);
    
            this.AddManipulator(new Clickable(OnClick));
            
            var isFavorite = FavoritesService.IsInFavorites(_fileInfo.Guid);
            SetState(isFavorite);
        }
    
        private void OnClick() 
        {
            if (!FavoritesService.CanChangeFavorites)
            {
                return;
            }
            
            SetState(!IsFavorite);

            if (IsFavorite)
            {
                FavoritesService.AddToFavorites(_fileInfo.Guid);
            }
            else
            {
                FavoritesService.RemoveFromFavorites(_fileInfo.Guid);
            }
        }
    
        public void SetState(bool isFavorite)
        {
            IsFavorite = isFavorite;
            _starImage.tintColor = IsFavorite ? Color.yellow : Color.gray;
        }
        
        public new class UxmlFactory : UxmlFactory<FavoritesButton, UxmlTraits> {}
    }
}