namespace RayTracing
{
    using System;
    using System.Numerics;

    public record Camera(
        Vector3 Origin,
        Vector3 LookAt,
        float VerticalFov,
        float AspectRatio,
        float Aperture,
        float FocusDistance,
        float ShutterOpen,
        float ShutterClose)
    {
        public Vector3 W => (this.Origin - this.LookAt).UnitVector();

        public Vector3 U => Vector3.Cross(Vector3.UnitY, this.W).UnitVector();

        public Vector3 V => Vector3.Cross(this.W, this.U);

        public Ray GetRay(float s, float t)
        {
            float viewportHeight = 2 * (float)Math.Tan(Math.PI / 180 * this.VerticalFov / 2);
            Vector3 horizontal = this.FocusDistance * this.AspectRatio * viewportHeight * this.U;
            Vector3 vertical = this.FocusDistance * viewportHeight * this.V;
            Vector3 rd = this.Aperture / 2 * Utils.GetRandomVec3InUnitDisk();
            Vector3 lowerLeftCorner = this.Origin - (horizontal / 2) - (vertical / 2) - (this.FocusDistance * this.W);
            Vector3 offset = (this.U * rd.X) + (this.V * rd.Y);
            return new Ray(
                Origin: this.Origin + offset,
                Direction: lowerLeftCorner + (s * horizontal) + (t * vertical) - this.Origin - offset,
                Time: ((float)new Random().NextDouble() * (this.ShutterClose - this.ShutterOpen)) + this.ShutterOpen);
        }
    }
}