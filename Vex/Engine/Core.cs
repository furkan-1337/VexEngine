using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Text;
using Vex.Engine.Scenes;
using Vex.Windowing;

namespace Vex.Engine
{
    public class Core
    {
        public static void Run(GameWindow gameWindow, Scenes.Scene scene)
        {
            Common.Window = gameWindow;
            SceneManager.SetScene(scene);
            gameWindow.Run();
        }
    }
}
