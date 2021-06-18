namespace RayTracing.Materials
{
    using RayTracing.Hittables;

    public abstract record Material
    {
        public abstract bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vec3 attenuation, out Ray scatteredRay);

        public virtual Vec3 Emitted(double u, double v, Vec3 point) =>
            new();
    }
}
