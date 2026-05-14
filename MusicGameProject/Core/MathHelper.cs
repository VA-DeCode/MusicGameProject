using System;

namespace MusicGameProject.Core
{
    public static class MathHelper
    {
        public static float Lerp(float a, float b, float t) =>
            a + (b - a) * Math.Clamp(t, 0, 1);
    }
}