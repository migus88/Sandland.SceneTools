#if SANDLAND_SCENE_CLASS_GEN
using System.Collections.Generic;
using System.IO;
using Sandland.SceneTool.Editor.Common.Data;
using Sandland.SceneTool.Editor.Common.Utils;
using Sandland.SceneTool.Editor.Services;
using UnityEditor;

namespace Sandland.SceneTool.Editor.Listeners
{
    internal static class SceneClassGenerationListener
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
        }

        [MenuItem(MenuItems.Tools.Actions + "Generate Scenes Class")]
        private static void OnSceneListChanged()
        {
            if (!SceneToolsService.ClassGeneration.IsAutoGenerateEnabled)
            {
                return;
            }
            
            var scenes = new List<SceneInfo>();

            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var sceneAsset = EditorBuildSettings.scenes[i];
                var name = Path.GetFileNameWithoutExtension(sceneAsset.path);
                scenes.Add(new SceneInfo(name, i));
            }
            
            SceneToolsService.ClassGeneration.CreateFile(scenes);
        }
    }
}

#endif