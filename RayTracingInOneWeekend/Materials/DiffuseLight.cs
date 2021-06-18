namespace RayTracing.Materials
{
    using RayTracing.Hittables;
    using RayTracing.Textures;

    public record DiffuseLight(Texture Emit) : Material
    {
        public DiffuseLight(Vec3 color)
            : this(new SolidColor(color))
        {
        }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay)
        {
            attenuation = new Vec3(0);
            scatteredRay = new Ray(new Vec3(0), new Vec3(0));
            return false;
        }

        public override Vec3 Emitted(double u, double v, Vec3 point)
        {
            return this.Emit.Value(u, v, point);
        }
    }
}