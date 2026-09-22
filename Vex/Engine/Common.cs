using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Text;
using Vex.Debugging;
using Vex.Windowing;

namespace Vex.Engine
{
    public class Common
    {
        private static GameWindow? _window = null;
        public static GameWindow Window
        {
            get => _window ?? throw new VexInvalidStateException("Engine is not running! Call Core.Run() first.");
            set
            {
                if (_window != null)
                    throw new VexInvalidStateException("Game Window has already been set.");
                _window = value;
            }
        }
    }
}
