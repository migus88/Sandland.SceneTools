using UnityEngine;

namespace SceneTools.Runtime.Settings
{
    public class ProjectScenes : ScriptableObject
    {
        [field: SerializeField] public string[] Names { get; set; }
        [field: SerializeField] public int[] Indexes { get; set; }
        [field: SerializeField] public string[] Addressables { get; set; }
    }
}