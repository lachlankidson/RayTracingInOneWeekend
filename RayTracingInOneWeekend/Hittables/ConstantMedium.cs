namespace RayTracing.Hittables
{
    using System;
    using RayTracing.Materials;
    using RayTracing.Textures;

    public class ConstantMedium : Hittable
    {
        public ConstantMedium(Hittable boundary, double density, Texture texture)
        {
            this.Boundary = boundary;
            this.Density = density;
            this.PhaseFunction = new Isotropic(texture);
        }

        public Hittable Boundary { get; init; }

        public double Density { get; init; }

        public Material PhaseFunction { get; init; }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            HitRecord rec1 = default;
            HitRecord rec2 = default;
            if (!this.Boundary.Hit(ray, double.NegativeInfinity, double.PositiveInfinity, ref rec1))
            {
                return false;
            }

            if (!this.Boundary.Hit(ray, rec1.T + 0.0001, double.PositiveInfinity, ref rec2))
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

            double rayLength = ray.Direction.Length();
            double distanceInsideBoundary = (rec2.T - rec1.T) * rayLength;
            double hitDistance = (-1 / this.Density) * Math.Log(new Random().NextDouble());
            if (hitDistance > distanceInsideBoundary)
            {
                return false;
            }

            hitRecord.T = rec1.T + (hitDistance / rayLength);
            hitRecord.Point = ray.At(hitRecord.T);
            hitRecord.Normal = new Vec3(1, 0, 0);
            hitRecord.FrontFace = true;
            hitRecord.Material = this.PhaseFunction;
            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox? boundingBox)
            => this.Boundary.BoundingBox(time0, time1, out boundingBox);
    }
}
