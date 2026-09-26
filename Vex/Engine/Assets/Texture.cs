using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Vex.Debugging;
using Vex.Graphics;


namespace Vex.Engine.Assets
{
    public class Texture : GameAsset
    {
        private GL? _gl = null;
        public uint Handle;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool HasLoaded { get; set; } = false;
        private int Slot { get; set; } = 0;

        public GLEnum Filtering { get; private set; } = GLEnum.Linear;
        public GLEnum Wrapping { get; private set; } = GLEnum.Repeat;

        public unsafe Texture(GL gl, string filePath, GLEnum filtering = GLEnum.Linear, GLEnum wrapping = GLEnum.Repeat)
        {
            _gl = gl ?? throw new VexArgumentNullException("Texture2D: GL instance cannot be null.");

            if(_gl == null)
                throw new VexInvalidStateException("Failed to create Texture2D: OpenGL context is null.");

            StbImage.stbi_set_flip_vertically_on_load(1);
            if (File.Exists(filePath))
            {
                Type = AssetType.Texture;
                FileName = filePath;
                Name = Path.GetFileNameWithoutExtension(filePath);

                using (Stream stream = File.OpenRead(filePath))
                {
                    // Read Image Data
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    Width = image.Width;
                    Height = image.Height;
                    //Data = image.Data;

                    Filtering = filtering;
                    Wrapping = wrapping;

                    // Load Texture
                    Handle = _gl.GenTexture();
                    _gl.BindTexture(TextureTarget.Texture2D, Handle);

                    _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)Filtering);
                    _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)Filtering);

                    _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)Wrapping);
                    _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)Wrapping);

                    fixed (byte* ptr = image.Data)
                    {
                        _gl.TexImage2D(
                            TextureTarget.Texture2D,
                            0,                                   
                            InternalFormat.Rgba,                 
                            (uint)image.Width, (uint)image.Height,
                            0,                                   
                            PixelFormat.Rgba,                    
                            PixelType.UnsignedByte,              
                            ptr                                  
                        );
                    }

                    _gl.BindTexture(TextureTarget.Texture2D, 0);
                    HasLoaded = true;
                }
            }
            else
            {
                HasLoaded = false;
                throw new VexFileNotFoundException($"Texture file not found: {filePath}");
            }
        }

        public void Bind(int slot = 0)
        {
            Slot = slot;
            _gl?.ActiveTexture(TextureUnit.Texture0 + Slot);
            _gl?.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Unbind()
        {
            _gl?.ActiveTexture(TextureUnit.Texture0 + Slot);
            _gl?.BindTexture(TextureTarget.Texture2D, 0);
            Slot = 0;
        }

        public override void Dispose()
        {
            _gl?.DeleteTexture(Handle);
        }
    }
}
