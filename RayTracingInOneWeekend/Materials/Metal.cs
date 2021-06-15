namespace RayTracingInOneWeekend.Materials
{
    public class Metal : Material
    {
        private double Fuzz { get; init; }

        private Vec3 Albedo { get; init; }

        public Metal(Vec3 albedo, double fuzz)
        {
            this.Albedo = albedo;
            this.Fuzz = fuzz < 1 ? fuzz : 1;
        }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay)
        {
            Vec3 reflected = Vec3.Reflect(Vec3.UnitVector(incidentRay.Direction), hitRecord.Normal);
            scatteredRay = new Ray(hitRecord.Point, reflected + (this.Fuzz * Vec3.GetRandomInUnitSphere()));
            attenuation = this.Albedo;
            return Vec3.DotProduct(scatteredRay.Direction, hitRecord.Normal) > 0;
        }
    }
}
