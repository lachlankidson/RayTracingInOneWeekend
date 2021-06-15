namespace RayTracing
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using RayTracing.Materials;
    using NetFabric.Hyperlinq;
    using System.Linq;

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

        public static HittableList GetScene()
        {
            Random random = new ();
            HittableList world = new ();
            Material ground = new Lambertian(new Vec3(.5, .5, .5));
            world.Add(new Sphere(new Vec3(0, -1000, 0), 1000, ground));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double chooseMat = random.NextDouble();
                    Vec3 center = new (a + (.9 * random.NextDouble()), .2, b + (.9 * random.NextDouble()));
                    if ((center - new Vec3(4, 0.2, 0)).Length() > .9)
                    {
                        Material sphereMaterial;
                        if (chooseMat < .8)
                        {
                            // Diffuse.
                            Vec3 albedo = Vec3.GetRandom() * Vec3.GetRandom();
                            sphereMaterial = new Lambertian(albedo);
                            world.Add(new Sphere(center, .2, sphereMaterial));
                        }
                        else if (chooseMat < .95)
                        {
                            // Metal.
                            Vec3 albedo = Vec3.GetRandom(.5, 1);
                            double fuzz = random.NextDouble() / 2;
                            sphereMaterial = new Metal(albedo, fuzz);
                            world.Add(new Sphere(center, .2, sphereMaterial));
                        }
                        else
                        {
                            // Glass.
                            sphereMaterial = new Dielectric(1.5);
                            world.Add(new Sphere(center, .2, sphereMaterial));
                        }
                    }
                }
            }

            world.Add(new Sphere(new Vec3(0, 1, 0), 1, new Dielectric(1.5)));
            world.Add(new Sphere(new Vec3(-4, 1, 0), 1, new Lambertian(new Vec3(.4, .2, .1))));
            world.Add(new Sphere(new Vec3(4, 1, 0), 1, new Metal(new Vec3(.7, .6, .5), 0)));
            return world;

        }

        public static void Main()
        {
            // Image.
            const double aspectRatio = 3 / 2.0;
            const int imageWidth = 500;
            const int imageHeight = (int)(imageWidth / aspectRatio);
            const int samplesPerPixel = 20;
            const int maxDepth = 50;

            // World.
            HittableList world = Program.GetScene();

            Vec3 lookFrom = new (13, 2, 3);
            Vec3 lookAt = new (0, 0, 0);
            Vec3 viewUp = new (0, 1, 0);
            double distanceToFocus = 10;
            double aperature = .1;

            // Camera.
            Camera camera = new (
                lookFrom,
                lookAt,
                viewUp,
                verticalFov: 20,
                aspectRatio,
                aperature,
                distanceToFocus);

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