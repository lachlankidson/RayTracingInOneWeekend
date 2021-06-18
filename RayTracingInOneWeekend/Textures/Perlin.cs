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
            static double Remainder(double x) => x - Math.Floor(x);
            double u = Remainder(point.X);
            double v = Remainder(point.Y);
            double w = Remainder(point.Z);

            static double Hermitian(double x) => Math.Pow(x, 2) * (3 - (2 * x));
            u = Hermitian(u);
            v = Hermitian(v);
            w = Hermitian(w);

            static int Truncate(double x) => (int)Math.Floor(x);
            int i = Truncate(point.X);
            int j = Truncate(point.Y);
            int k = Truncate(point.Z);

            double[,,] c = new double[2, 2, 2];
            for (int di = 0; di < 2; di++)
            {
                for (int dj = 0; dj < 2; dj++)
                {
                    for (int dk = 0; dk < 2; dk++)
                    {
                        c[di, dj, dk] = this.ranfloat[
                            this.permX[(i + di) & 255] ^
                            this.permY[(j + dj) & 255] ^
                            this.permZ[(k + dk) & 255]];
                    }
                }
            }

            return Perlin.TrilinearInterpolation(c, u, v, w);
        }

        private static double TrilinearInterpolation(double[,,] c, double u, double v, double w)
        {
            double accum = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        accum += ((i * u) + ((1 - i) * (1 - u))) *
                            ((j * v) + ((1 - j) * (1 - v))) *
                            ((k * w) + ((1 - k) * (1 - w))) *
                            c[i, j, k];
                    }
                }
            }

            return accum;
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
