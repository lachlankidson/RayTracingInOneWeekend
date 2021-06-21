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
                new Rect(RectOrientation.XY, (3, 5), (1, 3), -2, diffuseLight),
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
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 555, green),
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 0, red),
                new Rect(RectOrientation.XZ, (213, 343), (227, 332), 554, light),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 0, white),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 555, white),
                new Rect(RectOrientation.XY, (0, 555), (0, 555), 555, white),
                new Translate(
                    hittable: new RotateY(
                        hittable: new Box((new Vec3(), new Vec3(165, 330, 165)), white),
                        angle: 15),
                    displacement: new Vec3(265, 0, 295)),
                new Translate(
                    hittable: new RotateY(
                        hittable: new Box((new Vec3(), new Vec3(165)), white),
                        angle: -18),
                    displacement: new Vec3(130, 0, 65)),
            };
        }

        public static HittableList CornellSmoke()
        {
            Material red = new Lambertian(new Vec3(.65, .05, .05));
            Material white = new Lambertian(new Vec3(.73, .73, .73));
            Material green = new Lambertian(new Vec3(.12, .45, .15));
            Material light = new DiffuseLight(new Vec3(7, 7, 7));
            return new()
            {
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 555, green),
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 0, red),
                new Rect(RectOrientation.XZ, (113, 443), (127, 432), 554, light),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 0, white),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 555, white),
                new Rect(RectOrientation.XY, (0, 555), (0, 555), 555, white),
                new ConstantMedium(
                    boundary: new Translate(
                        hittable: new RotateY(
                            hittable: new Box((new Vec3(), new Vec3(165, 330, 165)), white),
                            angle: 15),
                        displacement: new Vec3(265, 0, 295)),
                    density: 0.01,
                    texture: new SolidColor(new Vec3())),
                new ConstantMedium(
                    boundary: new Translate(
                        hittable: new RotateY(
                            hittable: new Box((new Vec3(), new Vec3(165)), white),
                            angle: -18),
                        displacement: new Vec3(130, 0, 65)),
                    density: 0.01,
                    texture: new SolidColor(1)),
            };
        }

        public static HittableList FinalScene()
        {
            HittableList boxes = new();
            Material ground = new Lambertian(new SolidColor(.48, .83, .53));
            const int boxesPerSide = 20;
            for (int i = 0; i < boxesPerSide; i++)
            {
                for (int j = 0; j < boxesPerSide; j++)
                {
                    const double w = 100;
                    double x0 = -1000 + (i * w);
                    double z0 = -1000 + (j * w);
                    double x1 = x0 + w;
                    double y1 = (new Random().NextDouble() * 100) + 1;
                    double z1 = z0 + w;
                    boxes.Add(new Box(
                        (new Vec3(x0, 0, z0), new Vec3(x1, y1, z1)),
                        ground));
                }
            }

            HittableList boxes2 = new();
            Material white = new Lambertian(new SolidColor(.73));
            for (int j = 0; j < 1000; j++)
            {
                boxes2.Add(new Sphere(Vec3.GetRandom(0, 165), 10, white));
            }

            Material dielectric = new Dielectric(1.5);
            Vec3 center = new(400);
            Vec3 center2 = center + new Vec3(30, 0, 0);
            Sphere boundary = new(new Vec3(360, 150, 145), 70, dielectric);
            return new()
            {
                new BvhNode(boxes, 0, 1),
                new Rect(RectOrientation.XZ, (123, 423), (147, 412), 554, new DiffuseLight(new Vec3(7, 7, 7))),
                new MovingSphere(center, center2, 50, new Lambertian(new Vec3(.7, .3, .1)), 0, 1),
                new Sphere(new Vec3(260, 150, 45), 50, dielectric),
                new Sphere(new Vec3(0, 150, 145), 50, new Metal(new Vec3(.8, .8, .9), 1)),
                boundary,
                new ConstantMedium(boundary, .2, new SolidColor(.2, .4, .9)),
                new ConstantMedium(new Sphere(new Vec3(), 5000, dielectric), .0001, new SolidColor(1)),
                new Sphere(new Vec3(400, 200, 400), 100, new Lambertian(new ImageTexture("Images/earthmap.jpg"))),
                new Sphere(new Vec3(220, 280, 300), 80, new Lambertian(new NoiseTexture(new Perlin(), .1))),
                new Translate(new RotateY(new BvhNode(boxes2, 0, 1), 15), new Vec3(-100, 270, 300)),
            };
        }


        public static void Main()
        {
            // Image.
            double aspectRatio = 16 / 9.0;
            int imageWidth = 400;
            int samplesPerPixel = 10;
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
                case 6:
                    world = Program.CornellBox();
                    aspectRatio = 1;
                    imageWidth = 600;
                    samplesPerPixel = 200;
                    backgroundColor = new Vec3();
                    lookFrom = new Vec3(278, 278, -800);
                    lookAt = new Vec3(278, 278, 0);
                    verticalFov = 40;
                    break;
                case 7:
                    world = Program.CornellSmoke();
                    aspectRatio = 1.0;
                    imageWidth = 600;
                    samplesPerPixel = 200;
                    lookFrom = new Vec3(278, 278, -800);
                    lookAt = new Vec3(278, 278, 0);
                    verticalFov = 40.0;
                    break;
                default:
                    world = Program.FinalScene();
                    aspectRatio = 1.0;
                    imageWidth = 500;
                    samplesPerPixel = 1000;
                    backgroundColor = new Vec3();
                    lookFrom = new Vec3(478, 278, -600);
                    lookAt = new Vec3(278, 278, 0);
                    verticalFov = 40.0;
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