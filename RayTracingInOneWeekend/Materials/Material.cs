namespace RayTracing.Materials
{
    using RayTracing.Hittables;

    public abstract class Material
    {
        public abstract bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay);
    }
}
