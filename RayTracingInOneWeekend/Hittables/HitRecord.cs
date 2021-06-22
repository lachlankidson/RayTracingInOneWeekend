namespace RayTracing.Hittables
{
    using System.Numerics;
    using RayTracing.Materials;

    public struct HitRecord
    {
        public Vector3 Point;

        public Vector3 Normal;

        public float T;

        public float U;

        public float V;

        public bool FrontFace;

        public Material Material;

        public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
        {
            this.FrontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0;
            this.Normal = this.FrontFace ? outwardNormal : -outwardNormal;
        }
    }
}