using UnityEditor;

namespace Sandland.SceneTool.Editor.Common.Utils
{
    internal class DefineUtils
    {
        public static void AddDefine(string define)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (defines.Contains(define))
            {
                return;
            }

            defines += $";{define}";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }

        public static void RemoveDefine(string define)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            if (!defines.Contains(define))
            {
                return;
            }

            defines = defines.Replace(define + ";", string.Empty).Replace(define, string.Empty);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }
    }
}