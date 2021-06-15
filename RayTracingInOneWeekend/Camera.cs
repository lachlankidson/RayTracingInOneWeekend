namespace RayTracing
{
    using System;

    public class Camera
    {
        public Camera(
            Vec3 lookFrom,
            Vec3 lookAt,
            Vec3 viewUp,
            double verticalFov,
            double aspectRatio)
        {
            double theta = (Math.PI / 180) * verticalFov;
            double h = Math.Tan(theta / 2);
            double viewportHeight = 2 * h;
            double viewportWidth = aspectRatio * viewportHeight;

            Vec3 w = Vec3.UnitVector(lookFrom - lookAt);
            Vec3 u = Vec3.UnitVector(Vec3.CrossProduct(viewUp, w));
            Vec3 v = Vec3.CrossProduct(w, u);

            this.Origin = lookFrom;
            this.Horizontal = viewportWidth * u;
            this.Vertical = viewportHeight * v;
            this.LowerLeftCorner = this.Origin - (this.Horizontal / 2) - (this.Vertical / 2) - w;
        }

        public Vec3 Origin { get; init; }

        public Vec3 LowerLeftCorner { get; init; }

        public Vec3 Horizontal { get; set; }

        public Vec3 Vertical { get; set; }

        public Ray GetRay(double s, double t) =>
            new (this.Origin, this.LowerLeftCorner + (s * this.Horizontal) + (t * this.Vertical) - this.Origin);
    }
}
