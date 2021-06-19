namespace RayTracing.Materials
{
    using RayTracing.Hittables;
    using RayTracing.Textures;

    public record Isotropic(Texture Albedo) : Material
    {
        public Isotropic(Vec3 color)
            : this(new SolidColor(color))
        {
        }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay)
        {
            scatteredRay = new Ray(hitRecord.Point, Vec3.GetRandomInUnitSphere(), incidentRay.Time);
            attenuation = this.Albedo.Value(hitRecord.U, hitRecord.V, hitRecord.Point);
            return true;
        }
    }
}
