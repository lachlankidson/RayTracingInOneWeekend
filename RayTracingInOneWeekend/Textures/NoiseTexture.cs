namespace RayTracing.Textures
{
    public record NoiseTexture(Perlin Noise) : Texture
    {
        public override Vec3 Value(double u, double v, Vec3 point)
        {
            return new Vec3(1, 1, 1) * this.Noise.Noise(point);
        }
    }
}
