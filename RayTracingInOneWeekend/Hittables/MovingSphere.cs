namespace RayTracing.Hittables
{
    using System;
    using System.Numerics;
    using RayTracing.Materials;

    public record MovingSphere(Vector3 StartCenter, Vector3 EndCenter, float Radius, Material Material, float StartMoving, float StopMoving)
        : Sphere(Center: StartCenter, Radius, Material)
    {
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
