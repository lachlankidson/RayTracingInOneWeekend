namespace RayTracing.Materials
{
    using System;
    using System.Numerics;
    using RayTracing.Hittables;

    public record Dielectric(float RefractiveIndex) : Material
    {
        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay)
        {
            attenuation = Vector3.One;
            float refractionRatio = hitRecord.FrontFace ? (1 / this.RefractiveIndex) : this.RefractiveIndex;
            Vector3 unitDirection = incidentRay.Direction.UnitVector();
            float cosTheta = (float)Math.Min(Vector3.Dot(-unitDirection, hitRecord.Normal), 1);
            float sinTheta = (float)Math.Sqrt(1 - Math.Pow(cosTheta, 2));
            Random random = new();
            bool cannotRefract =
                refractionRatio * sinTheta > 1 || Dielectric.Reflectance(cosTheta, refractionRatio) > random.NextDouble();
            Vector3 direction = cannotRefract
                ? Vector3.Reflect(unitDirection, hitRecord.Normal)
                : Utils.Refract(unitDirection, hitRecord.Normal, refractionRatio);
            scatteredRay = new Ray(hitRecord.Point, direction, incidentRay.Time);
            return true;
        }

        private static float Reflectance(float cosine, float reflectIndex)
        {
            float r0 = (float)Math.Pow((1 - reflectIndex) / (1 + reflectIndex), 2);
            return r0 + ((1 - r0) * (float)Math.Pow(1 - cosine, 5));
        }
    }
}
