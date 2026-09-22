using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;
using Vex.Debugging;
using Vex.Graphics;

namespace Vex.Engine.Scenes
{
    public abstract class Scene : IDisposable
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
        public static int Count { get; private set; } = 0;
        public bool IsLoaded { get; internal set; } 
        public Vector4D<float>? ClearColor { get; set; }
        public Camera2D? Camera { get; set; } = new Camera2D();

        public Scene(string name)
        {
            Name = name;
            Id = Count++;
            IsLoaded = false;
        }

        internal void Initialize()
        {
            if (IsLoaded) return;
            Load();
            Camera?.SetViewportSize(Common.Window.FramebufferSize.X, Common.Window.FramebufferSize.Y);
            IsLoaded = true;
            Debug.Log(Debug.LogLevel.Info, $"Scene ({Name}) loaded.");
        }

        internal void Destroy()
        {
            if (!IsLoaded) return;
            Unload();
            IsLoaded = false;
            Debug.Log(Debug.LogLevel.Info, $"Scene ({Name}) unloaded.");
        }

        public virtual void Load() { }

        public virtual void Unload() { }

        public virtual void Update(double deltaTime) { }

        public virtual void Render(double deltaTime) { }

        public void Dispose()
        {
            if (IsLoaded)
            {
                Unload();
                IsLoaded = false;
            }
            Debug.Log(Debug.LogLevel.Info, $"Scene ({Name}) disposed.");
        }
    }
}
