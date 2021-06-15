namespace RayTracing
{
    public class Ray
    {
        public Ray(Vec3 origin, Vec3 direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        public Vec3 Origin { get; init; }

        public Vec3 Direction { get; init; }

        public Vec3 At(double t)
        {
            return this.Origin + (t * this.Direction);
        }
    }
}
