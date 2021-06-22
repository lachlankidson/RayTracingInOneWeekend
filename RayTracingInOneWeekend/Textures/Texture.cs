namespace RayTracing.Textures
{
    using System.Numerics;

    public abstract record Texture()
    {
        public virtual Vector3 Value(float u, float v, Vector3 point) => Vector3.Zero;
    }
}
