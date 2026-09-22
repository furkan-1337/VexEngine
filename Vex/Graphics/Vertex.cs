using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vex.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Vertex
    {
        public Vector3D<float> Position;
        public Vector4D<float> Color;
        public Vector2D<float> TexCoord;
        public float TexIndex;

        public Vertex(Vector3D<float> position, Vector4D<float> color)
        {
            Position = position;
            Color = color;
        }

        public Vertex(Vector3D<float> position, Vector4D<float> color, Vector2D<float> texCoord, float texIndex)
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
            TexIndex = texIndex;
        }
    }
}
