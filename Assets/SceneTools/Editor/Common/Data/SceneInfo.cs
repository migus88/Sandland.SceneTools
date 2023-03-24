namespace Sandland.SceneTool.Editor.Common.Data
{
    public struct SceneInfo
    {
        public string Name { get; set; }
        public int BuildIndex { get; set; }
        //TODO: Implement support for addressables scenes

        public SceneInfo(string name, int buildIndex)
        {
            Name = name;
            BuildIndex = buildIndex;
        }
    }
}