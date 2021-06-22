namespace RayTracing.Materials
{
    using System.Numerics;
    using RayTracing.Hittables;

    public abstract record Material
    {
        public abstract bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay);

        public virtual Vector3 Emitted(float u, float v, Vector3 point) => Vector3.Zero;
    }
}
