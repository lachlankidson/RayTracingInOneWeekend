namespace RayTracingInOneWeekend
{
    public class Camera
    {
        public Camera()
        {
            const double aspectRatio = 16 / 9.0;
            const double viewportHeight = 2;
            const double viewportWidth = aspectRatio * viewportHeight;
            const double focalLength = 1;

            this.Origin = new Vec3(0, 0, 0);
            this.Horizontal = new Vec3(viewportWidth, 0, 0);
            this.Vertical = new Vec3(0, viewportHeight, 0);
            this.LowerLeftCorner = this.Origin - (this.Horizontal / 2) - (this.Vertical / 2) - new Vec3(0, 0, focalLength);
        }

        public Vec3 Origin { get; init; }

        public Vec3 LowerLeftCorner { get; init; }

        public Vec3 Horizontal { get; set; }

        public Vec3 Vertical { get; set; }

        public Ray GetRay(double u, double v) =>
            new (this.Origin, this.LowerLeftCorner + (u * this.Horizontal) + (v * this.Vertical) - this.Origin);
    }
}
