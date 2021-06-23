namespace RayTracing.Hittables
{
    public abstract record Hittable
    {
        public virtual bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord) => false;

        public abstract bool BoundingBox(float time0, float time1, out AxisAlignedBoundingBox? boundingBox);
    }
}