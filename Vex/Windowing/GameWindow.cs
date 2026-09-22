using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using Vex.Debugging;
using Vex.Engine;
using Vex.Engine.Assets;
using Vex.Engine.Scenes;
using Vex.Graphics;

namespace Vex.Windowing
{
    public class GameWindow : IDisposable
    {
        private IWindow _window = null!;
        private GL? _gl = null!;
        public IWindow GetWindow() => _window;
        public GL? GetOpenGL() => _gl;

        public GameWindow(WindowOptions options)
        {
            _window = Window.Create(options);

            _window.Load += Load;
            _window.Update += Update;
            _window.Render += Render;
            _window.Closing += Closing;
            _window.FramebufferResize += OnFramebufferResize;
        }

        public static GameWindow Create(string title = "Vex Engine", int x = 100, int y = 100, int width = 800, int height = 600)
        {
            var options = WindowOptions.Default;
            options.Title = title;
            options.Position = new Vector2D<int>(x, y);
            options.Size = new Vector2D<int>(width, height);

            return new GameWindow(options);
        }

        internal void Run()
        {
            _window.Run();
        }

        public Vector2D<int> FramebufferSize => _window.FramebufferSize;

        public Vector2D<int> Position
        {
            get => _window.Position;
            set => _window.Position = value;
        }

        public Vector2D<int> Size
        {
            get => _window.Size;
            set => _window.Size = value;
        }

        public bool VSync
        {
            get => _window.VSync;
            set => _window.VSync = value;
        }

        private void Load()
        {
            _gl = GL.GetApi(_window);
            _gl.Viewport(0, 0, (uint)_window.FramebufferSize.X, (uint)_window.FramebufferSize.Y);

            Input.Initialize(_window);
            Renderer2D.Initialize(_gl);
            AssetsManager.Initialize(_gl);
            Debug.Log(Debug.LogLevel.Info, "Window loaded successfully.");
        }

        private void Update(double deltaTime)
        {
            SceneManager.Update((float)deltaTime);
            Input.Update();
        }

        private void Render(double deltaTime)
        {
            if(_gl == null)
                throw new VexException("OpenGL context is not initialized.");

            var color = SceneManager.CurrentScene?.ClearColor ?? new Vector4D<float>(0.1f, 0.1f, 0.15f, 1.0f);
            _gl.ClearColor(color.X, color.Y, color.Z, color.W);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            Renderer2D.BeginScene(SceneManager.CurrentScene?.Camera);
            SceneManager.Render(deltaTime);
            Renderer2D.EndScene();
        }

        private void Closing()
        {
            SceneManager.Shutdown();
            Input.Shutdown();
            Renderer2D.Shutdown();
            AssetsManager.Shutdown();
            Debug.Log(Debug.LogLevel.Info, "Window closed successfully.");
        }

        private void OnFramebufferResize(Vector2D<int> size)
        {
            _gl?.Viewport(0, 0, (uint)size.X, (uint)size.Y);
            SceneManager.CurrentScene?.Camera?.SetViewportSize(size.X, size.Y);
            //Debug.Log(Debug.LogLevel.Info, $"Framebuffer resized to {size.X}x{size.Y}");
        }

        public void Dispose()
        {
            _window?.Dispose();
            _gl?.Dispose();
        }
    }
}
