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
        // This string is saved in the format "build--SceneName" or "addressable--SceneAddress"
        [UnityEngine.SerializeField] private string _sceneValue;

        /// <summary>
        /// Loads the scene asynchronously using the proper method.
        /// For built-in scenes, uses SceneManager.
        /// For addressable scenes, uses Addressables.
        /// </summary>
        public async Awaitable LoadScene(LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (string.IsNullOrEmpty(_sceneValue))
            {
                UnityEngine.Debug.LogError("SelectableScene: SceneValue is not set.");
                return;
            }

            var parts = _sceneValue.Split(new[] { "--" }, System.StringSplitOptions.None);
            if (parts.Length != 2)
            {
                UnityEngine.Debug.LogError("SelectableScene: SceneValue format is invalid.");
                return;
            }

            var type = parts[0].ToLower();
            var sceneName = parts[1];

            // TODO: extract types into consts and reuse them
            if (type == "build")
            {
                await WaitForOperation(
                    SceneManager.LoadSceneAsync(sceneName, mode)
                );
            }
#if SANDLAND_ADDRESSABLES
            else if (type == "addressable")
            {
                await WaitForOperationHandle(
                    Addressables.LoadSceneAsync(sceneName, mode)
                );
            }
#endif
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
            await operation;
#else
            await operation.Task;
#endif
        }
    }
}