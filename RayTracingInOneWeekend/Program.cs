namespace RayTracingInOneWeekend
{
    using System;
    using Color = Vec3;

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
                    Color color = new(
                        (double)j / (imageWidth - 1),
                        (double)i / (imageHeight - 1),
                        0.25);

                    Console.WriteLine(color.ToPpmString());
                }
            }
        }
    }
}