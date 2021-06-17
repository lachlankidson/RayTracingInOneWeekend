namespace RayTracing.Textures
{
    using System;
    using System.Linq;

    public class CheckerTexture : Texture
    {
        public CheckerTexture(Texture even, Texture odd)
        {
            this.Even = even;
            this.Odd = odd;
        }

        public Texture Even { get; init; }

        public Texture Odd { get; init; }

        public override Vec3 Value(double u, double v, Vec3 point)
        {
            double sines = Math.Sin(10 * point.X) * Math.Sin(10 * point.Y) * Math.Sin(10 * point.Z);
            Func<double, double, Vec3, Vec3> m = sines < 0 ? this.Odd.Value : this.Even.Value;
            return m(u, v, point);
        }
    }
}
