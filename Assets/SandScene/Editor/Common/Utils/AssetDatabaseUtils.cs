using System;
using System.IO;
using System.Linq;
using SandScene.Editor.SandScene.Editor.Common.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SandScene.Editor.Common.Utils
{
    public static class AssetDatabaseUtils
    {
        public static VisualTreeAsset FindAndLoadVisualTreeAsset(string name) => FindAndLoadAsset<VisualTreeAsset>(name);
        public static StyleSheet FindAndLoadStyleSheet(string name) => FindAndLoadAsset<StyleSheet>(name);
        
        public static T FindAndLoadAsset<T>(string name) where T : Object
        {
            var type = typeof(T).Name;
            var guids = AssetDatabase.FindAssets($"{name} t:{type}");

            switch (guids.Length)
            {
                case 0:
                    throw new FileNotFoundException($"Cant locate {type} file with the name: {name}");
                case > 1:
                    Debug.LogWarning($"Found more than one {type} file with the name: {name}; Loading only the first");
                    break;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids.First());
            
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset == null)
            {
                throw new FileNotFoundException($"Unable to load the {type} with the name {name}");
            }

            return asset;
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