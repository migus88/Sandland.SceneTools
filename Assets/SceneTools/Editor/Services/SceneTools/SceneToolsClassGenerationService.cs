using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Utils;
using UnityEditor;

namespace Sandland.SceneTool.Editor.Services
{
    internal static partial class SceneToolsService
    {
        public static class ClassGeneration
        {
            private const string ToggleKey = "sandland-scene-class-generation-toggle";
            private const string LocationKey = "sandland-scene-class-generation-location";
            private const string ClassNameKey = "sandland-scene-class-generation-class-name";
            private const string NamespaceKey = "sandland-scene-class-generation-namespace";
            private const string DefaultLocation = "Assets/Sandland/Runtime/";
            private const string DefaultClassName = "SceneList";
            private const string DefaultNamespace = "Sandland";
            private const string Label = "sandland-scene-class";

            public static bool IsEnabled
            {
                get => EditorPrefs.GetBool(ToggleKey, false);
                set => EditorPrefs.SetBool(ToggleKey, value);
            }

            public static string Directory
            {
                get => GetDirectory();
                set => EditorPrefs.SetString(LocationKey, value);
            }

            public static string ClassName
            {
                get => EditorPrefs.GetString(ClassNameKey, DefaultClassName);
                set => EditorPrefs.SetString(ClassNameKey, value);
            }

            public static string Namespace
            {
                get => EditorPrefs.GetString(NamespaceKey, DefaultNamespace);
                set => EditorPrefs.SetString(NamespaceKey, value);
            }

            public static string FullPath => GetClassLocation();

            public static void CreateFile(IEnumerable<SceneInfo> scenes)
            {
                var builtInIndexes = scenes.Select(s =>
                    $"\t\t\t\tpublic const int {s.Name.ToPascalCase()} = {s.BuildIndex.ToString()};");
                var builtInNames = scenes.Select(s =>
                    $"\t\t\t\tpublic const string {s.Name.ToPascalCase()} = \"{s.Name}\";");

                var content = $"namespace {Namespace}\n" +
                              $"{{\n" +
                              $"\tpublic class {ClassName}\n" +
                              $"\t{{\n" +
                              $"\t\tpublic class BuildIn\n" +
                              $"\t\t{{\n" +
                              $"\t\t\tpublic class Names\n" +
                              $"\t\t\t{{\n" +
                              string.Join("\n", builtInNames) +
                              $"\n\t\t\t}}\n" +
                              $"\n\t\t\tpublic class Indexes\n" +
                              $"\t\t\t{{\n" +
                              string.Join("\n", builtInIndexes) +
                              $"\n\t\t\t}}\n" +
                              $"\t\t}}\n\n" +
                              "\t\tpublic class Addressables\n" +
                              $"\t\t{{\n" +
                              "\n" +
                              $"\t\t}}\n" +
                              $"\t}}\n" +
                              $"}}\n";

                var finalPath = FullPath ?? Path.Combine(Directory, $"{ClassName}.cs");

                if (!AssetDatabase.IsValidFolder(Directory))
                {
                    System.IO.Directory.CreateDirectory(Directory);
                }

                File.WriteAllText(finalPath, content);
                AssetDatabase.Refresh();

                AssetDatabaseUtils.SetLabel<MonoScript>(finalPath, Label);
            }

            private static string GetDirectory()
            {
                var classLocation = GetClassLocation();

                return classLocation == null
                    ? EditorPrefs.GetString(LocationKey, DefaultLocation)
                    : Path.GetDirectoryName(classLocation);
            }

            private static string GetClassLocation()
            {
                var info = AssetDatabase.FindAssets($"l:{Label}");
                return info.Length == 0 ? null : AssetDatabase.GUIDToAssetPath(info[0]);
            }
        }
    }
}