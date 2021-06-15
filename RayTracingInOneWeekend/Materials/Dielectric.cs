namespace RayTracingInOneWeekend.Materials
{
    using System;

    public class Dielectric : Material
    {
        public double RefractiveIndex { get; init; }

        public Dielectric(double refractiveIndex)
        {
            this.RefractiveIndex = refractiveIndex;
        }

        private static double Reflectance(double cosine, double reflectIndex)
        {
            double r0 = Math.Pow((1 - reflectIndex) / (1 + reflectIndex), 2);
            return r0 + ((1 - r0) * Math.Pow(1 - cosine, 5));
        }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay)
        {
            attenuation = new Vec3(1, 1, 1);
            double refractionRatio = hitRecord.FrontFace ? (1 / this.RefractiveIndex) : this.RefractiveIndex;
            Vec3 unitDirection = Vec3.UnitVector(incidentRay.Direction);
            double cosTheta = Math.Min(Vec3.DotProduct(-unitDirection, hitRecord.Normal), 1);
            double sinTheta = Math.Sqrt(1 - Math.Pow(cosTheta, 2));
            Random random = new ();
            bool cannotRefract =
                refractionRatio * sinTheta > 1 || Dielectric.Reflectance(cosTheta, refractionRatio) > random.NextDouble();
            Vec3 direction = cannotRefract
                ? Vec3.Reflect(unitDirection, hitRecord.Normal)
                : Vec3.Refract(unitDirection, hitRecord.Normal, refractionRatio);
            scatteredRay = new Ray(hitRecord.Point, direction);
            return true;
        }
    }
}
