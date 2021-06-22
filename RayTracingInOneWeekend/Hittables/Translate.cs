﻿namespace RayTracing.Hittables
{
    using System.Numerics;

    public class Translate : Hittable
    {
        public Translate(Hittable hittable, Vector3 displacement)
        {
            this.Hittable = hittable;
            this.Displacement = displacement;
        }

        public Hittable Hittable { get; set; }

        public Vector3 Displacement { get; set; }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            Ray movedRay = new(ray.Origin - this.Displacement, ray.Direction, ray.Time);
            if (!this.Hittable.Hit(movedRay, tMin, tMax, ref hitRecord))
            {
                return false;
            }

            hitRecord.Point += this.Displacement;
            hitRecord.SetFaceNormal(movedRay, hitRecord.Normal);
            return true;
        }

        public override bool BoundingBox(float time0, float time1, out AxisAlignedBoundingBox? boundingBox)
        {
            if (!this.Hittable.BoundingBox(time0, time1, out boundingBox) || boundingBox is null)
            {
                return false;
            }

            boundingBox = new AxisAlignedBoundingBox(
                boundingBox.Minimum + this.Displacement,
                boundingBox.Maximum + this.Displacement);
            return true;
        }
    }
}
