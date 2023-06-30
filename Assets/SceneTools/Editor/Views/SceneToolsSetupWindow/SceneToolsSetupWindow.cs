using System.Collections.Generic;
using System.Threading.Tasks;
using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Views.Base;
using Sandland.SceneTool.Editor.Views.Handlers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Views
{
    internal class SceneToolsSetupWindow : SceneToolsWindowBase
    {
        private const string WindowMenuItem = MenuItems.Tools.Root + "Setup Scene Tools";

        public override float MinWidth => 600;
        public override float MinHeight => 600;
        public override string WindowName => "Scene Tools Setup";
        public override string VisualTreeName => nameof(SceneToolsSetupWindow);
        public override string StyleSheetName => nameof(SceneToolsSetupWindow);

        private readonly List<ISceneToolsSetupUiHandler> _uiHandlers = new();

        private Button _saveAllButton;

        [MenuItem(WindowMenuItem, priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneToolsSetupWindow>();
            window.InitWindow();
            window.minSize = new Vector2(window.MinWidth, window.MinHeight);
        }

        protected override void InitGui()
        {
            _uiHandlers.Add(new SceneClassGenerationUiHandler(rootVisualElement));
            _uiHandlers.Add(new ThemesSelectionUiHandler(rootVisualElement));

            _saveAllButton = rootVisualElement.Q<Button>("save-button");

            _saveAllButton.clicked += OnSaveAllButtonClicked;
            _uiHandlers.ForEach(handler => handler.SubscribeToEvents());
        }

        private void OnDestroy()
        {
            _saveAllButton.clicked -= OnSaveAllButtonClicked;
            _uiHandlers.ForEach(handler => handler.UnsubscribeFromEvents());
        }

        private async void OnSaveAllButtonClicked()
        {
            rootVisualElement.SetEnabled(false);
            _uiHandlers.ForEach(handler => handler.Apply());

            while (EditorApplication.isCompiling)
            {
                await Task.Delay(100); // Checking every 100ms
            }

            rootVisualElement.SetEnabled(true);
        }
    }
}