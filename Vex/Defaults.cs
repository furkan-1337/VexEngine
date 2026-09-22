using System;
using System.Collections.Generic;
using System.Text;

namespace Vex
{
    public class Defaults
    {
        public static class Shader
        {
            public const string VertexShader = """
                #version 330 core

                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec4 aColor;
                layout (location = 2) in vec2 aTexCoords;
                layout (location = 3) in float aTexIndex;

                uniform mat4 uViewProjection;

                out vec4 vColor;
                out vec2 vTexCoords;
                out float vTexIndex;

                void main()
                {
                    vColor = aColor;
                    vTexCoords = aTexCoords;
                    vTexIndex = aTexIndex;
                    gl_Position = uViewProjection * vec4(aPosition, 1.0);
                }
                """;

            public const string FragmentShader = """
                #version 330 core

                in vec4 vColor;
                in vec2 vTexCoords;
                in float vTexIndex;

                uniform sampler2D uTextures[16];

                out vec4 FragColor;

                void main()
                {
                    FragColor = texture(uTextures[int(vTexIndex)], vTexCoords) * vColor;
                }
                """;
        }
    }
}
