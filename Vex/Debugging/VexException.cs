using System;

namespace Vex.Debugging
{
    public class VexException : Exception
    {
        public VexException() : this("An unspecified Vex Engine error occurred.", null) { }

        public VexException(string message) : this(message, null) { }

        public VexException(string message, Exception? innerException) : base(message, innerException)
        {
            string formattedLog = innerException != null
                ? $"[{GetType().Name}] {message}\n  -> Inner Exception: {innerException.Message}"
                : $"[{GetType().Name}] {message}";

            Debug.Log(Debug.LogLevel.Error, formattedLog);
        }
    }

    public class VexArgumentNullException : VexException
    {
        public VexArgumentNullException(string message) : base(message) { }
        public VexArgumentNullException(string message, Exception? innerException) : base(message, innerException) { }
    }

    public class VexAssetNotFoundException : VexException
    {
        public VexAssetNotFoundException(string message) : base(message) { }
        public VexAssetNotFoundException(string message, Exception? innerException) : base(message, innerException) { }
    }

    public class VexFileNotFoundException : VexException
    {
        public VexFileNotFoundException(string message) : base(message) { }
        public VexFileNotFoundException(string message, Exception? innerException) : base(message, innerException) { }
    }

    public class VexInvalidArgumentException : VexException
    {
        public VexInvalidArgumentException(string message) : base(message) { }
        public VexInvalidArgumentException(string message, Exception? innerException) : base(message, innerException) { }
    }

    public class VexInvalidStateException : VexException
    {
        public VexInvalidStateException(string message) : base(message) { }
        public VexInvalidStateException(string message, Exception? innerException) : base(message, innerException) { }
    }

    public class ShaderCompilationException : VexException
    {
        public string ShaderType { get; }
        public string Log { get; }

        public ShaderCompilationException(string shaderType, string log)
            : base($"{shaderType} compilation failed!\nOpenGL Log:\n{log}")
        {
            ShaderType = shaderType;
            Log = log;
        }
    }

    public class ShaderLinkException : VexException
    {
        public string Log { get; }

        public ShaderLinkException(string log)
            : base($"Shader Program linking failed!\nOpenGL Log:\n{log}")
        {
            Log = log;
        }
    }
}
