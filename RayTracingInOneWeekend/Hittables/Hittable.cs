namespace RayTracing.Hittables
{
    public abstract record Hittable
    {
        public virtual bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord) => false;

        public abstract AxisAlignedBoundingBox? BoundingBox(float time0, float time1);
    }
}