using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SandScene.Editor.Views
{
    public class LinkButton : Label
    {
        public event Action Clicked;

        public override string text
        {
            get => base.text;
            set => base.text = value.StartsWith("<u>") ? value : $"<u>{value}</u>"; 
        }

        public LinkButton() : base()
        {
            // Add a click event
            this.AddManipulator(new Clickable(OnClick));
        }

        public LinkButton(string text) : base(text)
        {
            // Add a click event
            this.AddManipulator(new Clickable(OnClick));
        }

        private void OnClick()
        {
            Clicked?.Invoke();
            Debug.Log(text);
        }
        
        public new class UxmlFactory : UxmlFactory<LinkButton, UxmlTraits> {}
    }
}