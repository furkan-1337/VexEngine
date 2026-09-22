using System;
using System.Collections.Generic;
using System.Text;
using Vex.Debugging;

namespace Vex.Engine.Scenes
{
    public class SceneManager
    {
        public static Scene? CurrentScene { get; private set; } = null;
        private static Scene? _nextScene = null;

        //public static Scene? GetCurrent()
        //{
        //    return CurrentScene ?? throw new VexInvalidStateException("No current scene is set.");
        //}

        public static void SetScene(Scene scene)
        {
            _nextScene = scene;
            Debug.Log(Debug.LogLevel.Info, CurrentScene == null ? $"Scene: {_nextScene.Name}" : $"Next scene selected: {_nextScene.Name}");
        }

        internal static void Update(double deltaTime)
        {
            SwitchScene();
            CurrentScene?.Update(deltaTime);
        }

        internal static void Render(double deltaTime)
        {
            CurrentScene?.Render(deltaTime);
        }

        private static void SwitchScene()
        {
            bool isFirstScene = CurrentScene == null;
            string currentSceneName = CurrentScene?.Name ?? "null";
            string nextSceneName = _nextScene?.Name ?? "null";
            if (_nextScene == null) return;
            CurrentScene?.Destroy();
            CurrentScene = _nextScene;
            CurrentScene.Initialize();
            _nextScene = null;

            if(!isFirstScene)
                Debug.Log(Debug.LogLevel.Info, $"Scene switched: {currentSceneName} -> {nextSceneName}");
        }

        internal static void Shutdown()
        {
            CurrentScene?.Destroy();
            CurrentScene = null;
            _nextScene = null;
        }
    }
}
