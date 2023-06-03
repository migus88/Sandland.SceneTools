using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

#if SANDLAND_ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Sandland.SceneTool.Editor.Common.Utils
{
    public static class Utils
    {
        public static bool IsAddressablesInstalled =>
            PackageInfo.FindForAssetPath("Packages/com.unity.addressables/AddressableAssets")?.name ==
            "com.unity.addressables";

        public static bool IsAssetAddressable(string guid, out string address)
        {
            address = string.Empty;

#if SANDLAND_ADDRESSABLES
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                Debug.LogError("Addressable Asset Settings not found.");
                return false;
            }

            if (!string.IsNullOrEmpty(guid))
            {
                var entry = settings.FindAssetEntry(guid);
                address = entry != null ? entry.address : address;
                return entry != null;
            }

            Debug.LogError($"No valid asset path found: {guid}");
            return false;
#else
            return false;
#endif
        }

        public static bool IsAssetInBundle(Dictionary<string, string> assetsInBundles, string assetPath, out string bundleName) 
            => assetsInBundles.TryGetValue(assetPath, out bundleName);

        public static Dictionary<string, string> GetAssetsInBundles() =>
            AssetDatabase
                .GetAllAssetBundleNames()
                .SelectMany(AssetDatabase.GetAssetPathsFromAssetBundle, (bundleName, path) => new { bundleName, path })
                .ToDictionary(x => x.path, x => x.bundleName);

        public static Dictionary<GUID, int> GetSceneBuildIndexes()
        {
            var collection = new Dictionary<GUID, int>();
            var scenesAmount = EditorBuildSettings.scenes.Length;

            for (var i = 0; i < scenesAmount; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                collection.Add(scene.guid, i);
            }

            return collection;
        }

        public static bool ContainsSceneGuid(this Dictionary<GUID, int> sceneBuildIndexes, string sceneGuid,
            out int buildIndex)
        {
            buildIndex = -1;
            var sceneGuidObj = new GUID(sceneGuid);

            var hasScene = sceneBuildIndexes.TryGetValue(sceneGuidObj, out var sceneIndex);

            if (hasScene)
            {
                buildIndex = sceneIndex;
            }

            return hasScene;
        }
    }
}