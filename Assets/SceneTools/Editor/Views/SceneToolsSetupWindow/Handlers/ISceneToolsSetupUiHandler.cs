namespace Sandland.SceneTool.Editor.Views.Handlers
{
    public interface ISceneToolsSetupUiHandler
    {
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
        void Apply();
    }
}