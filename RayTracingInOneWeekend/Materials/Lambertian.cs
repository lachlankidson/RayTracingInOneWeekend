namespace RayTracing.Materials
{
    using RayTracing.Hittables;
    using RayTracing.Textures;

    public record Lambertian(Texture Albedo) : Material
    {
        public Lambertian(Vec3 color)
            : this(new SolidColor(color))
        {
        }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay)
        {
            Vec3 scatterDirection = hitRecord.Normal + Vec3.GetRandomUnitVector();
            if (scatterDirection.NearZero())
            {
                scatterDirection = hitRecord.Normal;
            }

            scatteredRay = new Ray(hitRecord.Point, scatterDirection, incidentRay.Time);
            attenuation = this.Albedo.Value(hitRecord.U, hitRecord.V, hitRecord.Point);
            return true;
        }
    }
}