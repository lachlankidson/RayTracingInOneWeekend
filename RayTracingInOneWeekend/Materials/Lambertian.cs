namespace RayTracing.Materials
{
    using RayTracing.Hittables;
    using RayTracing.Textures;

    public class Lambertian : Material
    {
        public Lambertian(Vec3 color)
        {
            this.Albedo = new SolidColor(color);
        }

        public Lambertian(Texture texture)
        {
            this.Albedo = texture;
        }

        public Texture Albedo { get; init; }

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
