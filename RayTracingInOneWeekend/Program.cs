namespace RayTracingInOneWeekend
{
    using System;

    public static class Program
    {
        public static void Main()
        {
            const int imageHeight = 256;
            const int imageWidth = 256;

            Console.Write($"P3\n{imageWidth} {imageHeight}\n255\n");
            for (int i = imageHeight - 1; i >= 0; --i)
            {
                for (int j = 0; j < imageWidth;  ++j)
                {
                    double r = (double)j / (imageWidth - 1);
                    double g = (double)i / (imageHeight - 1);
                    double b = 0.25;

                    int ir = (int)(255.999 * r);
                    int ig = (int)(255.999 * g);
                    int ib = (int)(255.999 * b);

                    Console.Write($"{ir} {ig} {ib}\n");
                }
            }
        }
    }
}