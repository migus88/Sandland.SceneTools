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

        public static bool IsSceneInBuildSettings(string guid, out int buildIndex)
        {
            buildIndex = -1;
            var guidObj = new GUID(guid);

            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];

                if (scene.guid != guidObj)
                {
                    continue;
                }
                
                buildIndex = i;
                return true;
            }

            return false;
        }
    }
}