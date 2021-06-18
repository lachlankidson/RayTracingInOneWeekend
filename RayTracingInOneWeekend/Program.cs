namespace RayTracing
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using RayTracing.Hittables;
    using RayTracing.Materials;
    using RayTracing.Textures;

    public static class Program
    {
        public static Vec3 RayColor(Ray ray, Hittable world, uint depth)
        {
            if (depth == 0)
            {
                return new Vec3(0);
            }

            HitRecord hitRecord = default;
            if (world.Hit(ray, 0.001, double.PositiveInfinity, ref hitRecord))
            {
                if (hitRecord.Material.Scatter(ray, hitRecord, out Vec3 attenuation, out Ray scatteredRay))
                {
                    return attenuation * Program.RayColor(scatteredRay, world, depth - 1);
                }

                return new Vec3(0);
            }

            Vec3 unitDirection = Vec3.UnitVector(ray.Direction);
            double t = 0.5 * (unitDirection.Y + 1);
            return ((1 - t) * new Vec3(1)) + (t * new Vec3(0.5, 0.7, 1));
        }

        public static HittableList GetRandomScene()
        {
            Random random = new();
            HittableList world = new();
            CheckerTexture checker = new(new SolidColor(.2, .3, .1), new SolidColor(.9, .9, .9));
            world.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(checker)));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double chooseMat = random.NextDouble();
                    Vec3 center = new(a + (.9 * random.NextDouble()), .2, b + (.9 * random.NextDouble()));
                    if ((center - new Vec3(4, 0.2, 0)).Length() > .9)
                    {
                        Material sphereMaterial;
                        if (chooseMat < .8)
                        {
                            // Diffuse.
                            Vec3 albedo = Vec3.GetRandom() * Vec3.GetRandom();
                            sphereMaterial = new Lambertian(albedo);
                            Vec3 center2 = center + new Vec3(0, random.NextDouble() / 2, 0);
                            world.Add(new MovingSphere(
                                startCenter: center,
                                endCenter: center2,
                                radius: .2,
                                material: sphereMaterial,
                                startMoving: 0,
                                stopMoving: 1));
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

        public static HittableList TwoSpheres()
        {
            HittableList hittables = new();
            CheckerTexture checker = new(new SolidColor(.2, .3, .1), new SolidColor(.9, .9, .9));
            Material sphereMat = new Lambertian(checker);
            hittables.Add(new Sphere(new Vec3(0, -10, 0), 10, sphereMat));
            hittables.Add(new Sphere(new Vec3(0, 10, 0), 10, sphereMat));
            return hittables;
        }

        public static HittableList TwoPerlinSpheres()
        {
            HittableList hittables = new();
            Texture noise = new NoiseTexture(new Perlin(), 4);
            Material sphereMat = new Lambertian(noise);
            hittables.Add(new Sphere(new Vec3(0, -1000, 0), 1000, sphereMat));
            hittables.Add(new Sphere(new Vec3(0, 2, 0), 2, sphereMat));
            return hittables;
        }

        public static void Main()
        {
            // Image.
            const double aspectRatio = 16 / 9.0;
            const int imageWidth = 400;
            const int imageHeight = (int)(imageWidth / aspectRatio);
            const int samplesPerPixel = 100;
            const int maxDepth = 50;

            // World.
            HittableList world = Program.GetRandomScene();

            Vec3 lookFrom;
            Vec3 lookAt;
            Vec3 viewUp = new(0, 1, 0);
            const double distanceToFocus = 10;
            double verticalFov = 40.0;
            double aperture = .1;

            switch (0)
            {
                case 1:
                    world = Program.GetRandomScene();
                    lookFrom = new Vec3(13, 2, 3);
                    lookAt = new Vec3(0, 0, 0);
                    verticalFov = 20;
                    aperture = .1;
                    break;
                case 2:
                    world = Program.TwoSpheres();
                    lookFrom = new Vec3(13, 2, 3);
                    lookAt = new Vec3(0, 0, 0);
                    verticalFov = 20.0;
                    break;
                default:
                    world = Program.TwoPerlinSpheres();
                    lookFrom = new Vec3(13, 2, 3);
                    lookAt = new Vec3(0, 0, 0);
                    verticalFov = 20.0;
                    break;
            }

            // Camera.
            Camera camera = new(
                lookFrom,
                lookAt,
                viewUp,
                verticalFov,
                aspectRatio,
                aperture,
                distanceToFocus,
                shutterOpen: 0,
                shutterClose: 1);

            // Render.
            Random random = new();
            Console.Write($"P3\n{imageWidth} {imageHeight}\n255\n");
            for (int i = imageHeight - 1; i >= 0; --i)
            {
                Console.Error.WriteLine($"Scanlines remaining: {i}");
                ConcurrentBag<(int, string)> ppms = new();
                Parallel.For(0, imageWidth, (j) =>
                {
                    ConcurrentBag<(int, Vec3)> colors = new();
                    Vec3 pixelColor = new(0);
                    Parallel.For(0, samplesPerPixel - 1, (k) =>
                    {
                        double u = (j + random.NextDouble()) / (imageWidth - 1);
                        double v = (i + random.NextDouble()) / (imageHeight - 1);
                        Ray ray = camera.GetRay(u, v);
                        colors.Add((k, Program.RayColor(ray, world, maxDepth)));
                    });

                    foreach ((int, Vec3) pair in colors.OrderBy(x => x.Item1))
                    {
                        pixelColor += pair.Item2;
                    }

                    ppms.Add((j, Vec3.GetPpmString(pixelColor, samplesPerPixel)));
                });

                foreach ((int, string) pair in ppms.OrderBy(x => x.Item1))
                {
                    Console.WriteLine(pair.Item2);
                }
            }

            Console.Error.WriteLine($"{Environment.NewLine} Done.");
        }
    }
}