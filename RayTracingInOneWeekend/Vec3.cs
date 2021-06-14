namespace RayTracingInOneWeekend
{
    using System;

    public class Vec3
    {
        public Vec3(double x, double y, double z)
        {
            (this.X, this.Y, this.Z) = (x, y, z);
        }

        public double X { get; init; }

        public double Y { get; init; }

        public double Z { get; init; }

        public double this[int i] => i == 0 ? this.X : i == 1 ? this.Y : this.Z;

        public static Vec3 operator -(Vec3 vec) => new (-vec.X, -vec.Y, -vec.Z);

        public static Vec3 operator -(Vec3 a, Vec3 b) => new (a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vec3 operator +(Vec3 a, Vec3 b) => new (a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vec3 operator *(Vec3 a, Vec3 b) => new (a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vec3 operator *(Vec3 vec, double t) => new (vec.X * t, vec.Y * t, vec.Z * t);

        public static Vec3 operator *(double t, Vec3 vec) => vec * t;

        public static Vec3 operator /(Vec3 vec, double t) => vec * (1 / t);

        public static double DotProduct(Vec3 a, Vec3 b) => (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);

        public static Vec3 CrossProduct(Vec3 a, Vec3 b) =>
            new ((a.Y * b.Z) - (a.Z * b.Y),
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
            Random random = new ();
            return new Vec3(random.NextDouble(), random.NextDouble(), random.NextDouble());
        }

        public static Vec3 GetRandom(double min, double max)
        {
            Random random = new ();
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

        public static Vec3 RandomUnitVector() => Vec3.UnitVector(Vec3.GetRandomInUnitSphere());

        public double LengthSquared() => Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Z, 2);

        public double Length() => Math.Sqrt(this.LengthSquared());

        public override string ToString() => $"{this.X} {this.Y} {this.Z}";
    }
}
