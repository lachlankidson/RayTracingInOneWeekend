namespace RayTracing.Textures
{
    using System;
    using System.Numerics;

    public record CheckerTexture(Texture Even, Texture Odd) : Texture
    {
        public override Vector3 Value(float u, float v, Vector3 point)
        {
            float sines = (float)Math.Sin(10 * point.X) * (float)Math.Sin(10 * point.Y) * (float)Math.Sin(10 * point.Z);
            Func<float, float, Vector3, Vector3> m = sines < 0 ? this.Odd.Value : this.Even.Value;
            return m(u, v, point);
        }
    }
}
