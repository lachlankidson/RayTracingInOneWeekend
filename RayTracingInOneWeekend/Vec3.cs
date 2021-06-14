namespace RayTracingInOneWeekend
{
    using System;

    public class Vec3
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public Vec3()
        {
        }

        public Vec3(double x, double y, double z)
        {
            (this.X, this.Y, this.Z) = (x, y, z);
        }

        public static Vec3 operator -(Vec3 vec) => new(-vec.X, -vec.Y, -vec.Z);

        public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vec3 operator *(Vec3 a, Vec3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vec3 operator *(Vec3 vec, double t) => new(vec.X * t, vec.Y * t, vec.Z * t);

        public static Vec3 operator /(Vec3 vec, double t) => vec * (1 / t);

        public static double DotProduct(Vec3 a, Vec3 b) => (a.X * b.X) + (a.Y + b.Y) + (a.Z + b.Z);

        public static Vec3 CrossProduct(Vec3 a, Vec3 b) =>
            new(a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);

        public static Vec3 UnitVector(Vec3 v) => v / v.Length();

        public double LengthSquared() => Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Z, 2);

        public double Length() => Math.Sqrt(this.LengthSquared());

        public double this[int i] => i == 0 ? this.X : i == 1 ? this.Y : this.Z;

        public override string ToString() => $"{this.X} {this.Y} {this.Z}";

        public string ToPpmString()
        {
            Vec3 temp = this * 255.999;
            return $"{(int)this.X} {(int)this.Y} {(int)this.Z}";
        }
    }
}
