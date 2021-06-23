namespace RayTracing
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using RayTracing.Hittables;
    using RayTracing.Materials;
    using RayTracing.Textures;

    public static class Program
    {
        public static Vector3 RayColor(Ray ray, Vector3 background, Hittable world, uint depth)
        {
            if (depth == 0)
            {
                return Vector3.Zero;
            }

            HitRecord hitRecord = default;
            if (!world.Hit(ray, 0.001f, float.PositiveInfinity, ref hitRecord))
            {
                return background;
            }

            Vector3 emitted = hitRecord.Material.Emitted(hitRecord.U, hitRecord.V, hitRecord.Point);
            if (!hitRecord.Material.Scatter(ray, hitRecord, out Vector3 attenuation, out Ray scatteredRay))
            {
                return emitted;
            }

            return emitted + (attenuation * Program.RayColor(scatteredRay, background, world, depth - 1));
        }

        public static HittableList GetRandomScene()
        {
            Random random = new();
            HittableList world = new();
            CheckerTexture checker = new(new SolidColor(.2f, .3f, .1f), new SolidColor(.9f));
            world.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(checker)));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double chooseMat = random.NextDouble();
                    Vector3 center = new(a + (.9f * (float)random.NextDouble()), .2f, b + (.9f * (float)random.NextDouble()));
                    if ((center - new Vector3(4, .2f, 0)).Length() > .9)
                    {
                        Material sphereMaterial;
                        if (chooseMat < .8)
                        {
                            // Diffuse.
                            Vector3 albedo = Utils.GetRandomVec3() * Utils.GetRandomVec3();
                            sphereMaterial = new Lambertian(albedo);
                            Vector3 center2 = center + new Vector3(0, (float)random.NextDouble() / 2, 0);
                            world.Add(new MovingSphere(
                                startCenter: center,
                                endCenter: center2,
                                radius: .2f,
                                material: sphereMaterial,
                                startMoving: 0,
                                stopMoving: 1));
                        }
                        else if (chooseMat < .95)
                        {
                            // Metal.
                            Vector3 albedo = Utils.GetRandomVec3(.5f, 1);
                            float fuzz = (float)random.NextDouble() / 2;
                            sphereMaterial = new Metal(albedo, fuzz);
                            world.Add(new Sphere(center, .2f, sphereMaterial));
                        }
                        else
                        {
                            // Glass.
                            sphereMaterial = new Dielectric(1.5f);
                            world.Add(new Sphere(center, .2f, sphereMaterial));
                        }
                    }
                }
            }

            world.Add(new Sphere(new Vector3(0, 1, 0), 1, new Dielectric(1.5f)));
            world.Add(new Sphere(new Vector3(-4, 1, 0), 1, new Lambertian(new Vector3(.4f, .2f, .1f))));
            world.Add(new Sphere(new Vector3(4, 1, 0), 1, new Metal(new Vector3(.7f, .6f, .5f), 0)));
            return world;
        }

        public static HittableList TwoSpheres()
        {
            CheckerTexture checker = new(new SolidColor(.2f, .3f, .1f), new SolidColor(.9f));
            Material sphereMat = new Lambertian(checker);
            return new()
            {
                new Sphere(new Vector3(0, -10, 0), 10, sphereMat),
                new Sphere(new Vector3(0, 10, 0), 10, sphereMat),
            };
        }

        public static HittableList TwoPerlinSpheres()
        {
            Texture noise = new NoiseTexture(new Perlin(), 4);
            Material sphereMat = new Lambertian(noise);
            return new()
            {
                new Sphere(new Vector3(0, -1000, 0), 1000, sphereMat),
                new Sphere(new Vector3(0, 2, 0), 2, sphereMat),
            };
        }

        public static HittableList Earth()
        {
            Texture earthTexture = new ImageTexture("Images/earthmap.jpg");
            Material earthMaterial = new Lambertian(earthTexture);
            Sphere globe = new(Vector3.Zero, 2, earthMaterial);
            return new()
            {
                globe,
            };
        }

        public static HittableList SimpleLight()
        {
            Texture pertext = new NoiseTexture(Noise: new Perlin(), Scale: 4);
            Material sphereMat = new Lambertian(pertext);
            Material diffuseLight = new DiffuseLight(color: new Vector3(3));
            return new()
            {
                new Sphere(new Vector3(0, -1000, 0), 1000, sphereMat),
                new Sphere(new Vector3(0, 2, 0), 2, sphereMat),
                new Sphere(new Vector3(0, 7, 0), 2, new DiffuseLight(color: new Vector3(3, 3, 0))),
                new Rect(RectOrientation.XY, (3, 5), (1, 3), -2, diffuseLight),
            };
        }

        public static HittableList CornellBox()
        {
            Material red = new Lambertian(new Vector3(.65f, .05f, .05f));
            Material white = new Lambertian(new Vector3(.73f, .73f, .73f));
            Material green = new Lambertian(new Vector3(.12f, .45f, .15f));
            Material light = new DiffuseLight(new Vector3(15, 15, 15));
            return new()
            {
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 555, green),
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 0, red),
                new Rect(RectOrientation.XZ, (213, 343), (227, 332), 554, light),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 0, white),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 555, white),
                new Rect(RectOrientation.XY, (0, 555), (0, 555), 555, white),
                new Translate(
                    Hittable: new RotateY(
                        hittable: new Box((Vector3.Zero, new Vector3(165, 330, 165)), white),
                        angle: 15),
                    Displacement: new Vector3(265, 0, 295)),
                new Translate(
                    Hittable: new RotateY(
                        hittable: new Box((Vector3.Zero, new Vector3(165)), white),
                        angle: -18),
                    Displacement: new Vector3(130, 0, 65)),
            };
        }

        public static HittableList CornellSmoke()
        {
            Material red = new Lambertian(new Vector3(.65f, .05f, .05f));
            Material white = new Lambertian(new Vector3(.73f, .73f, .73f));
            Material green = new Lambertian(new Vector3(.12f, .45f, .15f));
            Material light = new DiffuseLight(new Vector3(7));
            return new()
            {
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 555, green),
                new Rect(RectOrientation.YZ, (0, 555), (0, 555), 0, red),
                new Rect(RectOrientation.XZ, (113, 443), (127, 432), 554, light),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 0, white),
                new Rect(RectOrientation.XZ, (0, 555), (0, 555), 555, white),
                new Rect(RectOrientation.XY, (0, 555), (0, 555), 555, white),
                new ConstantMedium(
                    Boundary: new Translate(
                        Hittable: new RotateY(
                            hittable: new Box((Vector3.Zero, new Vector3(165, 330, 165)), white),
                            angle: 15),
                        Displacement: new Vector3(265, 0, 295)),
                    Density: .01f,
                    PhaseFunction: new Isotropic(new SolidColor(Vector3.Zero))),
                new ConstantMedium(
                    Boundary: new Translate(
                        Hittable: new RotateY(
                            hittable: new Box((Vector3.Zero, new Vector3(165)), white),
                            angle: -18),
                        Displacement: new Vector3(130, 0, 65)),
                    Density: .01f,
                    PhaseFunction: new Isotropic(new SolidColor(1))),
            };
        }

        public static HittableList FinalScene()
        {
            HittableList boxes = new();
            Material ground = new Lambertian(new SolidColor(.48f, .83f, .53f));
            const int boxesPerSide = 20;
            for (int i = 0; i < boxesPerSide; i++)
            {
                for (int j = 0; j < boxesPerSide; j++)
                {
                    const float w = 100;
                    float x0 = -1000 + (i * w);
                    float z0 = -1000 + (j * w);
                    float x1 = x0 + w;
                    float y1 = ((float)new Random().NextDouble() * 100) + 1;
                    float z1 = z0 + w;
                    boxes.Add(new Box(
                        (new Vector3(x0, 0, z0), new Vector3(x1, y1, z1)),
                        ground));
                }
            }

            HittableList boxes2 = new();
            Material white = new Lambertian(new SolidColor(.73f));
            for (int j = 0; j < 1000; j++)
            {
                boxes2.Add(new Sphere(Utils.GetRandomVec3(0, 165), 10, white));
            }

            Material dielectric = new Dielectric(1.5f);
            Vector3 center = new(400);
            Vector3 center2 = center + new Vector3(30, 0, 0);
            Sphere boundary = new(new Vector3(360, 150, 145), 70, dielectric);
            return new()
            {
                new BvhNode(boxes, 0, 1),
                new Rect(RectOrientation.XZ, (123, 423), (147, 412), 554, new DiffuseLight(new Vector3(7))),
                new MovingSphere(center, center2, 50, new Lambertian(new Vector3(.7f, .3f, .1f)), 0, 1),
                new Sphere(new Vector3(260, 150, 45), 50, dielectric),
                new Sphere(new Vector3(0, 150, 145), 50, new Metal(new Vector3(.8f, .8f, .9f), 1)),
                boundary,
                new ConstantMedium(boundary, .2f, new Isotropic(new SolidColor(.2f, .4f, .9f))),
                new ConstantMedium(new Sphere(Vector3.Zero, 5000, dielectric), .0001f, new Isotropic(new SolidColor(1))),
                new Sphere(new Vector3(400, 200, 400), 100, new Lambertian(new ImageTexture("Images/earthmap.jpg"))),
                new Sphere(new Vector3(220, 280, 300), 80, new Lambertian(new NoiseTexture(new Perlin(), .1f))),
                new Translate(new RotateY(new BvhNode(boxes2, 0, 1), 15), new Vector3(-100, 270, 300)),
            };
        }

        public static void Main()
        {
            // Image.
            float aspectRatio = 16 / 9f;
            int imageWidth = 1200;
            int samplesPerPixel = 100;
            const int maxDepth = 50;

            // World.
            HittableList world = Program.GetRandomScene();

            Vector3 lookFrom;
            Vector3 lookAt;
            Vector3 viewUp = Vector3.UnitY;
            const float distanceToFocus = 10;
            float verticalFov = 40;
            float aperture = .1f;
            Vector3 backgroundColor = Vector3.Zero;
            switch (1)
            {
                case 1:
                    world = Program.GetRandomScene();
                    backgroundColor = new Vector3(.7f, .8f, 1);
                    lookFrom = new Vector3(13, 2, 3);
                    lookAt = Vector3.Zero;
                    verticalFov = 20;
                    aperture = .1f;
                    break;
                case 2:
                    world = Program.TwoSpheres();
                    backgroundColor = new Vector3(.7f, .8f, 1);
                    lookFrom = new Vector3(13, 2, 3);
                    lookAt = Vector3.Zero;
                    verticalFov = 20;
                    break;
                case 3:
                    world = Program.TwoPerlinSpheres();
                    backgroundColor = new Vector3(.7f, .8f, 1);
                    lookFrom = new Vector3(13, 2, 3);
                    lookAt = Vector3.Zero;
                    verticalFov = 20;
                    break;
                case 4:
                    world = Program.Earth();
                    backgroundColor = new Vector3(.7f, .8f, 1);
                    lookFrom = new Vector3(13, 2, 3);
                    lookAt = Vector3.Zero;
                    verticalFov = 20;
                    break;
                case 5:
                    world = Program.SimpleLight();
                    backgroundColor = Vector3.Zero;
                    lookFrom = new Vector3(26, 3, 6);
                    lookAt = new Vector3(0, 2, 0);
                    verticalFov = 20;
                    samplesPerPixel = 400;
                    break;
                case 6:
                    world = Program.CornellBox();
                    aspectRatio = 1;
                    imageWidth = 600;
                    samplesPerPixel = 200;
                    backgroundColor = Vector3.Zero;
                    lookFrom = new Vector3(278, 278, -800);
                    lookAt = new Vector3(278, 278, 0);
                    verticalFov = 40;
                    break;
                case 7:
                    world = Program.CornellSmoke();
                    aspectRatio = 1f;
                    imageWidth = 600;
                    samplesPerPixel = 200;
                    lookFrom = new Vector3(278, 278, -800);
                    lookAt = new Vector3(278, 278, 0);
                    verticalFov = 40;
                    break;
                default:
                    world = Program.FinalScene();
                    aspectRatio = 1;
                    imageWidth = 800;
                    samplesPerPixel = 5000;
                    backgroundColor = Vector3.Zero;
                    lookFrom = new Vector3(478, 278, -600);
                    lookAt = new Vector3(278, 278, 0);
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
                    ConcurrentBag<(int, Vector3)> colors = new();
                    Vector3 pixelColor = Vector3.Zero;
                    Parallel.For(0, samplesPerPixel - 1, (k) =>
                    {
                        float u = (j + (float)random.NextDouble()) / (imageWidth - 1);
                        float v = (i + (float)random.NextDouble()) / (imageHeight - 1);
                        Ray ray = camera.GetRay(u, v);
                        colors.Add((k, Program.RayColor(ray, backgroundColor, world, maxDepth)));
                    });

                    foreach ((int, Vector3) pair in colors.OrderBy(x => x.Item1))
                    {
                        pixelColor += pair.Item2;
                    }

                    ppms.Add((j, pixelColor.GetPpmString(samplesPerPixel)));
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