namespace RayTracing.Materials
{
    using System.Numerics;
    using RayTracing.Hittables;

    public record Metal(Vector3 Albedo, float Fuzz) : Material
    {
        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay)
        {
            Vector3 reflected = Vector3.Reflect(incidentRay.Direction.UnitVector(), hitRecord.Normal);
            scatteredRay = new Ray(hitRecord.Point, reflected + (this.Fuzz * Utils.GetRandomVec3InUnitSphere()), incidentRay.Time);
            attenuation = this.Albedo;
            return Vector3.Dot(scatteredRay.Direction, hitRecord.Normal) > 0;
        }
    }
}
