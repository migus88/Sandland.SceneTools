using UnityEditor;
using UnityEditor.Build;

namespace Sandland.SceneTool.Editor.Common.Utils
{
    internal class DefineUtils
    {
        public static void AddDefine(string define)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup); 
            var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            if (defines.Contains(define))
            {
                return;
            }

            defines += $";{define}";
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines);
        }

        public static void RemoveDefine(string define)
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            if (!defines.Contains(define))
            {
                return;
            }

            defines = defines.Replace(define + ";", string.Empty).Replace(define, string.Empty);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines);
        }
    }
}