namespace RayTracing.Textures
{
    using System;

    public class Perlin
    {
        private const int PointCount = 256;

        private readonly int[] permX;

        private readonly int[] permY;

        private readonly int[] permZ;

        private readonly Vec3[] ranvec;

        public Perlin()
        {
            Random random = new();
            this.ranvec = new Vec3[Perlin.PointCount];
            for (int i = 0; i < Perlin.PointCount; i++)
            {
                this.ranvec[i] = Vec3.UnitVector(Vec3.GetRandom(-1, 1));
            }

            this.permX = Perlin.GeneratePerm();
            this.permY = Perlin.GeneratePerm();
            this.permZ = Perlin.GeneratePerm();
        }

        public double Noise(Vec3 point)
        {
            static double Remainder(double x) => x - Math.Floor(x);
            double u = Remainder(point.X);
            double v = Remainder(point.Y);
            double w = Remainder(point.Z);

            static int Truncate(double x) => (int)Math.Floor(x);
            int i = Truncate(point.X);
            int j = Truncate(point.Y);
            int k = Truncate(point.Z);

            Vec3[,,] c = new Vec3[2, 2, 2];
            for (int di = 0; di < 2; di++)
            {
                for (int dj = 0; dj < 2; dj++)
                {
                    for (int dk = 0; dk < 2; dk++)
                    {
                        c[di, dj, dk] = this.ranvec[
                            this.permX[(i + di) & 255] ^
                            this.permY[(j + dj) & 255] ^
                            this.permZ[(k + dk) & 255]];
                    }
                }
            }

            return Perlin.TrilinearInterpolation(c, u, v, w);
        }

        private static double TrilinearInterpolation(Vec3[,,] c, double u, double v, double w)
        {
            static double Hermitian(double x) => Math.Pow(x, 2) * (3 - (2 * x));
            double uu = Hermitian(u);
            double vv = Hermitian(v);
            double ww = Hermitian(w);
            double accum = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vec3 weight = new(u - i, v - j, w - k);
                        accum += ((i * uu) + ((1 - i) * (1 - uu))) *
                            ((j * vv) + ((1 - j) * (1 - vv))) *
                            ((k * ww) + ((1 - k) * (1 - ww))) *
                            Vec3.DotProduct(c[i, j, k], weight);
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
