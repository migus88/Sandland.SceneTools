using System.Threading.Tasks;
using UnityEditor.PackageManager;

namespace Sandland.SceneTool.Editor.Common.Utils
{
    public static class Utils
    {
        public static bool IsAddressablesInstalled =>
            PackageInfo.FindForAssetPath("Packages/com.unity.addressables/AddressableAssets")?.name == "com.unity.addressables";
    }
}