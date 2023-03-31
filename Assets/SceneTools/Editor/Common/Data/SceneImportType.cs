using System.ComponentModel;

namespace Sandland.SceneTool.Editor.Common.Data
{
    public enum SceneImportType
    {
        [Description("Not Used")]
        None = 0,
        [Description("In Build Settings")]
        BuildSettings = 1,
        [Description("In Addressable Group")]
        Addressables = 2
        // TODO: Add Asset Bundles support
    }
}