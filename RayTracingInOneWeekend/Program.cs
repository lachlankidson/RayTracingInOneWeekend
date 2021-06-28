namespace RayTracing
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using RayTracing.Hittables;

    public static class Program
    {
        public static void Main()
        {
            // Image.
            float aspectRatio = 16 / 9f;
            int imageWidth = 1200;
            int samplesPerPixel = 100;
            const int maxDepth = 50;

            // World.
            HittableList world = Scenes.GetRandomScene();

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
                    world = Scenes.GetRandomScene();
                    backgroundColor = new Vector3(.7f, .8f, 1);
                    lookFrom = new Vector3(13, 2, 3);
                    lookAt = Vector3.Zero;
                    verticalFov = 20;
                    aperture = .1f;
                    break;
                case 2:
                    world = Scenes.TwoSpheres();
                    backgroundColor = new Vector3(.7f, .8f, 1);
                    lookFrom = new Vector3(13, 2, 3);
                    lookAt = Vector3.Zero;
                    verticalFov = 20;
                    break;
                case 3:
                    world = Scenes.TwoPerlinSpheres();
                    backgroundColor = new Vector3(.7f, .8f, 1);
                    lookFrom = new Vector3(13, 2, 3);
                    lookAt = Vector3.Zero;
                    verticalFov = 20;
                    break;
                case 4:
                    world = Scenes.Earth();
                    backgroundColor = new Vector3(.7f, .8f, 1);
                    lookFrom = new Vector3(13, 2, 3);
                    lookAt = Vector3.Zero;
                    verticalFov = 20;
                    break;
                case 5:
                    world = Scenes.SimpleLight();
                    backgroundColor = Vector3.Zero;
                    lookFrom = new Vector3(26, 3, 6);
                    lookAt = new Vector3(0, 2, 0);
                    verticalFov = 20;
                    samplesPerPixel = 400;
                    break;
                case 6:
                    world = Scenes.CornellBox();
                    aspectRatio = 1;
                    imageWidth = 600;
                    samplesPerPixel = 200;
                    backgroundColor = Vector3.Zero;
                    lookFrom = new Vector3(278, 278, -800);
                    lookAt = new Vector3(278, 278, 0);
                    verticalFov = 40;
                    break;
                case 7:
                    world = Scenes.CornellSmoke();
                    aspectRatio = 1f;
                    imageWidth = 600;
                    samplesPerPixel = 200;
                    lookFrom = new Vector3(278, 278, -800);
                    lookAt = new Vector3(278, 278, 0);
                    verticalFov = 40;
                    break;
                default:
                    world = Scenes.FinalScene();
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
                        colors.Add((k, ray.GetColor(backgroundColor, world, maxDepth)));
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