﻿namespace RayTracing
{
    using System;
    using System.Numerics;

    public class AxisAlignedBoundingBox
    {
        public AxisAlignedBoundingBox(Vector3 minimum, Vector3 maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        public Vector3 Minimum { get; init; }

        public Vector3 Maximum { get; init; }

        public static AxisAlignedBoundingBox GetSurroundingBox(AxisAlignedBoundingBox a, AxisAlignedBoundingBox b)
        {
            static Vector3 GetVec(Func<float, float, float> reduce, Vector3 a, Vector3 b) =>
                new(reduce(a.X, b.X), reduce(a.Y, b.Y), reduce(a.Z, b.Z));

            return new AxisAlignedBoundingBox(
                minimum: GetVec(Math.Min, a.Minimum, b.Minimum),
                maximum: GetVec(Math.Max, a.Maximum, b.Maximum));
        }

        public bool Hit(Ray ray, double tMin, double tMax)
        {
            for (int a = 0; a < 3; a++)
            {
                double invD = 1 / ray.Direction.Get(a);
                double origin = ray.Origin.Get(a);
                double t0 = (this.Minimum.Get(a) - origin) * invD;
                double t1 = (this.Maximum.Get(a) - origin) * invD;
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