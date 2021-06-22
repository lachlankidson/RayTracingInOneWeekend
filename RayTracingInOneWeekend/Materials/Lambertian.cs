namespace RayTracing.Materials
{
    using System.Numerics;
    using RayTracing.Hittables;
    using RayTracing.Textures;

    public record Lambertian(Texture Albedo) : Material
    {
        public Lambertian(Vector3 color)
            : this(new SolidColor(color))
        {
        }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay)
        {
            Vector3 scatterDirection = hitRecord.Normal + Utils.GetRandomUnitVector();
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