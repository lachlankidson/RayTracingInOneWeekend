namespace RayTracing.Textures
{
    using System;

    public record NoiseTexture(Perlin Noise, double Scale) : Texture
    {
        public override Vec3 Value(double u, double v, Vec3 point)
        {
            return new Vec3(1, 1, 1) * 0.5 *
                (1 + Math.Sin((this.Scale * point.Z) + (10 * this.Noise.Turbulence(point))));
        }
    }
}
