﻿namespace RayTracing
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
        public static Vec3 RayColor(Ray ray, Vec3 background, Hittable world, uint depth)
        {
            if (depth == 0)
            {
                return new Vec3(0);
            }

            HitRecord hitRecord = default;
            if (!world.Hit(ray, 0.001, double.PositiveInfinity, ref hitRecord))
            {
                return background;
            }

            Vec3 emitted = hitRecord.Material.Emitted(hitRecord.U, hitRecord.V, hitRecord.Point);
            if (!hitRecord.Material.Scatter(ray, hitRecord, out Vec3 attenuation, out Ray scatteredRay))
            {
                return emitted;
            }

            return emitted + (attenuation * Program.RayColor(scatteredRay, background, world, depth - 1));
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
            CheckerTexture checker = new(new SolidColor(.2, .3, .1), new SolidColor(.9, .9, .9));
            Material sphereMat = new Lambertian(checker);
            return new()
            {
                new Sphere(new Vec3(0, -10, 0), 10, sphereMat),
                new Sphere(new Vec3(0, 10, 0), 10, sphereMat),
            };
        }

        public static HittableList TwoPerlinSpheres()
        {
            Texture noise = new NoiseTexture(new Perlin(), 4);
            Material sphereMat = new Lambertian(noise);
            return new()
            {
                new Sphere(new Vec3(0, -1000, 0), 1000, sphereMat),
                new Sphere(new Vec3(0, 2, 0), 2, sphereMat),
            };
        }

        public static HittableList Earth()
        {
            Texture earthTexture = new ImageTexture("Images/earthmap.jpg");
            Material earthMaterial = new Lambertian(earthTexture);
            Sphere globe = new(new Vec3(0), 2, earthMaterial);
            return new HittableList(globe);
        }

        public static HittableList SimpleLight()
        {
            Texture pertext = new NoiseTexture(Noise: new Perlin(), Scale: 4);
            Material sphereMat = new Lambertian(pertext);
            Material diffuseLight = new DiffuseLight(color: new Vec3(3));
            return new()
            {
                new Sphere(new Vec3(0, -1000, 0), 1000, sphereMat),
                new Sphere(new Vec3(0, 2, 0), 2, sphereMat),
                new Sphere(new Vec3(0, 7, 0), 2, new DiffuseLight(color: new Vec3(3, 3, 0))),
                new XyRect(x: (3, 5), y: (1, 3), -2, diffuseLight),
            };
        }

        public static HittableList CornellBox()
        {
            Material red = new Lambertian(new Vec3(.65, .05, .05));
            Material white = new Lambertian(new Vec3(.73, .73, .73));
            Material green = new Lambertian(new Vec3(.12, .45, .15));
            Material light = new DiffuseLight(new Vec3(15, 15, 15));
            return new()
            {
                //new YzRect((0, 555), (0, 555), 555, green),
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 555, green),
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 0, red),
                new Rect(RectOrientation.XZ, (213, 343), (227, 332), 554, light),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 0, white),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 555, white),
                new Rect(RectOrientation.XY, (0, 555), (0, 555), 555, white),
            };
        }

        public static void Main()
        {
            // Image.
            double aspectRatio = 16 / 9.0;
            int imageWidth = 400;
            int samplesPerPixel = 100;
            const int maxDepth = 50;

            // World.
            HittableList world = Program.GetRandomScene();

            Vec3 lookFrom;
            Vec3 lookAt;
            Vec3 viewUp = new(0, 1, 0);
            const double distanceToFocus = 10;
            double verticalFov = 40;
            double aperture = .1;
            Vec3 backgroundColor = new();

            switch (0)
            {
                case 1:
                    world = Program.GetRandomScene();
                    backgroundColor = new Vec3(.7, .8, 1);
                    lookFrom = new Vec3(13, 2, 3);
                    lookAt = new Vec3(0, 0, 0);
                    verticalFov = 20;
                    aperture = .1;
                    break;
                case 2:
                    world = Program.TwoSpheres();
                    backgroundColor = new Vec3(.7, .8, 1);
                    lookFrom = new Vec3(13, 2, 3);
                    lookAt = new Vec3(0, 0, 0);
                    verticalFov = 20;
                    break;
                case 3:
                    world = Program.TwoPerlinSpheres();
                    backgroundColor = new Vec3(.7, .8, 1);
                    lookFrom = new Vec3(13, 2, 3);
                    lookAt = new Vec3(0, 0, 0);
                    verticalFov = 20;
                    break;
                case 4:
                    world = Program.Earth();
                    backgroundColor = new Vec3(.7, .8, 1);
                    lookFrom = new Vec3(13, 2, 3);
                    lookAt = new Vec3(0);
                    verticalFov = 20;
                    break;
                case 5:
                    world = Program.SimpleLight();
                    backgroundColor = new Vec3();
                    lookFrom = new Vec3(26, 3, 6);
                    lookAt = new Vec3(0, 2, 0);
                    verticalFov = 20;
                    samplesPerPixel = 400;
                    break;
                default:
                    world = Program.CornellBox();
                    aspectRatio = 1;
                    imageWidth = 600;
                    samplesPerPixel = 200;
                    backgroundColor = new Vec3();
                    lookFrom = new Vec3(278, 278, -800);
                    lookAt = new Vec3(278, 278, 0);
                    verticalFov = 40;
                    break;
            }

            int imageHeight = (int)(imageWidth / aspectRatio);

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
                    Vec3 pixelColor = new();
                    Parallel.For(0, samplesPerPixel - 1, (k) =>
                    {
                        double u = (j + random.NextDouble()) / (imageWidth - 1);
                        double v = (i + random.NextDouble()) / (imageHeight - 1);
                        Ray ray = camera.GetRay(u, v);
                        colors.Add((k, Program.RayColor(ray, backgroundColor, world, maxDepth)));
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