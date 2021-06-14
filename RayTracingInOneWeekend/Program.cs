namespace RayTracingInOneWeekend
{
    using System;

    public static class Program
    {
        public static Vec3 RayColor(Ray ray)
        {
            Vec3 zm1 = new (0, 0, -1);
            double t = Program.HitSphere(zm1, 0.5, ray);
            if (t > 0)
            {
                Vec3 n = Vec3.UnitVector(ray.At(t) - zm1);
                return 0.5 * new Vec3(n.X + 1, n.Y + 1, n.Z + 1);
            }

            Vec3 unitDirection = Vec3.UnitVector(ray.Direction);
            t = 0.5 * (unitDirection.Y + 1);
            return ((1 - t) * new Vec3(1, 1, 1)) + (t * new Vec3(0.5, 0.7, 1));
        }

        public static double HitSphere(Vec3 center, double radius, Ray ray)
        {
            Vec3 originCenter = ray.Origin - center;
            double a = Vec3.DotProduct(ray.Direction, ray.Direction);
            double b = 2.0 * Vec3.DotProduct(originCenter, ray.Direction);
            double c = Vec3.DotProduct(originCenter, originCenter) - Math.Pow(radius, 2);
            double discriminant = Math.Pow(b, 2) - (4 * a * c);
            if (discriminant < 0)
            {
                return -1.0;
            }
            else
            {
                return (-b - Math.Sqrt(discriminant)) / (2.0 * a);
            }
        }

        public static void Main()
        {
            // Image.
            const double aspectRatio = 16 / 9.0;
            const int imageWidth = 400;
            const int imageHeight = (int)(imageWidth / aspectRatio);

            // Camera.
            const double viewportHeight = 2;
            double viewportWidth = aspectRatio * viewportHeight;
            const double focalLength = 1;

            Vec3 origin = new (0, 0, 0);
            Vec3 horizontal = new (viewportWidth, 0, 0);
            Vec3 vertical = new (0, viewportHeight, 0);
            Vec3 lowerLeftCorner = origin - (horizontal / 2) - (vertical / 2) - new Vec3(0, 0, focalLength);

            // Render.
            Console.Write($"P3\n{imageWidth} {imageHeight}\n255\n");
            for (int i = imageHeight - 1; i >= 0; --i)
            {
                Console.Error.WriteLine($"Scanlines remaining: {i}");
                for (int j = 0; j < imageWidth;  ++j)
                {
                    double u = (double)j / (imageWidth - 1);
                    double v = (double)i / (imageHeight - 1);
                    Ray ray = new (origin, lowerLeftCorner + (u * horizontal) + (v * vertical) - origin);
                    Vec3 pixelColor = Program.RayColor(ray);
                    Console.WriteLine(pixelColor.ToPpmString());
                }
            }
        }
    }
}