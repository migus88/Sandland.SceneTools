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
            public string Address;
            public SceneType Type;

            public OptionValue() { }

            public OptionValue(string name, SceneType type)
            {
                Name = name;
                Address = string.Empty;
                Type = type;
            }
            
            public OptionValue(string name, string address, SceneType type)
            {
                Name = name;
                Address = address;
                Type = type;
            }
        }
    }
}