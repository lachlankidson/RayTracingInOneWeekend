namespace RayTracing.Hittables
{
    using System;
    using System.Linq;
    using System.Numerics;

    public class RotateY : Hittable
    {
        public RotateY(Hittable hittable, float angle)
        {
            this.Hittable = hittable;
            this.Angle = angle;
            float radians = (float)Math.PI / 180 * this.Angle;
            float sinTheta = (float)Math.Sin(radians);
            float cosTheta = (float)Math.Cos(radians);
            this.HasBox = this.Hittable.BoundingBox(0, 1, out AxisAlignedBoundingBox? boundingBox);
            if (!this.HasBox || boundingBox is null)
            {
                this.Box = null;
                return;
            }

            var min = Enumerable.Repeat(float.PositiveInfinity, 3).ToArray();
            var max = Enumerable.Repeat(float.NegativeInfinity, 3).ToArray();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        float x = (i * boundingBox.Maximum.X) + ((1 - i) * boundingBox.Minimum.X);
                        float y = (j * boundingBox.Maximum.Y) + ((1 - j) * boundingBox.Minimum.Y);
                        float z = (k * boundingBox.Maximum.Z) + ((1 - k) * boundingBox.Minimum.Z);

                        float newX = (cosTheta * x) + (sinTheta * z);
                        float newZ = (-sinTheta * x) + (cosTheta * z);

                        Vector3 tester = new(newX, y, newZ);
                        for (int c = 0; c < 3; c++)
                        {
                            min[c] = (float)Math.Min(min[c], tester.Get(c));
                            max[c] = (float)Math.Min(max[c], tester.Get(c));
                        }
                    }
                }
            }

            this.Box = new AxisAlignedBoundingBox(
                new Vector3(min[0], min[1], min[2]),
                new Vector3(max[0], max[1], max[2]));
        }

        public float Angle { get; init; }

        public bool HasBox { get; init; }

        public AxisAlignedBoundingBox? Box { get; init; }

        public Hittable Hittable { get; init; }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            float radians = (float)Math.PI / 180 * this.Angle;
            float sinTheta = (float)Math.Sin(radians);
            float cosTheta = (float)Math.Cos(radians);

            Vector3 M1(Vector3 vec) => new(
                    x: (cosTheta * vec.Get(0)) - (sinTheta * vec.Get(2)),
                    y: vec.Get(1),
                    z: (sinTheta * vec.Get(0)) + (cosTheta * vec.Get(2)));

            Vector3 origin = M1(ray.Origin);
            Vector3 direction = M1(ray.Direction);
            Ray rotatedRay = new(origin, direction, ray.Time);
            if (!this.Hittable.Hit(rotatedRay, tMin, tMax, ref hitRecord))
            {
                return false;
            }

            Vector3 M2(Vector3 vec) => new(
                (cosTheta * vec.Get(0)) + (sinTheta * vec.Get(2)),
                vec.Get(1),
                (-sinTheta * vec.Get(0)) + (cosTheta * vec.Get(2)));

            Vector3 point = M2(hitRecord.Point);
            Vector3 normal = M2(hitRecord.Normal);

            hitRecord.Point = point;
            hitRecord.SetFaceNormal(rotatedRay, normal);
            return true;
        }

        public override bool BoundingBox(float time0, float time1, out AxisAlignedBoundingBox? boundingBox)
        {
            boundingBox = this.Box;
            return this.HasBox;
        }
    }
}
