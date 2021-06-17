namespace RayTracing.Materials
{
    using RayTracing.Hittables;

    public class Lambertian : Material
    {
        public Lambertian(Vec3 albedo)
        {
            this.Albedo = albedo;
        }

        public Vec3 Albedo { get; init; }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay)
        {
            Vec3 scatterDirection = hitRecord.Normal + Vec3.GetRandomUnitVector();
            if (scatterDirection.NearZero())
            {
                scatterDirection = hitRecord.Normal;
            }

            scatteredRay = new Ray(hitRecord.Point, scatterDirection, incidentRay.Time);
            attenuation = this.Albedo;
            return true;
        }
    }
}
