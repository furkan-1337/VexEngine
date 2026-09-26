using System;
using System.Collections.Generic;
using System.Text;

namespace Vex
{
    public class Math
    {
        public const float Deg2Rad = MathF.PI / 180.0f;
        public const float Rad2Deg = 180.0f / MathF.PI;


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static (float Sin, float Cos) SinCosDeg(float degrees)
        {
            return MathF.SinCos(degrees * Deg2Rad);
        }

        public static float Sin(float degrees)
        {
            return SinCosDeg(degrees).Sin;
        }

        public static float Cos(float degrees)
        {
            return SinCosDeg(degrees).Cos;
        }
    }
}
