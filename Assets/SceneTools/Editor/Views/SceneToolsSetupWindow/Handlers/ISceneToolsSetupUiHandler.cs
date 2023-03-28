using System;

namespace Sandland.SceneTool.Editor.Views.Handlers
{
    internal interface ISceneToolsSetupUiHandler
    {
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
        void Apply();
    }
}