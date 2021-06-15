namespace RayTracing
{
    using System;
    using RayTracing.Materials;

    public static class Program
    {
        public static Vec3 RayColor(Ray ray, Hittable world, uint depth)
        {
            if (depth == 0)
            {
                return new Vec3(0, 0, 0);
            }

            HitRecord hitRecord = new ();
            if (world.Hit(ray, 0.001, double.PositiveInfinity, ref hitRecord))
            {
                if (hitRecord.Material.Scatter(ray, hitRecord, out Vec3 attenuation, out Ray scatteredRay))
                {
                    return attenuation * Program.RayColor(scatteredRay, world, depth - 1);
                }

                return new Vec3(0, 0, 0);
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
            const int samplesPerPixel = 50;
            const int maxDepth = 50;

            // World.
            HittableList world = new ();
            Material ground = new Lambertian(new Vec3(.8, .9, 0));
            Material center = new Lambertian(new Vec3(.1, .2, .5));
            Material left = new Dielectric(1.5);
            Material right = new Metal(new Vec3(.8, .6, .2), 0);

            world.Add(new Sphere(new Vec3(0, -100.5, -1), 100, ground));
            world.Add(new Sphere(new Vec3(0, 0, -1), .5, center));
            world.Add(new Sphere(new Vec3(-1, 0, -1), .5, left));
            world.Add(new Sphere(new Vec3(-1, 0, -1), -0.45, left));
            world.Add(new Sphere(new Vec3(1, 0, -1), .5, right));

            // Camera.
            Camera camera = new (
                lookFrom: new Vec3(-2, 2, 1),
                lookAt: new Vec3(0, 0, -1),
                viewUp: new Vec3(0, 1, 0),
                verticalFov: 20,
                aspectRatio);

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
                        pixelColor += Program.RayColor(ray, world, maxDepth);
                    }

                    Console.WriteLine(Vec3.GetPpmString(pixelColor, samplesPerPixel));
                }
            }

            Console.Error.WriteLine($"{Environment.NewLine} Done.");
        }
    }
}