namespace RayTracingInOneWeekend
{
    public abstract class Hittable
    {
        public virtual bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord) => false;
    }
}