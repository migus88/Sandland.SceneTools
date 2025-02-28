using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Drawers;
using Sandland.SceneTool.Editor.Common.Utils;
using SceneTools.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace Sandland.SceneTool.Editor.Services
{
    internal static partial class SceneToolsService
    {
        public static class SceneFilesGeneration
        {
            private const string MainToggleKey = "sandland-scene-class-generation-toggle";
            private const string ClassLocationKey = "sandland-scene-class-generation-location";
            private const string ScriptableObjectLocationKey = "sandland-scene-class-generation-location";
            private const string ClassNameKey = "sandland-scene-class-generation-class-name";
            private const string ScriptableObjectNameKey = "sandland-scene-so-generation-class-name";
            private const string NamespaceKey = "sandland-scene-class-generation-namespace";
            private const string AddressablesSupportKey = "sandland-scene-class-generation-addressables-support";
            private const string AutogenerateOnChangeToggleKey = "sandland-scene-class-generation-autogenerate-toggle";
            
            private const string DefaultLocation = "Assets/Sandland/Runtime/";
            private const string DefaultClassName = "SceneList";
            private const string DefaultScriptableObjectName = nameof(ProjectScenes);
            private const string DefaultNamespace = "Sandland";
            private const string ClassLabel = "sandland-scene-class";
            private const string ScriptableObjectLabel = "sandland-scene-so";

            public static bool IsEnabled
            {
                get => EditorPrefs.GetBool(MainToggleKey, false);
                set => EditorPrefs.SetBool(MainToggleKey, value);
            }
            
            public static bool IsAutoGenerateEnabled
            {
                get => EditorPrefs.GetBool(AutogenerateOnChangeToggleKey, true);
                set => EditorPrefs.SetBool(AutogenerateOnChangeToggleKey, value);
            }
            
            public static bool IsAddressablesSupportEnabled
            {
                get => EditorPrefs.GetBool(AddressablesSupportKey, false);
                set => EditorPrefs.SetBool(AddressablesSupportKey, value);
            }

            public static string ClassDirectory
            {
                get => GetDirectory(ClassLabel, ClassLocationKey);
                set => EditorPrefs.SetString(ClassLocationKey, value);
            }

            public static string ScriptableObjectDirectory
            {
                get => GetDirectory(ScriptableObjectLabel, ScriptableObjectLocationKey);
                set => EditorPrefs.SetString(ScriptableObjectLocationKey, value);
            }

            public static string ClassName
            {
                get => EditorPrefs.GetString(ClassNameKey, DefaultClassName);
                set => EditorPrefs.SetString(ClassNameKey, value);
            }

            public static string ScriptableObjectName
            {
                get => EditorPrefs.GetString(ScriptableObjectNameKey, DefaultScriptableObjectName);
                set => EditorPrefs.SetString(ScriptableObjectNameKey, value);
            }

            public static string Namespace
            {
                get => EditorPrefs.GetString(NamespaceKey, DefaultNamespace);
                set => EditorPrefs.SetString(NamespaceKey, value);
            }

            public static string ClassFullPath => GetLocation(ClassLabel);
            public static string ScriptableObjectFullPath => GetLocation(ScriptableObjectLabel);
            
            public static void CreateScriptableObjectFile(List<SceneInfo> scenes)
            {
                var builtInScenes = scenes.Where(s => string.IsNullOrEmpty(s.Address));
                var addressablesScenes = scenes.Where(s => !string.IsNullOrWhiteSpace(s.Address));
                
                var projectScenes = ScriptableObject.CreateInstance<ProjectScenes>();

                projectScenes.Names = builtInScenes.Select(s => s.Name).ToArray();
                projectScenes.Indexes = builtInScenes.Select(s => s.BuildIndex).ToArray();
                projectScenes.Addressables = addressablesScenes.Select(s => s.Address).ToArray();

                var finalPath = ScriptableObjectFullPath ?? Path.Combine(ScriptableObjectDirectory, $"{ScriptableObjectName}.asset");

                if (!AssetDatabase.IsValidFolder(ScriptableObjectDirectory))
                {
                    Directory.CreateDirectory(ScriptableObjectDirectory);
                }
                
                AssetDatabase.CreateAsset(projectScenes, finalPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            public static void CreateClassFile(List<SceneInfo> scenes)
            {
                var builtInScenes = scenes.Where(s => string.IsNullOrEmpty(s.Address));
                var addressablesScene = scenes.Where(s => !string.IsNullOrWhiteSpace(s.Address));
                
                var builtInIndexes = builtInScenes
                    .Select(s => $"\t\t\tpublic const int {s.Name.ToPascalCase()} = {s.BuildIndex.ToString()};");
                var builtInNames = builtInScenes
                    .Select(s => $"\t\t\tpublic const string {s.Name.ToPascalCase()} = \"{s.Name}\";");
                var addresses = addressablesScene
                    .Select(s => $"\t\t\tpublic const string {GetAddressablesConstName(s.Address)} = \"{s.Address}\";");

                var content = $"namespace {Namespace}\n" +
                              $"{{\n" +
                              $"// This class is autogenerated. Any manual changes will be overriden during next generation" +
                              $"\tpublic static class {ClassName}\n" +
                              $"\t\tpublic static class Names\n" +
                              $"\t\t{{\n" +
                              string.Join("\n", builtInNames) +
                              $"\n\t\t}}\n" +
                              $"\n\t\tpublic static class Indexes\n" +
                              $"\t\t{{\n" +
                              string.Join("\n", builtInIndexes) +
                              $"\n\t\t}}\n\n" +
                              "\t\tpublic static class Addressables\n" +
                              $"\t\t{{\n" +
                              string.Join("\n", addresses) +
                              $"\n\t\t}}\n" +
                              $"\t}}\n" +
                              $"}}\n";

                var finalPath = ClassFullPath ?? Path.Combine(ClassDirectory, $"{ClassName}.cs");

                if (!AssetDatabase.IsValidFolder(ClassDirectory))
                {
                    System.IO.Directory.CreateDirectory(ClassDirectory);
                }

                File.WriteAllText(finalPath, content);
                AssetDatabase.Refresh();

                AssetDatabaseUtils.SetLabel<MonoScript>(finalPath, ClassLabel);
                
                // Save scenes data to EditorPrefs
                SaveScenesData(scenes);
            }
            
            public static void SaveScenesData(List<SceneInfo> scenes)
            {
                var builtInScenes = scenes.Where(s => string.IsNullOrEmpty(s.Address));
                var addressableScenes = scenes.Where(s => !string.IsNullOrWhiteSpace(s.Address));

                var options = new List<string>();
                var optionValues = new List<string>();

                foreach (var scene in builtInScenes)
                {
                    options.Add("Build: " + scene.Name);
                    optionValues.Add("build--" + scene.Name);
                }
                foreach (var scene in addressableScenes)
                {
                    options.Add("Addressable: " + scene.Address);
                    optionValues.Add("addressable--" + scene.Address);
                }

                var scenesData = new SerializedScenesData { Options = options, OptionValues = optionValues };
                var json = JsonUtility.ToJson(scenesData);
                EditorPrefs.SetString("SelectableScenesData", json);

                // Reset the property drawer's static flag so it will reload the new data.
                SelectableScenePropertyDrawer.Reset();
            }

            private static string GetAddressablesConstName(string address) =>
                Path.GetFileNameWithoutExtension(address).ToPascalCase();

            private static string GetDirectory(string label, string locationKey)
            {
                var classLocation = GetLocation(label);

                return classLocation == null
                    ? EditorPrefs.GetString(locationKey, DefaultLocation)
                    : Path.GetDirectoryName(classLocation);
            }

            private static string GetLocation(string label)
            {
                var info = AssetDatabase.FindAssets($"l:{label}");
                return info.Length == 0 ? null : AssetDatabase.GUIDToAssetPath(info[0]);
            }
        }
    }
}