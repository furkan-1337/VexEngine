using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using Vex.Debugging;
using Vex.Engine.Assets;

namespace Vex.Graphics
{
    public static class Renderer2D
    {
        private static GL _gl = null!;
        private static Vector4D<float> _clearColor = new Vector4D<float>(0.1f, 0.1f, 0.15f, 1.0f);
        private static Shader _defaultShader = null!;
        private static Shader _currentShader = null!;

        private static uint _vao; // Vertex Array Object (Works like a definer, it defines the layout of the vertex data)
        private static uint _vbo; // Array of vertices
        private static uint _ebo; // Array of indices (defines the order of vertices to draw)

        private static uint _indexCount = 0; // Current number of indices in the batch
        private static uint _vertexCount = 0; // Current number of vertices in the batch

        private static readonly uint _maxQuads = 10000; // Maximum number of quads that can be drawn in a single batch
        private static uint _maxIndices = _maxQuads * 6; // Maximum number of indices (6 indices per quad)
        private static uint _maxVertices = _maxQuads * 4; // Maximum number of vertices (4 vertices per quad)
        private static Vertex[] Vertices = new Vertex[_maxVertices]; // 40.000 vertices for 10.000 quads (4 vertices per quad)

        private static uint _whiteTexture;
        private static readonly uint[] _textureSlots = new uint[16];
        private static uint _textureSlotIndex = 1;

        public static GL? GetContext() => _gl;
        internal unsafe static void Initialize(GL gl)
        {
            _gl = gl ?? throw new VexInvalidStateException($"Failed to initialize Renderer2D: OpenGL context ({nameof(gl)}) is null. Ensure window is loaded before initializing renderer.");

            _defaultShader = Shader.Create(_gl);
            _currentShader = _defaultShader;

            _defaultShader.Use();
            _defaultShader.SetMatrix4("uViewProjection", Matrix4x4.Identity);

            // Alpha Blending
            _gl.Enable(EnableCap.Blend);
            _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);

            _vbo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(_maxVertices * sizeof(Vertex)), null, BufferUsageARB.DynamicDraw);

            // EBO
            uint[] indices = new uint[_maxIndices];
            uint offset = 0;
            for (int i = 0; i < _maxIndices; i += 6)
            {
                indices[i + 0] = offset + 0;
                indices[i + 1] = offset + 1;
                indices[i + 2] = offset + 2;
                indices[i + 3] = offset + 2;
                indices[i + 4] = offset + 3;
                indices[i + 5] = offset + 0;
                offset += 4;
            }

            _ebo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
            fixed (uint* i = indices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);
            }

            uint stride = (uint)sizeof(Vertex); // 40 byte

            // layout (location = 0) in vec3 aPosition -> 3 float (12 byte)
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0);
            _gl.EnableVertexAttribArray(0);

            // layout (location = 1) in vec4 aColor -> 4 float (16 byte)
            _gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, stride, (void*)(3 * sizeof(float)));
            _gl.EnableVertexAttribArray(1);

            // layout (location = 2) in vec2 aTexCoords -> 2 float (8 byte)
            _gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, (void*)(7 * sizeof(float)));
            _gl.EnableVertexAttribArray(2);

            // layout (location = 3) in float aTexIndex -> 1 float (4 byte)
            _gl.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, stride, (void*)(9 * sizeof(float)));
            _gl.EnableVertexAttribArray(3);

            int[] samplers = new int[16];
            for (int i = 0; i < 16; i++) samplers[i] = i;
            fixed (int* s = samplers)
            {
                int loc = _gl.GetUniformLocation(_defaultShader.Id, "uTextures");
                _gl.Uniform1(loc, 16, s);
            }

            // White Texture (for render colours without actual texture)
            _whiteTexture = _gl.GenTexture();
            _gl.BindTexture(TextureTarget.Texture2D, _whiteTexture);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            uint whiteColor = 0xffffffff;
            _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, 1, 1, 0, PixelFormat.Rgba, PixelType.UnsignedByte, &whiteColor);
            _textureSlots[0] = _whiteTexture;
        }

        public static void SetShader(Shader? shader)
        {
            _currentShader = shader ?? _defaultShader;
        }

        public static void BeginScene(Camera2D? camera = null)
        {
            _currentShader.Use();
            if(camera != null)
                _currentShader.SetMatrix4("uViewProjection", camera.ViewProjection);
        }

        public unsafe static void EndScene()
        {
            DrawBatch();
        }

        private static float GetTextureIndex(Texture2D texture)
        {
            for (uint i = 1; i < _textureSlotIndex; i++)
            {
                if (_textureSlots[i] == texture.Handle)
                    return (float)i;
            }
            if (_textureSlotIndex >= 16)
                DrawBatch();

            float index = (float)_textureSlotIndex;
            _textureSlots[_textureSlotIndex] = texture.Handle;
            _textureSlotIndex++;
            return index;
        }

        public static void DrawQuad(Vector2D<float> position, Vector2D<float> size, Vector4D<float> color)
        {
            DrawQuadInternal(position, size, color, textureIndex: 0.0f);
        }

        public static void DrawQuad(Vector2D<float> position, Vector2D<float> size, Texture2D texture, Vector4D<float> color)
        {
            float textureIndex = GetTextureIndex(texture);
            DrawQuadInternal(position, size, color, textureIndex);
        }

        public static void DrawQuad(Vector2D<float> position, Vector2D<float> size, Texture2D texture)
        {
            DrawQuad(position, size, texture, new Vector4D<float>(1.0f, 1.0f, 1.0f, 1.0f));
        }

        private static void DrawQuadInternal(Vector2D<float> position, Vector2D<float> size, Vector4D<float> color, float textureIndex)
        {
            if (_indexCount >= _maxIndices)
                DrawBatch();
            float halfW = size.X / 2.0f;
            float halfH = size.Y / 2.0f;
            Add(new Vertex(new Vector3D<float>(position.X - halfW, position.Y - halfH, 0.0f), color, new Vector2D<float>(0.0f, 0.0f), textureIndex));
            Add(new Vertex(new Vector3D<float>(position.X + halfW, position.Y - halfH, 0.0f), color, new Vector2D<float>(1.0f, 0.0f), textureIndex));
            Add(new Vertex(new Vector3D<float>(position.X + halfW, position.Y + halfH, 0.0f), color, new Vector2D<float>(1.0f, 1.0f), textureIndex));
            Add(new Vertex(new Vector3D<float>(position.X - halfW, position.Y + halfH, 0.0f), color, new Vector2D<float>(0.0f, 1.0f), textureIndex));
            _indexCount += 6;
        }

        private static unsafe void DrawBatch()
        {
            if (_indexCount == 0) return;

            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            fixed (Vertex* v = Vertices)
            {
                _gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0, (nuint)(_vertexCount * sizeof(Vertex)), v);
            }

            _gl.BindVertexArray(_vao);
            for (int i = 0; i < _textureSlotIndex; i++)
            {
                _gl.ActiveTexture(TextureUnit.Texture0 + i);
                _gl.BindTexture(TextureTarget.Texture2D, _textureSlots[i]);
            }
            _gl.DrawElements(PrimitiveType.Triangles, (uint)_indexCount, DrawElementsType.UnsignedInt, (void*)0);

            _vertexCount = 0;
            _indexCount = 0;
            _textureSlotIndex = 1;
        }

        private static void Add(Vertex vertex)
        {
            Vertices[_vertexCount] = vertex;
            _vertexCount++;
        }

        internal static void Shutdown()
        {
            _gl.DeleteVertexArray(_vao);
            _gl.DeleteBuffer(_vbo);
            _gl.DeleteBuffer(_ebo);
            _defaultShader?.Dispose();
            _gl?.DeleteTexture(_whiteTexture);
        }
    }
}
