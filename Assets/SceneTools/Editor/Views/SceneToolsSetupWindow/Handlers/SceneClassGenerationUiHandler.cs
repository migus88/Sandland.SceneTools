using System.IO;
using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Services;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sandland.SceneTool.Editor.Views.Handlers
{
    internal class SceneClassGenerationUiHandler : ISceneToolsSetupUiHandler
    {
        private const string HiddenContentClass = "hidden";
        private const string ScriptDefine = "SANDLAND_SCENE_CLASS_GEN";
        private const string AddressablesSupportDefine = "SANDLAND_ADDRESSABLES";

        private readonly Toggle _mainToggle;
        private readonly Toggle _autogenerateOnChangeToggle;
        private readonly Toggle _addressableScenesSupportToggle;
        private readonly VisualElement _section;
        private readonly TextField _locationText;
        private readonly TextField _namespaceText;
        private readonly TextField _classNameText;
        private readonly Button _locationButton;

        public SceneClassGenerationUiHandler(VisualElement root)
        {
            _mainToggle = root.Q<Toggle>("scene-class-generation-toggle");
            _autogenerateOnChangeToggle = root.Q<Toggle>("scene-class-changes-detection-toggle");
            _addressableScenesSupportToggle = root.Q<Toggle>("scene-class-addressables-support-toggle");
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
            _mainToggle.SetValueWithoutNotify(SceneToolsService.ClassGeneration.IsEnabled);
            _autogenerateOnChangeToggle.SetValueWithoutNotify(SceneToolsService.ClassGeneration.IsAutoGenerateEnabled);
            _locationText.SetValueWithoutNotify(SceneToolsService.ClassGeneration.Directory);
            _namespaceText.SetValueWithoutNotify(SceneToolsService.ClassGeneration.Namespace);
            _classNameText.SetValueWithoutNotify(SceneToolsService.ClassGeneration.ClassName);

            if (!Utils.IsAddressablesInstalled)
            {
                _addressableScenesSupportToggle.SetValueWithoutNotify(false);
                _addressableScenesSupportToggle.SetEnabled(false);
            }
            else
            {
                _addressableScenesSupportToggle.SetValueWithoutNotify(SceneToolsService.ClassGeneration.IsAddressablesSupportEnabled);
            }
            
            SetSectionVisibility(_mainToggle.value);
        }

        public void SubscribeToEvents()
        {
            _locationButton.RegisterCallback<ClickEvent>(OnLocationClicked);
            _mainToggle.RegisterValueChangedCallback(OnMainToggleChanged);
        }

        public void UnsubscribeFromEvents()
        {
            _locationButton.UnregisterCallback<ClickEvent>(OnLocationClicked);
            _mainToggle.UnregisterValueChangedCallback(OnMainToggleChanged);
        }

        public void Apply()
        {
            SceneToolsService.ClassGeneration.IsEnabled = _mainToggle.value;

            if (!SceneToolsService.ClassGeneration.IsEnabled)
            {
                DefineUtils.RemoveDefine(ScriptDefine);
                return;
            }

            DefineUtils.AddDefine(ScriptDefine);

            SceneToolsService.ClassGeneration.IsAddressablesSupportEnabled = _addressableScenesSupportToggle.value;
            if (SceneToolsService.ClassGeneration.IsAddressablesSupportEnabled)
            {
                DefineUtils.AddDefine(AddressablesSupportDefine);
            }
            else
            {
                DefineUtils.RemoveDefine(AddressablesSupportDefine);
            }

            SceneToolsService.ClassGeneration.Directory = _locationText.text;
            SceneToolsService.ClassGeneration.Namespace = _namespaceText.text;
            SceneToolsService.ClassGeneration.ClassName = _classNameText.text;
            SceneToolsService.ClassGeneration.IsAutoGenerateEnabled = _autogenerateOnChangeToggle.value;

            if (!AssetDatabase.IsValidFolder(_locationText.text))
            {
                Directory.CreateDirectory(_locationText.text);
            }
        }

        private void OnMainToggleChanged(ChangeEvent<bool> args)
        {
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