using System.Collections.Generic;

namespace Sandland.SceneTool.Editor.Common.Data
{
    internal class AssetFileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Guid { get; set; }
        public string BundleName { get; set; }
        public List<string> Labels { get; set; }

        public AssetFileInfo(string name, string path, string guid, string bundleName, List<string> labels)
        {
            Name = name;
            Path = path;
            Guid = guid;
            BundleName = bundleName;
            Labels = labels;
        }
    }
}