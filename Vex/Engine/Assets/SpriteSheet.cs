using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;
using Vex.Debugging;

namespace Vex.Engine.Assets
{
    public class SpriteSheet
    {
        public Texture Texture { get; private set; }
        public Vector2D<int> CellSize { get; private set; }
        public List<SubTexture> SubTextures { get; private set; } = new List<SubTexture>();
        public int Columns => Texture.Width / CellSize.X;
        public int Rows => Texture.Height / CellSize.Y;

        public SpriteSheet(Texture texture, Vector2D<int> cellSize)
        {
            Texture = texture ?? throw new VexArgumentNullException($"Texture object ({nameof(texture)}) was null.");
            CellSize = cellSize;

            for (int y = 0; y < Rows; y++)
            {
                int actualY = (Rows - 1) - y;
                for (int x = 0; x < Columns; x++)
                {
                    SubTextures.Add(SubTexture.CreateFromCoords(Texture, new Vector2D<int>(x, actualY), CellSize));
                }
            }
        }

        public SubTexture this[int index] => GetSprite(index);
        public SubTexture this[int x, int y] => GetSprite(x, y);

        public SubTexture GetSprite(int index) 
        {
            if (index < 0 || index >= SubTextures.Count)
                throw new VexArgumentOutOfRangeException($"Index ({nameof(index)}) is out of range.");
            return SubTextures[index];
        }

        public SubTexture GetSprite(int x, int y)
        {
            if (x < 0 || x >= Columns || y < 0 || y >= Rows)
                throw new VexArgumentOutOfRangeException($"Coordinates ({x}, {y}) are out of range.");

            return SubTextures[y * Columns + x];
        }
    }
}
