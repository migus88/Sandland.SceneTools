using System;
using System.IO;
using System.Linq;
using Sandland.SceneTool.Editor.Common.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sandland.SceneTool.Editor.Common.Utils
{
    internal static class AssetDatabaseUtils
    {
        public static VisualTreeAsset FindAndLoadVisualTreeAsset(string name = null) =>
            FindAndLoadAsset<VisualTreeAsset>(name);

        public static StyleSheet FindAndLoadStyleSheet(string name = null) => FindAndLoadAsset<StyleSheet>(name);

        public static bool TryFindAndLoadAsset<T>(out T result, string name = null) where T : Object
        {
            try
            {
                result = FindAndLoadAsset<T>(name);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static T FindAndLoadAsset<T>(string name = null) where T : Object
        {
            // TODO: Reuse code from FindAssets
            var typeName = typeof(T).Name;
            var query = string.IsNullOrEmpty(name) ? $"t:{typeName}" : $"{name} t:{typeName}";
            var guids = AssetDatabase.FindAssets(query);

            switch (guids.Length)
            {
                case 0:
                    throw new FileNotFoundException($"Cant locate {typeName} file with the name: {name}");
                case > 1:
                    Debug.LogWarning(
                        $"Found more than one {typeName} file with the name: {name}; Loading only the first");
                    break;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids.First());

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset == null)
            {
                throw new FileNotFoundException($"Unable to load the {typeName} with the name {name}");
            }

            return asset;
        }

        public static bool TryFindAssets<T>(out AssetFileInfo[] result, string name = null)
        {
            try
            {
                result = FindAssets<T>(name);
                return result.Length > 0;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static SceneInfo[] FindScenes(string name = null)
        {
            var assets = FindAssets<Scene>(name);

            var result = new SceneInfo[assets.Length];

            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                
                if (Utils.IsAssetAddressable(asset.Guid, out var address))
                {
                    result[i] = SceneInfo.Create.Addressable(address, asset.Name, asset.Path, asset.Guid, asset.Labels);
                }
                else if (Utils.IsAssetInBundle(asset.Path))
                {
                    result[i] = SceneInfo.Create.AssetBundle(asset.Name, asset.Path, asset.Guid, asset.Labels);
                }
                else if (Utils.IsSceneInBuildSettings(asset.Guid, out var buildIndex))
                {
                    result[i] = SceneInfo.Create.BuiltIn(asset.Name, buildIndex, asset.Path, asset.Guid, asset.Labels);
                }
                else
                {
                    result[i] = SceneInfo.Create.Default(asset.Name, asset.Path, asset.Guid, asset.Labels);
                }
            }

            return result;
        }

        public static AssetFileInfo[] FindAssets<T>(string name = null)
        {
            var typeName = typeof(T).Name;
            var query = string.IsNullOrEmpty(name) ? $"t:{typeName}" : $"{name} t:{typeName}";
            var guids = AssetDatabase.FindAssets(query);

            if (guids.Length == 0)
            {
                return Array.Empty<AssetFileInfo>();
            }

            var result = new AssetFileInfo[guids.Length];

            for (var i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var assetName = Path.GetFileNameWithoutExtension(path);
                var labels = AssetDatabase.GetLabels(new GUID(guid)).ToList();
                result[i] = new AssetFileInfo(assetName, path, guid, labels);
            }

            return result;
        }

        public static void SetLabels<T>(this AssetFileInfo info) where T : Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(info.Path);
            AssetDatabase.SetLabels(asset, info.Labels.ToArray());
        }

        public static void SetLabel<T>(string path, string label) where T : Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            var labels = AssetDatabase.GetLabels(asset).ToList();

            if (labels.Contains(label))
            {
                return;
            }

            labels.Add(label);
            AssetDatabase.SetLabels(asset, labels.ToArray());
        }
    }
}