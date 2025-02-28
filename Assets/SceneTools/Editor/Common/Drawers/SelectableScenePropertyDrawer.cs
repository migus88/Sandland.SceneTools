using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Listeners;
using Sandland.SceneTool.Runtime.Data;

namespace Sandland.SceneTool.Editor.Common.Drawers
{
    
[CustomPropertyDrawer(typeof(SelectableScene))]
public class SelectableScenePropertyDrawer : PropertyDrawer
{
    private static List<string> _options;
    private static List<string> _optionValues;
    private static bool _isInitialized = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Find the wrapped sceneValue string inside the SelectableScene class.
        var sceneValueProp = property.FindPropertyRelative("_sceneValue");
        if (sceneValueProp == null || sceneValueProp.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Missing _sceneValue string.");
            return;
        }

        if (!_isInitialized)
        {
            InitializeOptions();
            _isInitialized = true;
        }

        var currentValue = sceneValueProp.stringValue;
        var currentIndex = 0;
        if (!string.IsNullOrEmpty(currentValue))
        {
            var found = _optionValues.IndexOf(currentValue);
            if (found >= 0)
                currentIndex = found;
        }

        var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, _options.ToArray());
        if (selectedIndex != currentIndex)
            sceneValueProp.stringValue = _optionValues[selectedIndex];
    }

    private static void InitializeOptions(bool shouldRetry = true)
    {
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
        _optionValues = new List<string>();

        var json = EditorPrefs.GetString("SelectableScenesData");
        var scenesData = JsonUtility.FromJson<SerializedScenesData>(json);
        if (scenesData == null || scenesData.Options == null || scenesData.OptionValues == null)
        {
            Debug.LogWarning("SelectableScene: Invalid scenes data in EditorPrefs.");
            return;
        }

        _options = scenesData.Options;
        _optionValues = scenesData.OptionValues;
    }

    // Called by the scene generator to reset the cached data.
    public static void Reset()
    {
        _isInitialized = false;
    }
}
}