using System;
using System.Collections.Generic;
using Sandland.SceneTool.Editor.Common.Utils;
using UnityEngine;

namespace Sandland.SceneTool.Editor.Sandland.SceneTool.Editor.Common.Data
{
    public class SceneSelectorConfig : ScriptableObject
    {
        public static SceneSelectorConfig Config => _config ? _config : _config = LoadConfig(); // Cannot use ??= here
        private static SceneSelectorConfig _config;

        [field: SerializeField] public List<string> Favorites { get; set; } = new List<string>();

        private static SceneSelectorConfig LoadConfig()
        {
            AssetDatabaseUtils.TryFindAndLoadAsset<SceneSelectorConfig>(out var config);
            return config;
        }
    }
}