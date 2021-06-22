namespace RayTracing.Materials
{
    using System.Numerics;
    using RayTracing.Hittables;
    using RayTracing.Textures;

    public record DiffuseLight(Texture Emit) : Material
    {
        public DiffuseLight(Vector3 color)
            : this(new SolidColor(color))
        {
        }

        public override bool Scatter(Ray incidentRay, HitRecord hitRecord, out Vector3 attenuation, out Ray scatteredRay)
        {
            attenuation = Vector3.Zero;
            scatteredRay = new Ray(Vector3.Zero, Vector3.Zero);
            return false;
        }

        public override Vector3 Emitted(float u, float v, Vector3 point)
        {
            return this.Emit.Value(u, v, point);
        }
    }
}