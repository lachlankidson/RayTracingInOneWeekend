namespace RayTracing.Textures
{
    using System;
    using System.Numerics;

    public class Perlin
    {
        private const int PointCount = 256;

        private readonly int[] permX;

        private readonly int[] permY;

        private readonly int[] permZ;

        private readonly Vector3[] ranvec;

        public Perlin()
        {
            this.ranvec = new Vector3[Perlin.PointCount];
            for (int i = 0; i < Perlin.PointCount; i++)
            {
                this.ranvec[i] = Utils.GetRandomVec3(-1, 1).UnitVector();
            }

            this.permX = Perlin.GeneratePerm();
            this.permY = Perlin.GeneratePerm();
            this.permZ = Perlin.GeneratePerm();
        }

        public float Noise(Vector3 point)
        {
            static float Remainder(float x) => x - (float)Math.Floor(x);
            float u = Remainder(point.X);
            float v = Remainder(point.Y);
            float w = Remainder(point.Z);

            static int Truncate(float x) => (int)Math.Floor(x);
            int i = Truncate(point.X);
            int j = Truncate(point.Y);
            int k = Truncate(point.Z);

            Vector3[,,] c = new Vector3[2, 2, 2];
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

        public double Turbulence(Vector3 point, int depth = 7)
        {
            double accum = 0;
            Vector3 tempPoint = point;
            double weight = 1;
            for (int i = 0; i < depth; i++)
            {
                accum += weight * this.Noise(tempPoint);
                weight *= 0.5;
                tempPoint *= 2;
            }

            return Math.Abs(accum);
        }

        private static float TrilinearInterpolation(Vector3[,,] c, float u, float v, float w)
        {
            static float Hermitian(float x) => (float)Math.Pow(x, 2) * (3 - (2 * x));
            float uu = Hermitian(u);
            float vv = Hermitian(v);
            float ww = Hermitian(w);
            float accum = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 weight = new(u - i, v - j, w - k);
                        accum += ((i * uu) + ((1 - i) * (1 - uu))) *
                            ((j * vv) + ((1 - j) * (1 - vv))) *
                            ((k * ww) + ((1 - k) * (1 - ww))) *
                            Vector3.Dot(c[i, j, k], weight);
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
