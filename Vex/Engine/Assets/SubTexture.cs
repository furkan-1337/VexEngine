using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Text;
using Vex.Debugging;

namespace Vex.Engine.Assets
{
    public class SubTexture
    {
        public Texture Texture { get; private set; }
        public Vector2D<float>[] TexCoords { get; private set; } = new Vector2D<float>[4];

        public SubTexture(Texture texture, Vector2D<float> min, Vector2D<float> max)
        {
            Texture = texture ?? throw new VexArgumentNullException($"Texture object ({nameof(texture)}) was null.");

            TexCoords[0] = new Vector2D<float>(min.X, min.Y);
            TexCoords[1] = new Vector2D<float>(max.X, min.Y);
            TexCoords[2] = new Vector2D<float>(max.X, max.Y);
            TexCoords[3] = new Vector2D<float>(min.X, max.Y);
        }

        public static SubTexture CreateFromCoords(Texture texture, Vector2D<int> coords, Vector2D<int> cellSize)
        {
            if (texture == null)
                throw new VexArgumentNullException($"Texture object ({nameof(texture)}) was null.");

            Vector2D<float> min = new Vector2D<float>(
                (float)(coords.X * cellSize.X) / texture.Width,
                (float)(coords.Y * cellSize.Y) / texture.Height
            );

            Vector2D<float> max = new Vector2D<float>(
                (float)((coords.X + 1) * cellSize.X) / texture.Width,
                (float)((coords.Y + 1) * cellSize.Y) / texture.Height
            );

            return new SubTexture(texture, min, max);
        }
    }
}
