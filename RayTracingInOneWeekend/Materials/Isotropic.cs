namespace RayTracing.Materials
{
    using System.Numerics;
    using RayTracing.Hittables;
    using RayTracing.Textures;

    public record Isotropic(Texture Albedo) : Material
    {
        public Isotropic(Vector3 color)
            : this(new SolidColor(color))
        {
        }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay)
        {
            scatteredRay = new Ray(hitRecord.Point, Utils.GetRandomVec3InUnitSphere(), incidentRay.Time);
            attenuation = this.Albedo.Value(hitRecord.U, hitRecord.V, hitRecord.Point);
            return true;
        }
    }
}
