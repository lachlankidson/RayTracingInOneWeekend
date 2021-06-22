namespace RayTracing.Textures
{
    using System;
    using System.Numerics;

    public record NoiseTexture(Perlin Noise, float Scale) : Texture
    {
        public override Vector3 Value(float u, float v, Vector3 point)
        {
            return Vector3.One * .5f *
                (1 + (float)Math.Sin((this.Scale * point.Z) + (10 * this.Noise.Turbulence(point))));
        }
    }
}
