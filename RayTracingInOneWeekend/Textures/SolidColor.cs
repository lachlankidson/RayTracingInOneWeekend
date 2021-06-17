namespace RayTracing.Textures
{
    public record SolidColor(Vec3 ColorValue) : Texture
    {
        public SolidColor(double red, double green, double blue)
            : this(new Vec3(red, green, blue))
        {
        }

        public SolidColor(double grayscale)
            : this(grayscale, grayscale, grayscale)
        {
        }

        public override Vec3 Value(double u, double v, Vec3 point)
        {
            return this.ColorValue;
        }
    }
}