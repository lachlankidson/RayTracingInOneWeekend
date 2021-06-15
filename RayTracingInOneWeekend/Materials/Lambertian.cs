namespace RayTracing.Materials
{
    public class Lambertian : Material
    {
        Vec3 Albedo { get; init; }

        public Lambertian(Vec3 albedo)
        {
            this.Albedo = albedo;
        }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay)
        {
            Vec3 scatterDirection = hitRecord.Normal + Vec3.GetRandomUnitVector();
            if (scatterDirection.NearZero())
            {
                scatterDirection = hitRecord.Normal;
            }

            scatteredRay = new Ray(hitRecord.Point, scatterDirection);
            attenuation = this.Albedo;
            return true;
        }
    }
}
