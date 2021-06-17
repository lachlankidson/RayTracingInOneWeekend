namespace RayTracing.Textures
{
    public abstract class Texture
    {
        public virtual Vec3 Value(double u, double v, Vec3 point) => new(0);
    }
}
