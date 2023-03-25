using System;
using Sandland.SceneTool.Editor.Common.Data;
using UnityEditor;
using UnityEngine;

namespace Sandland.SceneTool.Editor.Common.Utils
{
    internal static class Icons
    {
        public static Texture2D SceneToolIcon => _sceneToolIcon ??=
            AssetDatabaseUtils.FindAndLoadAsset<Texture2D>("tool_icon l:Sandland-scene-tool-icon"); 
        public static Texture2D DefaultSceneIcon => _defaultSceneIcon ??=
            AssetDatabaseUtils.FindAndLoadAsset<Texture2D>("unused_scene_icon l:Sandland-scene-tool-icon"); 
        public static Texture2D AddressableSceneIcon => _addressableSceneIcon ??=
            AssetDatabaseUtils.FindAndLoadAsset<Texture2D>("addressable_scene_icon l:Sandland-scene-tool-icon"); 
        public static Texture2D BuiltInSceneIcon => _builtInSceneIcon ??=
            AssetDatabaseUtils.FindAndLoadAsset<Texture2D>("built_in_scene_icon l:Sandland-scene-tool-icon"); 
        
        
        private static Texture2D _sceneToolIcon;
        private static Texture2D _defaultSceneIcon;
        private static Texture2D _addressableSceneIcon;
        private static Texture2D _builtInSceneIcon;

        public static Texture2D GetSceneIcon(this SceneInfo info) => info.ImportType switch
        {
            SceneImportType.None => DefaultSceneIcon,
            SceneImportType.BuildSettings => BuiltInSceneIcon,
            SceneImportType.Addressables => AddressableSceneIcon,
            _ => throw new ArgumentOutOfRangeException()
        };

    }
}