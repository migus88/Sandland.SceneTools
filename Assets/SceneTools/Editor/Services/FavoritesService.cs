using System;
using System.Collections.Generic;
using System.Linq;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Utils;
using UnityEditor;

namespace Sandland.SceneTool.Editor.Services
{
    internal static class FavoritesService
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

        public static IOrderedEnumerable<SceneInfo> OrderByFavorites(this IEnumerable<SceneInfo> infos) =>
            infos.OrderByDescending(i => i.IsFavorite());
    }
}