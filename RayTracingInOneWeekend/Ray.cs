namespace RayTracing
{
    public class Ray
    {
        public Ray(Vec3 origin, Vec3 direction, double time = 0)
        {
            this.Origin = origin;
            this.Direction = direction;
            this.Time = time;
        }

        public Vec3 Origin { get; init; }

        public Vec3 Direction { get; init; }

        public double Time { get; init; }

        public Vec3 At(double t)
        {
            return this.Origin + (t * this.Direction);
        }
    }
}
