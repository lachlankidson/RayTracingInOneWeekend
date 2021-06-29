namespace RayTracing.Hittables
{
    using System;
    using System.Numerics;
    using RayTracing.Materials;

    public record ConstantMedium(Hittable Boundary, float Density, Material PhaseFunction) : Hittable
    {
        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            HitRecord rec1 = default;
            HitRecord rec2 = default;
            if (!this.Boundary.Hit(ray, float.NegativeInfinity, float.PositiveInfinity, ref rec1))
            {
                return false;
            }

            if (!this.Boundary.Hit(ray, rec1.T + 0.0001f, float.PositiveInfinity, ref rec2))
            {
                return false;
            }

            if (rec1.T < tMin)
            {
                rec1.T = tMin;
            }

            if (rec2.T < tMax)
            {
                rec2.T = tMax;
            }

            if (rec1.T >= rec2.T)
            {
                return false;
            }

            if (rec1.T < 0)
            {
                rec1.T = 0;
            }

            float rayLength = ray.Direction.Length();
            float distanceInsideBoundary = (rec2.T - rec1.T) * rayLength;
            float hitDistance = -1 / this.Density * (float)Math.Log(new Random().NextDouble());
            if (hitDistance > distanceInsideBoundary)
            {
                return false;
            }

            hitRecord.T = rec1.T + (hitDistance / rayLength);
            hitRecord.Point = ray.At(hitRecord.T);
            hitRecord.Normal = Vector3.UnitX;
            hitRecord.FrontFace = true;
            hitRecord.Material = this.PhaseFunction;
            return true;
        }

        public override AxisAlignedBoundingBox? BoundingBox(float time0, float time1)
            => this.Boundary.BoundingBox(time0, time1);
    }
}
