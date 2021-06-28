namespace RayTracing
{
    using System.Numerics;
    using RayTracing.Hittables;

    public record Ray(Vector3 Origin, Vector3 Direction, float Time = 0)
    {
        public Vector3 At(float t) => this.Origin + (t * this.Direction);

        public Vector3 GetColor(Vector3 background, Hittable world, uint depth)
        {
            if (depth == 0)
            {
                return Vector3.Zero;
            }

            HitRecord hitRecord = default;
            if (!world.Hit(this, 0.001f, float.PositiveInfinity, ref hitRecord))
            {
                return background;
            }

            Vector3 emitted = hitRecord.Material.Emitted(hitRecord.U, hitRecord.V, hitRecord.Point);
            if (!hitRecord.Material.Scatter(this, hitRecord, out Vector3 attenuation, out Ray scatteredRay))
            {
                return emitted;
            }

            return emitted + (attenuation * scatteredRay.GetColor(background, world, depth - 1));
        }
    }
}