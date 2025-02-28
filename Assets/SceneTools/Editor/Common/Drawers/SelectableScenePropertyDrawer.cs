using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Listeners;
using Sandland.SceneTool.Runtime.Data;
using Sandland.SceneTool.Runtime.Enums;

namespace Sandland.SceneTool.Editor.Common.Drawers
{
    
[CustomPropertyDrawer(typeof(SelectableScene))]
public class SelectableScenePropertyDrawer : PropertyDrawer
{
    private static List<string> _options;
    private static List<SerializedScenesData.OptionValue> _optionValues;
    private static bool _isInitialized = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var sceneNameProp = property.FindPropertyRelative("_sceneName");
        
        if (sceneNameProp == null || sceneNameProp.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Missing _sceneName string.");
            return;
        }
        
        var sceneTypeProp = property.FindPropertyRelative("_sceneType");
        
        if (sceneTypeProp == null || sceneTypeProp.propertyType != SerializedPropertyType.Enum)
        {
            EditorGUI.LabelField(position, label.text, "Missing _sceneType string.");
            return;
        }
        
        InitializeOptions();

        var currentName = sceneNameProp.stringValue;
        var currentType = (SceneType)sceneTypeProp.enumValueIndex;
        var currentIndex = 0;
        
        if (!string.IsNullOrEmpty(currentName) && currentType != SceneType.None)
        {
            var found = _optionValues.FindIndex(x => x.Name == currentName && x.Type == currentType);
            if (found >= 0)
            {
                currentIndex = found;
            }
        }

        var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, _options.ToArray());
        
        if (selectedIndex != currentIndex)
        {
            sceneNameProp.stringValue = _optionValues[selectedIndex].Name;
            sceneTypeProp.enumValueIndex = (int)_optionValues[selectedIndex].Type;
        }
    }

    private static void InitializeOptions(bool shouldRetry = true)
    {
        if (_isInitialized)
        {
            return;
        }
        
        if (!EditorPrefs.HasKey("SelectableScenesData"))
        {
            Debug.LogWarning($"SelectableScene: No scenes data found in EditorPrefs.{(shouldRetry ? " Trying to generate it..." : " Also unable to generate it.")}");
            if (shouldRetry)
            {
                SceneClassGenerationListener.GenerateSceneEditorData();
                InitializeOptions(false);
            }
            return;
        }

        _options = new List<string>();
        _optionValues = new List<SerializedScenesData.OptionValue>();

        var json = EditorPrefs.GetString("SelectableScenesData");
        var scenesData = JsonUtility.FromJson<SerializedScenesData>(json);
        if (scenesData == null || scenesData.Options == null || scenesData.OptionValues == null)
        {
            Debug.LogWarning("SelectableScene: Invalid scenes data in EditorPrefs.");
            return;
        }

        _options = scenesData.Options;
        _optionValues = scenesData.OptionValues;
        _isInitialized = true;
    }

    // Called by the scene generator to reset the cached data.
    public static void Reset()
    {
        _isInitialized = false;
    }
}
}