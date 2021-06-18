namespace RayTracing.Textures
{
    public record NoiseTexture(Perlin Noise, double Scale) : Texture
    {
        public override Vec3 Value(double u, double v, Vec3 point)
        {
            return new Vec3(1, 1, 1) * 0.5 * (1 + this.Noise.Noise(this.Scale * point));
        }
    }
}
