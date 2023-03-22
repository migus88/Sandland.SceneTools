namespace SandScene.Editor.SandScene.Editor.Common.Data
{
    public struct AssetFileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Guid { get; set; }

        public AssetFileInfo(string name, string path, string guid)
        {
            Name = name;
            Path = path;
            Guid = guid;
        }
    }
}