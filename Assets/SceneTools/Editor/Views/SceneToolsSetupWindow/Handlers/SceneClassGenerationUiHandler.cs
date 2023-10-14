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
        private readonly TextField _classLocationText;
        private readonly TextField _scriptableObjectLocationText;
        private readonly TextField _namespaceText;
        private readonly TextField _classNameText;
        private readonly TextField _scriptableObjectNameText;
        private readonly Button _classLocationButton;
        private readonly Button _scriptableObjectLocationButton;

        public SceneClassGenerationUiHandler(VisualElement root)
        {
            _mainToggle = root.Q<Toggle>("scene-class-generation-toggle");
            _autogenerateOnChangeToggle = root.Q<Toggle>("scene-class-changes-detection-toggle");
            _addressableScenesSupportToggle = root.Q<Toggle>("scene-class-addressables-support-toggle");
            _section = root.Q<VisualElement>("scene-class-generation-block");
            _classLocationText = root.Q<TextField>("scene-class-location-text");
            _scriptableObjectLocationText = root.Q<TextField>("scene-so-location-text");
            _namespaceText = root.Q<TextField>("scene-namespace-text");
            _classNameText = root.Q<TextField>("scene-class-name-text");
            _scriptableObjectNameText = root.Q<TextField>("scene-so-name-text");
            _classLocationButton = root.Q<Button>("scene-class-location-button");
            _scriptableObjectLocationButton = root.Q<Button>("scene-so-location-button");

            Init();
        }

        private void Init()
        {
            _classLocationText.SetEnabled(false);
            _scriptableObjectLocationText.SetEnabled(false);
            _mainToggle.SetValueWithoutNotify(SceneToolsService.SceneFilesGeneration.IsEnabled);
            _autogenerateOnChangeToggle.SetValueWithoutNotify(SceneToolsService.SceneFilesGeneration.IsAutoGenerateEnabled);
            _classLocationText.SetValueWithoutNotify(SceneToolsService.SceneFilesGeneration.ClassDirectory);
            _scriptableObjectLocationText.SetValueWithoutNotify(SceneToolsService.SceneFilesGeneration.ScriptableObjectDirectory);
            _namespaceText.SetValueWithoutNotify(SceneToolsService.SceneFilesGeneration.Namespace);
            _classNameText.SetValueWithoutNotify(SceneToolsService.SceneFilesGeneration.ClassName);
            _scriptableObjectNameText.SetValueWithoutNotify(SceneToolsService.SceneFilesGeneration.ScriptableObjectName);

            if (!Utils.IsAddressablesInstalled)
            {
                _addressableScenesSupportToggle.SetValueWithoutNotify(false);
                _addressableScenesSupportToggle.SetEnabled(false);
            }
            else
            {
                _addressableScenesSupportToggle.SetValueWithoutNotify(SceneToolsService.SceneFilesGeneration.IsAddressablesSupportEnabled);
            }
            
            SetSectionVisibility(_mainToggle.value);
        }

        public void SubscribeToEvents()
        {
            _classLocationButton.RegisterCallback<ClickEvent>(OnClassLocationClicked);
            _scriptableObjectLocationButton.RegisterCallback<ClickEvent>(OnScriptableObjectLocationClicked);
            _mainToggle.RegisterValueChangedCallback(OnMainToggleChanged);
        }

        public void UnsubscribeFromEvents()
        {
            _classLocationButton.UnregisterCallback<ClickEvent>(OnClassLocationClicked);
            _scriptableObjectLocationButton.UnregisterCallback<ClickEvent>(OnScriptableObjectLocationClicked);
            _mainToggle.UnregisterValueChangedCallback(OnMainToggleChanged);
        }

        public void Apply()
        {
            SceneToolsService.SceneFilesGeneration.IsEnabled = _mainToggle.value;

            if (!SceneToolsService.SceneFilesGeneration.IsEnabled)
            {
                DefineUtils.RemoveDefine(ScriptDefine);
                return;
            }

            DefineUtils.AddDefine(ScriptDefine);

            SceneToolsService.SceneFilesGeneration.IsAddressablesSupportEnabled = _addressableScenesSupportToggle.value;
            if (SceneToolsService.SceneFilesGeneration.IsAddressablesSupportEnabled)
            {
                DefineUtils.AddDefine(AddressablesSupportDefine);
            }
            else
            {
                DefineUtils.RemoveDefine(AddressablesSupportDefine);
            }

            SceneToolsService.SceneFilesGeneration.ClassDirectory = _classLocationText.text;
            SceneToolsService.SceneFilesGeneration.Namespace = _namespaceText.text;
            SceneToolsService.SceneFilesGeneration.ClassName = _classNameText.text;
            SceneToolsService.SceneFilesGeneration.IsAutoGenerateEnabled = _autogenerateOnChangeToggle.value;
            
            SceneToolsService.SceneFilesGeneration.ScriptableObjectDirectory = _scriptableObjectLocationText.text;
            SceneToolsService.SceneFilesGeneration.ScriptableObjectName = _scriptableObjectNameText.text;

            if (!AssetDatabase.IsValidFolder(_classLocationText.text))
            {
                Directory.CreateDirectory(_classLocationText.text);
            }
            
            if (!AssetDatabase.IsValidFolder(_scriptableObjectLocationText.text))
            {
                Directory.CreateDirectory(_scriptableObjectLocationText.text);
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

        private void OnClassLocationClicked(ClickEvent args)
        {
            var path = EditorUtility.OpenFolderPanel("Select Folder", "Assets/", string.Empty);
            _classLocationText.SetValueWithoutNotify(path.GetRelativePath());
        }
        
        private void OnScriptableObjectLocationClicked(ClickEvent args)
        {
            var path = EditorUtility.OpenFolderPanel("Select Folder", "Assets/", string.Empty);
            _scriptableObjectLocationText.SetValueWithoutNotify(path.GetRelativePath());
        }
    }
}