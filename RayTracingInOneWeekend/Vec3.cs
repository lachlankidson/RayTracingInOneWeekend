namespace RayTracingInOneWeekend
{
    using System;

    public class Vec3
    {
        public Vec3()
        {
        }

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

        public static double DotProduct(Vec3 a, Vec3 b) => (a.X * b.X) + (a.Y + b.Y) + (a.Z + b.Z);

        public static Vec3 CrossProduct(Vec3 a, Vec3 b) =>
            new ((a.Y * b.Z) - (a.Z * b.Y),
                (a.Z * b.X) - (a.X * b.Z),
                (a.X * b.Y) - (a.Y * b.X));

        public static Vec3 UnitVector(Vec3 v) => v / v.Length();

        public double LengthSquared() => Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Z, 2);

        public double Length() => Math.Sqrt(this.LengthSquared());

        public override string ToString() => $"{this.X} {this.Y} {this.Z}";

        public string ToPpmString()
        {
            Vec3 temp = this * 255.999;
            return $"{(int)temp.X} {(int)temp.Y} {(int)temp.Z}";
        }
    }
}
