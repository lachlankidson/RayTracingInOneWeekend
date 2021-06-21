namespace RayTracing.Hittables
{
    using System;
    using System.Linq;

    public class RotateY : Hittable
    {
        public RotateY(Hittable hittable, double angle)
        {
            this.Hittable = hittable;
            this.Angle = angle;
            double radians = Math.PI / 180 * this.Angle;
            double sinTheta = Math.Sin(radians);
            double cosTheta = Math.Cos(radians);
            this.HasBox = this.Hittable.BoundingBox(0, 1, out AxisAlignedBoundingBox? boundingBox);
            if (!this.HasBox || boundingBox is null)
            {
                this.Box = null;
                return;
            }

            var min = Enumerable.Repeat(double.PositiveInfinity, 3).ToArray();
            var max = Enumerable.Repeat(double.NegativeInfinity, 3).ToArray();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        double x = (i * boundingBox.Maximum.X) + ((1 - i) * boundingBox.Minimum.X);
                        double y = (j * boundingBox.Maximum.Y) + ((1 - j) * boundingBox.Minimum.Y);
                        double z = (k * boundingBox.Maximum.Z) + ((1 - k) * boundingBox.Minimum.Z);

                        double newX = (cosTheta * x) + (sinTheta * z);
                        double newZ = (-sinTheta * x) + (cosTheta * z);

                        Vec3 tester = new(newX, y, newZ);
                        for (int c = 0; c < 3; c++)
                        {
                            min[c] = Math.Min(min[c], tester[c]);
                            max[c] = Math.Min(max[c], tester[c]);
                        }
                    }
                }
            }

            this.Box = new AxisAlignedBoundingBox(
                new Vec3(min[0], min[1], min[2]),
                new Vec3(max[0], max[1], max[2]));
        }

        public double Angle { get; init; }

        public bool HasBox { get; init; }

        public AxisAlignedBoundingBox? Box { get; init; }

        public Hittable Hittable { get; init; }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            double radians = Math.PI / 180 * this.Angle;
            double sinTheta = Math.Sin(radians);
            double cosTheta = Math.Cos(radians);

            Vec3 M1(Vec3 vec) => new(
                    x: (cosTheta * vec[0]) - (sinTheta * vec[2]),
                    y: vec[1],
                    z: (sinTheta * vec[0]) + (cosTheta * vec[2]));

            Vec3 origin = M1(ray.Origin);
            Vec3 direction = M1(ray.Direction);
            Ray rotatedRay = new(origin, direction, ray.Time);
            if (!this.Hittable.Hit(rotatedRay, tMin, tMax, ref hitRecord))
            {
                return false;
            }

            Vec3 M2(Vec3 vec) => new(
                (cosTheta * vec[0]) + (sinTheta * vec[2]),
                vec[1],
                (-sinTheta * vec[0]) + (cosTheta * vec[2]));

            Vec3 point = M2(hitRecord.Point);
            Vec3 normal = M2(hitRecord.Normal);

            hitRecord.Point = point;
            hitRecord.SetFaceNormal(rotatedRay, normal);
            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox? boundingBox)
        {
            boundingBox = this.Box;
            return this.HasBox;
        }
    }
}
