namespace RayTracing
{
    using System.Numerics;

    public class Ray
    {
        public Ray(Vector3 origin, Vector3 direction, float time = 0)
        {
            this.Origin = origin;
            this.Direction = direction;
            this.Time = time;
        }

        public Vector3 Origin { get; init; }

        public Vector3 Direction { get; init; }

        public float Time { get; init; }

        public Vector3 At(float t) => this.Origin + (t * this.Direction);
    }
}
