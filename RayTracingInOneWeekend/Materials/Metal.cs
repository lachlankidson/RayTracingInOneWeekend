namespace RayTracing.Materials
{
    using RayTracing.Hittables;

    public record Metal(Vec3 Albedo, double Fuzz) : Material
    {
        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay)
        {
            Vec3 reflected = Vec3.Reflect(Vec3.UnitVector(incidentRay.Direction), hitRecord.Normal);
            scatteredRay = new Ray(hitRecord.Point, reflected + (this.Fuzz * Vec3.GetRandomInUnitSphere()), incidentRay.Time);
            attenuation = this.Albedo;
            return Vec3.DotProduct(scatteredRay.Direction, hitRecord.Normal) > 0;
        }
    }
}
