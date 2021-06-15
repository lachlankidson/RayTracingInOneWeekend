namespace RayTracingInOneWeekend
{
    using RayTracingInOneWeekend.Materials;

    public struct HitRecord
    {
        public Vec3 Point;

        public Vec3 Normal;

        public double T;

        public bool FrontFace;

        public Material Material;

        public void SetFaceNormal(Ray ray, Vec3 outwardNormal)
        {
            this.FrontFace = Vec3.DotProduct(ray.Direction, outwardNormal) < 0;
            this.Normal = this.FrontFace ? outwardNormal : -outwardNormal;
        }
    }
}