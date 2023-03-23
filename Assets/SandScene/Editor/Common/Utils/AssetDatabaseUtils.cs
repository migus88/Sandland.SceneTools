using System;
using System.IO;
using System.Linq;
using Sandland.SceneTool.Editor.Sandland.SceneTool.Editor.Common.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sandland.SceneTool.Editor.Common.Utils
{
    public static class AssetDatabaseUtils
    {
        public static VisualTreeAsset FindAndLoadVisualTreeAsset(string name = null) => FindAndLoadAsset<VisualTreeAsset>(name);
        
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
                    Debug.LogWarning($"Found more than one {typeName} file with the name: {name}; Loading only the first");
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
        
        public static AssetFileInfo[] FindScenes(string name = null) => FindAssets<Scene>(name);
        
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
                result[i] = new AssetFileInfo(assetName, path, guid);
            }

            return result;
        }
    }
}