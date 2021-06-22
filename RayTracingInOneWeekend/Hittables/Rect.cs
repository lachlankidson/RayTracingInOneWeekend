namespace RayTracing.Hittables
{
    using System;
    using System.Numerics;
    using RayTracing.Materials;

    public class Rect : Hittable
    {
        public Rect(RectOrientation orientation, (float A, float B) a, (float A, float B) b, float k, Material material)
        {
            this.A = a;
            this.B = b;
            this.K = k;
            this.Material = material;
            this.Orientation = orientation;
        }

        public (float A, float B) A { get; init; }

        public (float A, float B) B { get; init; }

        public float K { get; init; }

        public Material Material { get; init; }

        public RectOrientation Orientation { get; init; }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            int index = Math.Abs((int)this.Orientation - 2);
            float t = (this.K - ray.Origin.Get(index)) / ray.Direction.Get(index);
            if (t < tMin || t > tMax)
            {
                return false;
            }

            (int ai, int bi, Vector3 outwardNormal) = this.Orientation switch
            {
                RectOrientation.XY => (0, 1, Vector3.UnitZ),
                RectOrientation.XZ => (0, 2, Vector3.UnitY),
                RectOrientation.YZ => (1, 2, Vector3.UnitX),
                _ => throw new NotImplementedException(),
            };

            float a = ray.Origin.Get(ai) + (t * ray.Direction.Get(ai));
            float b = ray.Origin.Get(bi) + (t * ray.Direction.Get(bi));
            if (a < this.A.A || a > this.A.B || b < this.B.A || b > this.B.B)
            {
                return false;
            }

            static float M(float a, (float A, float B) b) => (a - b.A) / (b.B - b.A);
            hitRecord.U = M(a, this.A);
            hitRecord.V = M(b, this.B);
            hitRecord.T = t;
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = this.Material;
            hitRecord.Point = ray.At(t);
            return true;
        }

        public override bool BoundingBox(float time0, float time1, out AxisAlignedBoundingBox boundingBox)
        {
            const float kAdjust = .0001f;
            boundingBox = this.Orientation switch
            {
                RectOrientation.XY => new(new Vector3(this.A.A, this.B.A, this.K - kAdjust), new Vector3(this.A.B, this.B.B, this.K + kAdjust)),
                RectOrientation.XZ => new(new Vector3(this.A.A, this.K - kAdjust, this.B.A), new Vector3(this.A.B, this.K + kAdjust, this.B.B)),
                RectOrientation.YZ => new(new Vector3(this.K - kAdjust, this.A.A, this.B.A), new Vector3(this.K + kAdjust, this.A.B, this.B.B)),
                _ => throw new NotImplementedException(),
            };

            return true;
        }
    }
}
