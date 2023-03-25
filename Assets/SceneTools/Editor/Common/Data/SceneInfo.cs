namespace Sandland.SceneTool.Editor.Common.Data
{
    internal struct SceneInfo
    {
        public string Name { get; set; }

        public int BuildIndex { get; set; }
        public string Address { get; set; }

        public SceneInfo(string name, int buildIndex, string address)
        {
            Name = name;
            BuildIndex = buildIndex;
            Address = address;
        }
    }
}