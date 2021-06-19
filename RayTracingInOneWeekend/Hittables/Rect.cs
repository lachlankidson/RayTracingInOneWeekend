namespace RayTracing.Hittables
{
    using System;
    using RayTracing.Materials;

    public class Rect : Hittable
    {
        public Rect(RectOrientation orientation, (double A, double B) a, (double A, double B) b, double k, Material material)
        {
            this.A = a;
            this.B = b;
            this.K = k;
            this.Material = material;
            this.Orientation = orientation;
        }

        public (double A, double B) A { get; init; }

        public (double A, double B) B { get; init; }

        public double K { get; init; }

        public Material Material { get; init; }

        public RectOrientation Orientation { get; init; }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            int index = Math.Abs((int)this.Orientation - 2);
            double t = (this.K - ray.Origin[index]) / ray.Direction[index];
            if (t < tMin || t > tMax)
            {
                return false;
            }

            (int ai, int bi, Vec3 outwardNormal) = this.Orientation switch
            {
                RectOrientation.XY => (0, 1, new Vec3(0, 0, 1)),
                RectOrientation.XZ => (0, 2, new Vec3(0, 1, 0)),
                RectOrientation.YZ => (1, 2, new Vec3(1, 0, 0)),
                _ => throw new NotImplementedException(),
            };

            double a = ray.Origin[ai] + (t * ray.Direction[ai]);
            double b = ray.Origin[bi] + (t * ray.Direction[bi]);
            if (a < this.A.A || a > this.A.B || b < this.B.A || b > this.B.B)
            {
                return false;
            }

            static double M(double a, (double A, double B) b) => (a - b.A) / (b.B - b.A);
            hitRecord.U = M(a, this.A);
            hitRecord.V = M(b, this.B);
            hitRecord.T = t;
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = this.Material;
            hitRecord.Point = ray.At(t);
            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox boundingBox)
        {
            const double kAdjust = 0.0001;
            boundingBox = this.Orientation switch
            {
                RectOrientation.XY => new(new Vec3(this.A.A, this.B.A, this.K - kAdjust), new Vec3(this.A.B, this.B.B, this.K + kAdjust)),
                RectOrientation.XZ => new(new Vec3(this.A.A, this.K - kAdjust, this.B.A), new Vec3(this.A.B, this.K + kAdjust, this.B.B)),
                RectOrientation.YZ => new(new Vec3(this.K - kAdjust, this.A.A, this.B.A), new Vec3(this.K + kAdjust, this.A.B, this.B.B)),
                _ => throw new NotImplementedException(),
            };

            return true;
        }
    }
}
