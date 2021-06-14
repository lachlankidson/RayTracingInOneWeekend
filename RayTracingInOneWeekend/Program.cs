namespace RayTracingInOneWeekend
{
    using System;

    public static class Program
    {
        public static Vec3 RayColor(Ray ray, Hittable world)
        {
            HitRecord hitRecord = new ();
            hitRecord.Normal = new Vec3(1, 2, 3);
            if (world.Hit(ray, 0, double.PositiveInfinity, ref hitRecord))
            {
                return 0.5 * (hitRecord.Normal + new Vec3(1, 1, 1));
            }

            Vec3 unitDirection = Vec3.UnitVector(ray.Direction);
            double t = 0.5 * (unitDirection.Y + 1);
            return ((1 - t) * new Vec3(1, 1, 1)) + (t * new Vec3(0.5, 0.7, 1));
        }

        public static void Main()
        {
            // Image.
            const double aspectRatio = 16 / 9.0;
            const int imageWidth = 400;
            const int imageHeight = (int)(imageWidth / aspectRatio);
            const int samplesPerPixel = 100;

            // World.
            HittableList world = new ();
            world.Add(new Sphere(new Vec3(0, 0, -1), 0.5));
            world.Add(new Sphere(new Vec3(0, -100.5, -1), 100));

            // Camera.
            Camera camera = new ();

            // Render.
            Random random = new ();
            Console.Write($"P3\n{imageWidth} {imageHeight}\n255\n");
            for (int i = imageHeight - 1; i >= 0; --i)
            {
                Console.Error.WriteLine($"Scanlines remaining: {i}");
                for (int j = 0; j < imageWidth;  ++j)
                {
                    Vec3 pixelColor = new (0, 0, 0);
                    for (int k = 0; k < samplesPerPixel; ++k)
                    {
                        double u = (j + random.NextDouble()) / (imageWidth - 1);
                        double v = (i + random.NextDouble()) / (imageHeight - 1);
                        Ray ray = camera.GetRay(u, v);
                        pixelColor += Program.RayColor(ray, world);
                    }

                    Console.WriteLine(Vec3.GetPpmString(pixelColor, samplesPerPixel));
                }
            }

            Console.Error.WriteLine($"{Environment.NewLine} Done.");
        }
    }
}