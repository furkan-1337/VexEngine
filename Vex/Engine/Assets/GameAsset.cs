using System;
using System.Collections.Generic;
using System.Text;

namespace Vex.Engine.Assets
{
    public abstract class GameAsset : IDisposable
    {
        public int Id { get; internal set; } = -1;
        public string FileName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public AssetType Type { get; protected set; } = AssetType.None;

        public abstract void Dispose();
    }

    public enum AssetType
    {
        None,
        Texture,
        Audio
    }
}
