using System;
using System.Collections.Generic;
using System.Linq;
using Sandland.SceneTool.Editor.Common.Data;
using UnityEditor;

namespace Sandland.SceneTool.Editor.Common.Utils
{
    public static class FavoritesService
    {
        private const string FavoriteSceneLabel = "sandland-favorite-scene";

        public static event Action FavoritesChanged;

        public static bool IsFavorite(this AssetFileInfo info) => info.Labels?.Contains(FavoriteSceneLabel) ?? false;

        public static void AddToFavorites(this AssetFileInfo info)
        {
            if (info.IsFavorite())
            {
                return;
            }

            info.Labels ??= new List<string>();
            info.Labels.Add(FavoriteSceneLabel);
            info.SetLabels<SceneAsset>();
            FavoritesChanged?.Invoke();
        }

        public static void RemoveFromFavorites(this AssetFileInfo info)
        {
            if (!info.IsFavorite())
            {
                return;
            }

            info.Labels.Remove(FavoriteSceneLabel);
            info.SetLabels<SceneAsset>();
            FavoritesChanged?.Invoke();
        }

        public static IEnumerable<AssetFileInfo> OrderByFavorites(this IEnumerable<AssetFileInfo> infos) =>
            infos.OrderByDescending(i => i.IsFavorite());
    }
}