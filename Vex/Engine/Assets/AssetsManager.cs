using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using Vex.Debugging;

namespace Vex.Engine.Assets
{
    public class AssetsManager
    {
        private static GL? _gl = null;
        private static int _nextId = 0;
        private static readonly Dictionary<string, GameAsset> _assets = new();

        public static void Initialize(GL gl)
        {
            _gl = gl ?? throw new VexArgumentNullException("AssetsManager: GL instance cannot be null.");
            Debug.Log(Debug.LogLevel.Info, "AssetsManager initialized successfully.");
        }

        public static Texture LoadTexture(string filePath, GLEnum filtering = GLEnum.Linear, GLEnum wrapping = GLEnum.Repeat)
        {
            if(_gl == null)
                throw new VexInvalidStateException("AssetsManager is not initialized.");

            string name = Path.GetFileNameWithoutExtension(filePath);
            if (_assets.TryGetValue(name, out var existing))
                return (Texture)existing;

            var texture = new Texture(_gl,filePath, filtering, wrapping);

            texture.Id = _nextId++;
            _assets.Add(name, texture);
            Debug.Log(Debug.LogLevel.Info, $"Texture '{name}' loaded successfully.");
            return texture;
        }

        public static T Get<T>(string name) where T : GameAsset
        {
            if (_assets.TryGetValue(name, out var asset) && asset is T typedAsset)
                return typedAsset;
            throw new VexAssetNotFoundException($"Asset '{name}' of type {typeof(T).Name} not found.");
        }

        public static void Shutdown()
        {
            foreach(GameAsset asset in _assets.Values)
                asset.Dispose();
            _assets.Clear();
            _nextId = 0;
            Debug.Log(Debug.LogLevel.Info, "AssetsManager shut down successfully.");
        }
    }
}
