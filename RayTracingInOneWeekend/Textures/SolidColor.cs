namespace RayTracing.Textures
{
    using System.Numerics;

    public record SolidColor(Vector3 ColorValue) : Texture
    {
        public SolidColor(float red, float green, float blue)
            : this(new Vector3(red, green, blue))
        {
        }

        public SolidColor(float grayscale)
            : this(new Vector3(grayscale))
        {
        }

        public override Vector3 Value(float u, float v, Vector3 point)
        {
            return this.ColorValue;
        }
    }
}