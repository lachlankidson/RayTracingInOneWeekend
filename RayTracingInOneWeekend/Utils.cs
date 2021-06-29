namespace RayTracing
{
    using System;
    using System.Numerics;

    public static class Utils
    {
        private static float RandRange(float min, float max) => ((float)new Random().NextDouble() * (max - min)) + min;

        public static Vector3 GetRandomVec3(float min = 0, float max = 1)
            => new(Utils.RandRange(min, max), Utils.RandRange(min, max), Utils.RandRange(min, max));

        public static Vector3 UnitVector(this Vector3 vec) => vec / vec.Length();

        public static Vector3 GetRandomUnitVector() => Utils.GetRandomVec3InUnitSphere().UnitVector();

        public static Vector3 GetRandomVec3InUnitSphere()
        {
            while (true)
            {
                Vector3 vec = Utils.GetRandomVec3(-1, 1);
                if (vec.LengthSquared() >= 1)
                {
                    continue;
                }

                return vec;
            }
        }

        public static Vector3 GetRandomVec3InUnitDisk()
        {
            while (true)
            {
                Vector3 p = new(Utils.RandRange(1, -1), Utils.RandRange(1, -1), 0);
                if (p.LengthSquared() >= 1)
                {
                    continue;
                }

                return p;
            }
        }

        public static bool NearZero(this Vector3 vec) =>
            Math.Abs(vec.X) < 1e-8
            && Math.Abs(vec.Y) < 1e-8
            && Math.Abs(vec.Z) < 1e-8;

        public static float Get(this Vector3 vec, int index) =>
            index == 0
                ? vec.X
                : index == 1
                    ? vec.Y
                    : vec.Z;

        public static string GetPpmString(this Vector3 pixelColor, int samplesPerPixel)
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

        public static Vector3 Refract(Vector3 unitVector, Vector3 normal, float ratio)
        {
            float cosTheta = Math.Min(Vector3.Dot(-unitVector, normal), 1);
            Vector3 perpendicularRay = ratio * (unitVector + (cosTheta * normal));
            Vector3 parallelRay = (float)-Math.Sqrt(Math.Abs(1 - perpendicularRay.LengthSquared())) * normal;
            return perpendicularRay + parallelRay;
        }
    }
}