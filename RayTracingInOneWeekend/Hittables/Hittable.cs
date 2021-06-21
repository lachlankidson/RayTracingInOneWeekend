namespace RayTracing.Hittables
{
    public abstract class Hittable
    {
        public virtual bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord) => false;

        public abstract bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox? boundingBox);
    }
}