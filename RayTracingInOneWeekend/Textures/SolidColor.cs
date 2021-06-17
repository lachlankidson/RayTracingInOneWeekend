namespace RayTracing.Textures
{
    public class SolidColor : Texture
    {
        public Vec3 ColorValue { get; init; }

        public SolidColor(Vec3 color)
        {
            this.ColorValue = color;
        }

        public SolidColor(double red, double green, double blue)
        {
            this.ColorValue = new Vec3(red, green, blue);
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
