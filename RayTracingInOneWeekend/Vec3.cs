namespace RayTracing
{
    using System;
    using NetFabric.Hyperlinq;

    public class Vec3
    {
        private readonly double[] e = new double[3];

        public Vec3(double x, double y, double z)
        {
            (this.e[0], this.e[1], this.e[2]) = (x, y, z);
        }

        public Vec3(double a)
            : this(a, a, a)
        {
        }

        public double X => this.e[0];

        public double Y => this.e[1];

        public double Z => this.e[2];

        public double this[int i] => this.e[i];

        public static Vec3 operator -(Vec3 vec) => new(-vec.X, -vec.Y, -vec.Z);

        public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vec3 operator *(Vec3 a, Vec3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vec3 operator *(Vec3 vec, double t) => new(vec.X * t, vec.Y * t, vec.Z * t);

        public static Vec3 operator *(double t, Vec3 vec) => vec * t;

        public static Vec3 operator /(Vec3 vec, double t) => vec * (1 / t);

        public static double DotProduct(Vec3 a, Vec3 b) => (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);

        public static Vec3 CrossProduct(Vec3 a, Vec3 b) =>
            new((a.Y * b.Z) - (a.Z * b.Y),
                (a.Z * b.X) - (a.X * b.Z),
                (a.X * b.Y) - (a.Y * b.X));

        public static Vec3 UnitVector(Vec3 v) => v / v.Length();

        public static string GetPpmString(Vec3 pixelColor, int samplesPerPixel)
        {
            double scale = 1.0 / samplesPerPixel;
            double rs = Math.Sqrt(scale * pixelColor.X);
            double gs = Math.Sqrt(scale * pixelColor.Y);
            double bs = Math.Sqrt(scale * pixelColor.Z);
            int r = (int)(256 * Math.Clamp(rs, 0, 0.999));
            int g = (int)(256 * Math.Clamp(gs, 0, 0.999));
            int b = (int)(256 * Math.Clamp(bs, 0, 0.999));
            return $"{r} {g} {b}";
        }

        public static Vec3 GetRandom()
        {
            Random random = new();
            return new Vec3(random.NextDouble(), random.NextDouble(), random.NextDouble());
        }

        public static Vec3 GetRandom(double min, double max)
        {
            Random random = new();
            double dif = max - min;
            return new Vec3(
                (random.NextDouble() * dif) + min,
                (random.NextDouble() * dif) + min,
                (random.NextDouble() * dif) + min);
        }

        public static Vec3 GetRandomInUnitSphere()
        {
            while (true)
            {
                Vec3 vec = Vec3.GetRandom(-1, 1);
                if (vec.LengthSquared() >= 1)
                {
                    continue;
                }

                return vec;
            }
        }

        public static Vec3 GetRandomUnitVector() => Vec3.UnitVector(Vec3.GetRandomInUnitSphere());

        public static Vec3 GetRandomInHemisphere(Vec3 normal)
        {
            Vec3 inUnit = Vec3.GetRandomInUnitSphere();
            return Vec3.DotProduct(inUnit, normal) > 0 ? inUnit : -inUnit;
        }

        public static Vec3 GetRandomInUnitDisk()
        {
            Random random = new();
            while (true)
            {
                Vec3 p = new(
                    (random.NextDouble() * 2) - 1,
                    (random.NextDouble() * 2) - 1,
                    0);
                if (p.LengthSquared() >= 1)
                {
                    continue;
                }

                return p;
            }
        }

        public static Vec3 Reflect(Vec3 v, Vec3 n) =>
            v - (2 * Vec3.DotProduct(v, n) * n);

        public static Vec3 Refract(Vec3 unitVector, Vec3 normal, double ratio)
        {
            double cosTheta = Math.Min(Vec3.DotProduct(-unitVector, normal), 1);
            Vec3 perpendicularRay = ratio * (unitVector + (cosTheta * normal));
            Vec3 parallelRay = -Math.Sqrt(Math.Abs(1 - perpendicularRay.LengthSquared())) * normal;
            return perpendicularRay + parallelRay;
        }

        public double LengthSquared() =>
            this.e.AsValueEnumerable().Select(x => Math.Pow(x, 2)).Sum();

        public double Length() => Math.Sqrt(this.LengthSquared());

        public bool NearZero() => this.e.AsValueEnumerable().All(x => Math.Abs(x) < 1e-8);

        public override string ToString() => $"{this.X} {this.Y} {this.Z}";
    }
}
