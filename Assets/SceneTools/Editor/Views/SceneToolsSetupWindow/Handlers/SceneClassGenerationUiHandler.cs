using System;
using System.IO;
using Sandland.SceneTool.Editor.Common.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Views.Handlers
{
    internal class SceneClassGenerationUiHandler : ISceneToolsSetupUiHandler
    {
        private const string HiddenContentClass = "hidden";
        private const string ScriptDefine = "SANDLAND_SCENE_CLASS_GEN";

        private readonly Toggle _toggle;
        private readonly VisualElement _section;
        private readonly TextField _locationText;
        private readonly TextField _namespaceText;
        private readonly TextField _classNameText;
        private readonly Button _locationButton;

        public SceneClassGenerationUiHandler(VisualElement root)
        {
            _toggle = root.Q<Toggle>("scene-class-generation-toggle");
            _section = root.Q<VisualElement>("scene-class-generation-block");
            _locationText = root.Q<TextField>("scene-class-location-text");
            _namespaceText = root.Q<TextField>("scene-namespace-text");
            _classNameText = root.Q<TextField>("scene-class-name-text");
            _locationButton = root.Q<Button>("scene-class-location-button");

            Init();
        }

        private void Init()
        {
            _locationText.SetEnabled(false);
            _toggle.SetValueWithoutNotify(SceneToolsService.ClassGeneration.IsEnabled);
            _locationText.SetValueWithoutNotify(SceneToolsService.ClassGeneration.Directory);
            SetSectionVisibility(_toggle.value);
        }

        public void SubscribeToEvents()
        {
            _locationButton.RegisterCallback<ClickEvent>(OnLocationClicked);
            _toggle.RegisterValueChangedCallback(OnToggleChanged);
        }

        public void UnsubscribeFromEvents()
        {
            _locationButton.UnregisterCallback<ClickEvent>(OnLocationClicked);
            _toggle.UnregisterValueChangedCallback(OnToggleChanged);
        }

        public void Apply()
        {
            if (!SceneToolsService.ClassGeneration.IsEnabled)
            {
                DefineUtils.RemoveDefine(ScriptDefine);
                return;
            }
            
            DefineUtils.AddDefine(ScriptDefine);

            SceneToolsService.ClassGeneration.Directory = _locationText.text;
            SceneToolsService.ClassGeneration.Namespace = _namespaceText.text;
            SceneToolsService.ClassGeneration.ClassName = _classNameText.text;
            
            if (!AssetDatabase.IsValidFolder(_locationText.text))
            {
                Directory.CreateDirectory(_locationText.text);
            }
        }

        private void OnToggleChanged(ChangeEvent<bool> args)
        {
            SceneToolsService.ClassGeneration.IsEnabled = args.newValue;
            SetSectionVisibility(args.newValue);
        }

        private void SetSectionVisibility(bool isVisible)
        {
            if (isVisible)
            {
                _section.RemoveFromClassList(HiddenContentClass);
            }
            else
            {
                _section.AddToClassList(HiddenContentClass);
            }
        }

        private void OnLocationClicked(ClickEvent args)
        {
            var path = EditorUtility.OpenFolderPanel("Select Folder", "Assets/", string.Empty);
            _locationText.SetValueWithoutNotify(path.GetRelativePath());
        }
    }
}