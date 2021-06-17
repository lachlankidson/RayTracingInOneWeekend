namespace RayTracing.Textures
{
    using System;

    public class Perlin
    {
        private static readonly int PointCount = 256;

        public Perlin()
        {
            Random random = new();
            this.ranfloat = new double[Perlin.PointCount];
            for (int i = 0; i < Perlin.PointCount; i++)
            {
                this.ranfloat[i] = random.NextDouble();
            }

            this.permX = Perlin.GeneratePerm();
            this.permY = Perlin.GeneratePerm();
            this.permZ = Perlin.GeneratePerm();
        }

        private readonly int[] permX;

        private readonly int[] permY;

        private readonly int[] permZ;

        private readonly double[] ranfloat;

        public double Noise(Vec3 point)
        {
            Func<double, int> method = (x) => (int)(4 * x) & 255;
            int i = method(point.X);
            int j = method(point.Y);
            int k = method(point.Z);
            return this.ranfloat[this.permX[i] ^ this.permY[j] ^ this.permZ[k]];
        }

        private static int[] GeneratePerm()
        {
            int[] p = new int[Perlin.PointCount];
            for (int i = 0; i < Perlin.PointCount; i++)
            {
                p[i] = i;
            }

            Perlin.Permute(p, Perlin.PointCount);
            return p;
        }

        private static void Permute(int[] array, int n)
        {
            Random random = new();
            for (int i = n - 1; i > 0; i--)
            {
                int target = random.Next(0, i);
                (array[i], array[target]) = (array[target], array[i]);
            }
        }
    }
}
