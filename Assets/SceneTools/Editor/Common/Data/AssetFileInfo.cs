using System.Collections.Generic;

namespace Sandland.SceneTool.Editor.Common.Data
{
    public struct AssetFileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Guid { get; set; }
        public List<string> Labels { get; set; }

        public AssetFileInfo(string name, string path, string guid, List<string> labels)
        {
            Name = name;
            Path = path;
            Guid = guid;
            Labels = labels;
        }
    }
}