namespace RayTracing.Hittables
{
    using System;
    using RayTracing.Materials;

    public class Sphere : Hittable
    {
        public Sphere(Vec3 center, double radius, Material material)
        {
            this.Center = center;
            this.Radius = radius;
            this.Material = material;
        }

        public Vec3 Center { get; init; }

        public double Radius { get; init; }

        public Material Material { get; init; }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            Vec3 originCenter = ray.Origin - this.Center;
            double a = ray.Direction.LengthSquared();
            double halfB = Vec3.DotProduct(originCenter, ray.Direction);
            double c = originCenter.LengthSquared() - Math.Pow(this.Radius, 2);
            double discriminant = Math.Pow(halfB, 2) - (a * c);
            if (discriminant < 0)
            {
                return false;
            }

            double squareroot = Math.Sqrt(discriminant);
            double root = (-halfB - squareroot) / a;
            if (root < tMin || tMax < root)
            {
                root = (-halfB + squareroot) / a;
                if (root < tMin || tMax < root)
                {
                    return false;
                }
            }

            hitRecord.T = root;
            hitRecord.Point = ray.At(hitRecord.T);
            hitRecord.SetFaceNormal(ray, (hitRecord.Point - this.Center) / this.Radius);
            hitRecord.Material = this.Material;
            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox boundingBox)
        {
            Vec3 radiusVec = new(this.Radius, this.Radius, this.Radius);
            boundingBox = new AxisAlignedBoundingBox(this.Center - radiusVec, this.Center + radiusVec);
            return true;
        }
    }
}
