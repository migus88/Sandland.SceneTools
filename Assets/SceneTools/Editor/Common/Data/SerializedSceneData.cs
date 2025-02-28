using System.Collections.Generic;
using Sandland.SceneTool.Runtime.Enums;

namespace Sandland.SceneTool.Editor.Common.Data
{
    [System.Serializable]
    public class SerializedScenesData
    {
        public List<string> Options;
        public List<OptionValue> OptionValues;

        [System.Serializable]
        public class OptionValue
        {
            public string Name;
            public SceneType Type;

            public OptionValue() { }

            public OptionValue(string name, SceneType type)
            {
                Name = name;
                Type = type;
            }
        }
    }
}