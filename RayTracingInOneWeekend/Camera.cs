namespace RayTracing
{
    using System;
    using System.Numerics;

    public class Camera
    {
        public Camera(
            Vector3 lookFrom,
            Vector3 lookAt,
            Vector3 viewUp,
            float verticalFov,
            float aspectRatio,
            float aperture,
            float focusDistance,
            float shutterOpen,
            float shutterClose)
        {
            float theta = ((float)Math.PI / 180) * verticalFov;
            float h = (float)Math.Tan(theta / 2);
            float viewportHeight = 2 * h;
            float viewportWidth = aspectRatio * viewportHeight;

            this.W = (lookFrom - lookAt).UnitVector();
            this.U = Vector3.Cross(viewUp, this.W).UnitVector();
            this.V = Vector3.Cross(this.W, this.U);

            this.Origin = lookFrom;
            this.Horizontal = focusDistance * viewportWidth * this.U;
            this.Vertical = focusDistance * viewportHeight * this.V;
            this.LowerLeftCorner = this.Origin - (this.Horizontal / 2) - (this.Vertical / 2) - (focusDistance * this.W);
            this.LensRadius = aperture / 2;
            this.ShutterOpen = shutterOpen;
            this.ShutterClose = shutterClose;
        }

        public Vector3 Origin { get; init; }

        public Vector3 LowerLeftCorner { get; init; }

        public Vector3 Horizontal { get; init; }

        public Vector3 Vertical { get; init; }

        public Vector3 W { get; init; }

        public Vector3 U { get; init; }

        public Vector3 V { get; init; }

        public float LensRadius { get; init; }

        public float ShutterOpen { get; init; }

        public float ShutterClose { get; init; }

        public Ray GetRay(float s, float t)
        {
            Random random = new();
            Vector3 rd = this.LensRadius * Utils.GetRandomVec3InUnitDisk();
            Vector3 offset = (this.U * rd.X) + (this.V * rd.Y);
            return new Ray(
                Origin: this.Origin + offset,
                Direction: this.LowerLeftCorner + (s * this.Horizontal) + (t * this.Vertical) - this.Origin - offset,
                Time: ((float)random.NextDouble() * (this.ShutterClose - this.ShutterOpen)) + this.ShutterOpen);
        }
    }
}
