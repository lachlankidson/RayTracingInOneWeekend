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
            double aspectRatio,
            double aperture,
            double focusDistance,
            double shutterOpen,
            double shutterClose)
        {
            double theta = (Math.PI / 180) * verticalFov;
            double h = Math.Tan(theta / 2);
            double viewportHeight = 2 * h;
            double viewportWidth = aspectRatio * viewportHeight;

            this.W = Vec3.UnitVector(lookFrom - lookAt);
            this.U = Vec3.UnitVector(Vec3.CrossProduct(viewUp, this.W));
            this.V = Vec3.CrossProduct(this.W, this.U);

            this.Origin = lookFrom;
            this.Horizontal = focusDistance * viewportWidth * this.U;
            this.Vertical = focusDistance * viewportHeight * this.V;
            this.LowerLeftCorner = this.Origin - (this.Horizontal / 2) - (this.Vertical / 2) - (focusDistance * this.W);
            this.LensRadius = aperture / 2;
            this.ShutterOpen = shutterOpen;
            this.ShutterClose = shutterClose;
        }

        public Vec3 Origin { get; init; }

        public Vec3 LowerLeftCorner { get; init; }

        public Vec3 Horizontal { get; init; }

        public Vec3 Vertical { get; init; }

        public Vec3 W { get; init; }

        public Vec3 U { get; init; }

        public Vec3 V { get; init; }

        public double LensRadius { get; init; }

        public double ShutterOpen { get; init; }

        public double ShutterClose { get; init; }

        public Ray GetRay(double s, double t)
        {
            Random random = new();
            Vec3 rd = this.LensRadius * Vec3.GetRandomInUnitDisk();
            Vec3 offset = (this.U * rd.X) + (this.V * rd.Y);
            return new Ray(
                origin: this.Origin + offset,
                direction: this.LowerLeftCorner + (s * this.Horizontal) + (t * this.Vertical) - this.Origin - offset,
                time: (random.NextDouble() * (this.ShutterClose - this.ShutterOpen)) + this.ShutterOpen);
        }
    }
}
