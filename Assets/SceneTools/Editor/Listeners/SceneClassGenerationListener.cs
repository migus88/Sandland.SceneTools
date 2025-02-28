#if SANDLAND_SCENE_CLASS_GEN
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Services;
using UnityEditor;

#if SANDLAND_ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Sandland.SceneTool.Editor.Listeners
{
    internal static class SceneClassGenerationListener
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorBuildSettings.sceneListChanged += GenerateScenesFiles;

#if SANDLAND_ADDRESSABLES
            AddressableAssetSettings.OnModificationGlobal += OnAddressablesModification;
#endif
        }


#if SANDLAND_ADDRESSABLES
        private static void OnAddressablesModification(AddressableAssetSettings settings,
            AddressableAssetSettings.ModificationEvent args, object obj)
        {
            switch (obj)
            {
                case AddressableAssetEntry entry:
                {
                    var shouldGenerate = IsShouldGenerateClass(args, entry);
                    if (shouldGenerate)
                    {
                        GenerateScenesFiles();
                    }

                    break;
                }
                case IList<AddressableAssetEntry> entries:
                {
                    foreach (var e in entries)
                    {
                        var shouldGenerate = IsShouldGenerateClass(args, e);
                        if (shouldGenerate)
                        {
                            GenerateScenesFiles();
                        }
                    }

                    break;
                }
            }
        }

        private static bool IsShouldGenerateClass(AddressableAssetSettings.ModificationEvent evt,
            AddressableAssetEntry entry)
        {
            var assetPath = entry.AssetPath;
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            if (assetType != typeof(SceneAsset))
            {
                return false;
            }

            return evt is AddressableAssetSettings.ModificationEvent.BatchModification
                       or AddressableAssetSettings.ModificationEvent.EntryModified ||
                   EditorBuildSettings.scenes.All(s => s.path != assetPath);
        }
#endif

        [MenuItem(MenuItems.Tools.Actions + "Generate Scenes Files")]
        public static void GenerateScenesFiles()
        {
            if (!SceneToolsService.SceneFilesGeneration.IsAutoGenerateEnabled)
            {
                return;
            }

            var scenes = GetSceneInfos();

            SceneToolsService.SceneFilesGeneration.CreateClassFile(scenes);
            SceneToolsService.SceneFilesGeneration.CreateScriptableObjectFile(scenes);
        }

        public static void GenerateSceneEditorData()
        {
            var scenes = GetSceneInfos();
            SceneToolsService.SceneFilesGeneration.SaveScenesData(scenes);
        }

        private static List<SceneInfo> GetSceneInfos()
        {
            var scenes = new List<SceneInfo>();

            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var sceneAsset = EditorBuildSettings.scenes[i];
                var name = Path.GetFileNameWithoutExtension(sceneAsset.path);
                var info = SceneInfo.Create.BuiltIn(name, i, null, null);
                scenes.Add(info);
            }

#if SANDLAND_ADDRESSABLES

            var addressableScenes = AddressableAssetSettingsDefaultObject.Settings.groups.SelectMany(g => g.entries)
                .Where(e => EditorUtility.IsPersistent(e.MainAsset) && e.MainAsset is SceneAsset)
                .Select(e => SceneInfo.Create.Addressable(e.address, e.address))
                .ToList();

            scenes.AddRange(addressableScenes);
#endif

            return scenes;
        }
    }
}

#endif