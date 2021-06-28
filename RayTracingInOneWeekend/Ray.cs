namespace RayTracing
{
    using System.Numerics;

    public record Ray(Vector3 Origin, Vector3 Direction, float Time = 0)
    {
        public Vector3 At(float t) => this.Origin + (t * this.Direction);
    }
}