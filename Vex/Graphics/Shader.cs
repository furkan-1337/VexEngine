using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vex.Debugging;

namespace Vex.Graphics
{
    public class Shader : IDisposable
    {
        public uint Id { get; private set; }
        private readonly GL _gl;
        private readonly Dictionary<string, int> _uniformLocations = new();
        private bool _disposed;

        public Shader(GL gl, string vertexSource, string fragmentSource)
        {
            _gl = gl ?? throw new VexInvalidStateException($"Failed to initialize Shader: OpenGL context ({nameof(gl)}) is null. Ensure window is loaded before initializing shader.");

            uint vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
            uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);

            Id = _gl.CreateProgram();
            _gl.AttachShader(Id, vertexShader);
            _gl.AttachShader(Id, fragmentShader);
            _gl.LinkProgram(Id);

            _gl.GetProgram(Id, ProgramPropertyARB.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                string infoLog = _gl.GetProgramInfoLog(Id);
                _gl.DeleteProgram(Id);
                _gl.DeleteShader(vertexShader);
                _gl.DeleteShader(fragmentShader);
                throw new ShaderCompilationException("Program", $"Shader program linking error:\n{infoLog}");
            }

            _gl.DetachShader(Id, vertexShader);
            _gl.DetachShader(Id, fragmentShader);
            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            _gl.UseProgram(Id);
        }

        public static Shader Create(GL gl, string vertexSource = Defaults.Shader.VertexShader, string fragmentSource = Defaults.Shader.FragmentShader)
        {
            return new Shader(gl, vertexSource, fragmentSource);
        }

        #region Uniform Helpers

        public void SetInt(string name, int value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
                _gl.Uniform1(location, value);
        }

        public void SetFloat(string name, float value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
                _gl.Uniform1(location, value);
        }

        public void SetVector2(string name, Vector2 value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
                _gl.Uniform2(location, value.X, value.Y);
        }

        public void SetVector3(string name, Vector3 value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
                _gl.Uniform3(location, value.X, value.Y, value.Z);
        }

        public void SetVector4(string name, Vector4 value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
                _gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
        }

        public unsafe void SetMatrix4(string name, Matrix4x4 value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
            {
                _gl.UniformMatrix4(location, 1, false, (float*)&value);
            }
        }

        private int GetUniformLocation(string name)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
                return location;

            location = _gl.GetUniformLocation(Id, name);
            if (location == -1)
            {
                // For performance, only log as a warning; do not throw to avoid crashing the engine
                Console.WriteLine($"[Warning] Uniform '{name}' was not found in shader or was optimized out.");
            }

            _uniformLocations[name] = location;
            return location;
        }

        #endregion

        #region Private Compilation Helper

        private uint CompileShader(ShaderType type, string source)
        {
            uint shader = _gl.CreateShader(type);
            _gl.ShaderSource(shader, source);
            _gl.CompileShader(shader);

            _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
            if (status == 0)
            {
                string infoLog = _gl.GetShaderInfoLog(shader);
                _gl.DeleteShader(shader);
                throw new ShaderCompilationException(type.ToString(), $"Compilation error:\n{infoLog}");
            }

            return shader;
        }

        #endregion

        public void Dispose()
        {
            if (_disposed) return;

            if (Id != 0)
            {
                _gl.DeleteProgram(Id);
                Id = 0;
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    public static class ShaderExtensions
    {
        public static void SetUniform(this Shader shader, string name, int value) => shader.SetInt(name, value);
        public static void SetUniform(this Shader shader, string name, float value) => shader.SetFloat(name, value);
        public static void SetUniform(this Shader shader, string name, Vector2 value) => shader.SetVector2(name, value);
        public static void SetUniform(this Shader shader, string name, Vector3 value) => shader.SetVector3(name, value);
        public static void SetUniform(this Shader shader, string name, Vector4 value) => shader.SetVector4(name, value);
        public static void SetUniform(this Shader shader, string name, Matrix4x4 value) => shader.SetMatrix4(name, value);
        
    }
}
