using System;
using System.Collections.Generic;
using System.Linq;
using SandScene.Editor.SandScene.Editor.Common.Data;
using UnityEditor;
using static SandScene.Editor.Common.Utils.SceneSelectorService;
using static SandScene.Editor.SandScene.Editor.Common.Data.SceneSelectorConfig;

namespace SandScene.Editor.Common.Utils
{
    public static class FavoritesService
    {
        public static event Action FavoritesChanged;
        
        public static bool IsInFavorites(string guid) => HasConfig && Config.Favorites.Contains(guid);
            
        public static void AddToFavorites(string guid)
        {
            if (IsInFavorites(guid))
            {
                return;
            }

            Config.Favorites.Add(guid);
            EditorUtility.SetDirty(Config);
            AssetDatabase.SaveAssetIfDirty(Config);
            FavoritesChanged?.Invoke();
        }

        public static void RemoveFromFavorites(string guid)
        {
            if (!IsInFavorites(guid))
            {
                return;
            }

            Config.Favorites.Remove(guid);
            EditorUtility.SetDirty(Config);
            AssetDatabase.SaveAssetIfDirty(Config);
            FavoritesChanged?.Invoke();
        }

        public static IEnumerable<AssetFileInfo> OrderByFavorites(this IEnumerable<AssetFileInfo> infos) =>
            infos.OrderByDescending(i => IsInFavorites(i.Guid));
    }
}