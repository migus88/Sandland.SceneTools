using System.Collections.Generic;
using SandScene.Editor.Common.Utils;
using UnityEngine;

namespace SandScene.Editor.SandScene.Editor.Common.Data
{
    public class SceneSelectorConfig : ScriptableObject
    {
        [field:SerializeField] public List<string> Favorites { get; set; }
    }
}