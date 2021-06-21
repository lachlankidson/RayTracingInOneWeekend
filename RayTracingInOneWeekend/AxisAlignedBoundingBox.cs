namespace RayTracing
{
    using System;

    public class AxisAlignedBoundingBox
    {
        public AxisAlignedBoundingBox(Vec3 minimum, Vec3 maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        public Vec3 Minimum { get; init; }

        public Vec3 Maximum { get; init; }

        public static AxisAlignedBoundingBox GetSurroundingBox(AxisAlignedBoundingBox a, AxisAlignedBoundingBox b)
        {
            static Vec3 GetVec(Func<double, double, double> reduce, Vec3 a, Vec3 b) =>
                new(reduce(a.X, b.X), reduce(a.Y, b.Y), reduce(a.Z, b.Z));

            return new AxisAlignedBoundingBox(
                minimum: GetVec(Math.Min, a.Minimum, b.Minimum),
                maximum: GetVec(Math.Max, a.Maximum, b.Maximum));
        }

        public bool Hit(Ray ray, double tMin, double tMax)
        {
            for (int a = 0; a < 3; a++)
            {
                double invD = 1 / ray.Direction[a];
                double origin = ray.Origin[a];
                double t0 = (this.Minimum[a] - origin) * invD;
                double t1 = (this.Maximum[a] - origin) * invD;
                if (invD < 0)
                {
                    (t0, t1) = (t1, t0);
                }

                tMin = t0 > tMin ? t0 : tMin;
                tMax = t1 < tMax ? t1 : tMax;
                if (tMax <= tMin)
                {
                    return false;
                }
            }

            return true;
        }
    }
}