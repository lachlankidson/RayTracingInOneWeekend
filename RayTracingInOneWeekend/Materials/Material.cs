namespace RayTracing.Materials
{
    public abstract class Material
    {

        public abstract bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay);
    }
}
