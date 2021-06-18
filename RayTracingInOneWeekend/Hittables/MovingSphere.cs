namespace RayTracing.Hittables
{
    using System;

    public class MovingSphere : Sphere
    {
        public MovingSphere(Vec3 startCenter, Vec3 endCenter, double radius, Materials.Material material, double startMoving, double stopMoving)
            : base(startCenter, radius, material)
        {
            this.StartMoving = startMoving;
            this.StopMoving = stopMoving;
            this.EndCenter = endCenter;
        }

        public double StartMoving { get; init; }

        public double StopMoving { get; init; }

        public Vec3 EndCenter { get; init; }

        public Vec3 GetCenterAt(double time) =>
            this.Center + (((time - this.StartMoving) / (this.StopMoving - this.StartMoving)) * (this.Center - this.EndCenter));

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            Vec3 originCenter = ray.Origin - this.GetCenterAt(ray.Time);
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
            Vec3 outwardNormal = (hitRecord.Point - this.GetCenterAt(ray.Time)) / this.Radius;
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = this.Material;
            return true;
        }

        public override bool BoundingBox(double startTime, double endTime, out AxisAlignedBoundingBox boundingBox)
        {
            AxisAlignedBoundingBox GetBox(double time)
            {
                Vec3 radiusVec = new(this.Radius);
                Vec3 center = this.GetCenterAt(time);
                return new(center - radiusVec, center + radiusVec);
            }

            boundingBox = AxisAlignedBoundingBox.GetSurroundingBox(GetBox(startTime), GetBox(endTime));
            return true;
        }
    }
}
