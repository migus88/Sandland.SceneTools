using System.IO;
using SandScene.Editor.SandScene.Editor.Common.Data;
using UnityEditor;
using UnityEngine;

namespace SandScene.Editor.Common.Utils
{
    public static class SceneSelectorService
    {
        private const string DefaultConfigDirectory = "Assets/Editor/";
        private const string DefaultConfigPath = DefaultConfigDirectory + "SceneSelectorConfig.asset";

        public static bool HasConfig => AssetDatabaseUtils.TryFindAssets<SceneSelectorConfig>(out var result);

        public static SceneSelectorConfig LoadConfig() => AssetDatabaseUtils.FindAndLoadAsset<SceneSelectorConfig>();
        
        [MenuItem(MenuItems.Tools.CreateConfig)]
        private static void CreateConfig()
        {
            if (HasConfig)
            {
                UI.NotifyConfigExists();
                return;
            }
            
            if (!AssetDatabase.IsValidFolder(DefaultConfigDirectory))
            {
                Directory.CreateDirectory(DefaultConfigDirectory);
            }
            
            var config = ScriptableObject.CreateInstance<SceneSelectorConfig>();
            AssetDatabase.CreateAsset(config, DefaultConfigPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            UI.NotifyConfigCreated();
        }

        public static class UI
        {
            private const string Ok = "OK";
            private const string Yes = "Yes";
            private const string No = "No";
            
            private const string ConfigCreationTitle = "Config Missing";
            private const string ConfigCreationMessage = "Config File not found. We'll create one and place it in:\n" +
                                                         DefaultConfigPath + "\nYou can move or rename it after " +
                                                         "creation, but keep it in Editor folder.";
            
            private const string SetupIncompleteTitle = "Setup Incomplete";
            private const string SetupIncompleteMessage = "Config file not created.\nSome functionality is disabled";

            private const string ConfigCreatedTitle = "Config Created";
            private const string ConfigCreatedMessage = "Config file is created. You can move it to any location, " +
                                                        "but keep it under Editor folder:\n" +
                                                        DefaultConfigPath;

            private const string ConfigExistsTitle = "Config Exists";
            private const string ConfigExistsMessage = "Config file already exists. No need to create a new one";
            
            public static void AskToCreateConfig()
            {
                var isAccepting = EditorUtility.DisplayDialog(ConfigCreationTitle, ConfigCreationMessage, Yes, No);

                if (!isAccepting)
                {
                    EditorUtility.DisplayDialog(SetupIncompleteTitle, SetupIncompleteMessage, Ok);
                    return;
                }
                
                CreateConfig();
            }

            public static void NotifyConfigCreated()
            {
                EditorUtility.DisplayDialog(ConfigCreatedTitle, ConfigCreatedMessage, Ok);
            }

            public static void NotifyConfigExists()
            {
                EditorUtility.DisplayDialog(ConfigExistsTitle, ConfigExistsMessage, Ok);
            }
        }
    }
}