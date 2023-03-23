using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SandScene.Editor.Views
{
    public class FavoritesButton : VisualElement
    {
        private static Texture2D Icon => _icon ??= EditorGUIUtility.IconContent("Favorite On Icon").image as Texture2D;
        private static Texture2D _icon;
        
        private Image _starImage;
    
        public event Action<FavoritesButton, bool> Clicked;
    
        public bool IsFavorite { get; private set; }
    
        public FavoritesButton()
        {
            Init();
        }
    
        private void Init()
        {
            _starImage = new Image
            {
                image = Icon
            };
            Add(_starImage);
    
            this.AddManipulator(new Clickable(OnClick));
            SetFavorite(false);
        }
    
        private void OnClick() 
        {
            SetFavorite(!IsFavorite);
            Clicked?.Invoke(this, IsFavorite);
        }
    
        public void SetFavorite(bool isFavorite)
        {
            IsFavorite = isFavorite;
            
            _starImage.tintColor = IsFavorite ? Color.yellow : Color.gray;
        }
        
        public new class UxmlFactory : UxmlFactory<FavoritesButton, UxmlTraits> {}
    }
}