using Sandland.SceneTool.Runtime.Enums;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if SANDLAND_UNITASK
using Cysharp.Threading.Tasks;
using Awaitable = Cysharp.Threading.Tasks.UniTask;
#elif UNITY_2023_1_OR_NEWER
using Awaitable = UnityEngine.Awaitable;
#else
using Awaitable = System.Threading.Tasks.Task;
#endif

namespace Sandland.SceneTool.Runtime.Data
{
    [System.Serializable]
    public class SelectableScene
    {
        [UnityEngine.SerializeField] private string _sceneName;
        [UnityEngine.SerializeField] private string _sceneAddress;
        [UnityEngine.SerializeField] private SceneType _sceneType;

        public SelectableScene()
        {
            _sceneName = string.Empty;
            _sceneAddress = string.Empty;
            _sceneType = SceneType.None;
        }

        public SelectableScene(string sceneName, SceneType sceneType)
        {
            _sceneName = sceneName;
            _sceneAddress = string.Empty;
            _sceneType = sceneType;
        }

        public SelectableScene(string sceneName, string sceneAddress, SceneType sceneType)
        {
            _sceneName = sceneName;
            _sceneAddress = sceneAddress;
            _sceneType = sceneType;
        }
        
        /// <summary>
        /// Tells if the scene is loaded.
        /// </summary>
        public bool IsLoaded()
        {
            if (string.IsNullOrEmpty(_sceneName))
            {
                UnityEngine.Debug.LogWarning("SelectableScene: Scene name is not set.");
                return false;
            }
            
            var scene = SceneManager.GetSceneByName(_sceneName);
            return scene.IsValid() && scene.isLoaded;
        }
        
        public T GetComponentInScene<T>() where T : UnityEngine.Component
        {
            var scene = SceneManager.GetSceneByName(_sceneName);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                UnityEngine.Debug.LogError($"Scene '{_sceneName}' is not valid or not loaded.");
                return null;
            }

            foreach (UnityEngine.GameObject rootObj in scene.GetRootGameObjects())
            {
                T component = rootObj.GetComponentInChildren<T>(true);
                if (component != null)
                {
                    return component;
                }
            }
    
            return null;
        }
        
        public async Awaitable Unload()
        {
            await WaitForOperation(
                SceneManager.UnloadSceneAsync(_sceneName)
            );
        }

        /// <summary>
        /// Loads the scene asynchronously using the proper method.
        /// For built-in scenes, uses SceneManager.
        /// For addressable scenes, uses the Addressables class.
        /// </summary>
        public async Awaitable Load(LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (string.IsNullOrEmpty(_sceneName) || _sceneType == SceneType.None)
            {
                UnityEngine.Debug.LogError("SelectableScene: Incorrect scene data.");
                return;
            }

            if (_sceneType == SceneType.BuiltIn)
            {
                await WaitForOperation(
                    SceneManager.LoadSceneAsync(_sceneName, mode)
                );
            }
            else if (_sceneType == SceneType.Addressable)
            {
#if SANDLAND_ADDRESSABLES
                await WaitForOperationHandle(
                    Addressables.LoadSceneAsync(_sceneAddress, mode)
                );
#endif
            }
            else
            {
                UnityEngine.Debug.LogError("SelectableScene: Unknown scene type.");
            }
        }

        private async Awaitable WaitForOperation(UnityEngine.AsyncOperation operation)
        {
#if SANDLAND_UNITASK
            await Awaitable.WaitUntil(() => operation.isDone);
#elif UNITY_2023_1_OR_NEWER
            while (operation.isDone)
            {
                await Awaitable.NextFrameAsync();
            }
#else
            while(operation.isDone)
            {
                await Awaitable.Delay(System.TimeSpan.FromSeconds(Time.deltaTime * 1000));
            }
#endif
        }

        private async Awaitable WaitForOperationHandle(AsyncOperationHandle operation)
        {
#if SANDLAND_UNITASK
            await operation.ToUniTask();
#else
            await operation.Task;
#endif
        }
    }
}