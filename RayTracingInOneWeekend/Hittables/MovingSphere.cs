namespace RayTracing.Hittables
{
    using System;
    using System.Numerics;

    public class MovingSphere : Sphere
    {
        public MovingSphere(Vector3 startCenter, Vector3 endCenter, float radius, Materials.Material material, float startMoving, float stopMoving)
            : base(startCenter, radius, material)
        {
            this.StartMoving = startMoving;
            this.StopMoving = stopMoving;
            this.EndCenter = endCenter;
        }

        public float StartMoving { get; init; }

        public float StopMoving { get; init; }

        public Vector3 EndCenter { get; init; }

        public Vector3 GetCenterAt(float time) =>
            this.Center + (((time - this.StartMoving) / (this.StopMoving - this.StartMoving)) * (this.Center - this.EndCenter));

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            Vector3 originCenter = ray.Origin - this.GetCenterAt(ray.Time);
            float a = ray.Direction.LengthSquared();
            float halfB = Vector3.Dot(originCenter, ray.Direction);
            float c = originCenter.LengthSquared() - (float)Math.Pow(this.Radius, 2);
            float discriminant = (float)Math.Pow(halfB, 2) - (a * c);
            if (discriminant < 0)
            {
                return false;
            }

            float squareroot = (float)Math.Sqrt(discriminant);
            float root = (-halfB - squareroot) / a;
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
            Vector3 outwardNormal = (hitRecord.Point - this.GetCenterAt(ray.Time)) / this.Radius;
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = this.Material;
            return true;
        }

        public override bool BoundingBox(float startTime, float endTime, out AxisAlignedBoundingBox boundingBox)
        {
            AxisAlignedBoundingBox GetBox(float time)
            {
                Vector3 radiusVec = new(this.Radius);
                Vector3 center = this.GetCenterAt(time);
                return new(center - radiusVec, center + radiusVec);
            }

            boundingBox = AxisAlignedBoundingBox.GetSurroundingBox(GetBox(startTime), GetBox(endTime));
            return true;
        }
    }
}
