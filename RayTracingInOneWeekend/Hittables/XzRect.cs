namespace RayTracing.Hittables
{
    using RayTracing.Materials;

    public class XzRect : Hittable
    {
        public XzRect((double A, double B) x, (double A, double B) z, double k, Material material)
        {
            this.X = x;
            this.Z = z;
            this.K = k;
            this.Material = material;
        }

        public (double A, double B) X { get; init; }

        public (double A, double B) Z { get; init; }

        public double K { get; init; }

        public Material Material { get; init; }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            double t = (this.K - ray.Origin.Y) / ray.Direction.Y;
            if (t < tMin || t > tMax)
            {
                return false;
            }

            double x = ray.Origin.X + (t * ray.Direction.X);
            double z = ray.Origin.Z + (t * ray.Direction.Z);
            if (x < this.X.A || x > this.X.B || z < this.Z.A || z > this.Z.B)
            {
                return false;
            }

            hitRecord.U = (x - this.X.A) / (this.X.B - this.X.A);
            hitRecord.V = (z - this.Z.A) / (this.Z.B - this.Z.A);
            hitRecord.T = t;
            Vec3 outwardNormal = new(0, 1, 0);
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = this.Material;
            hitRecord.Point = ray.At(t);
            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox boundingBox)
        {
            boundingBox = new AxisAlignedBoundingBox(
                minimum: new Vec3(this.X.A, this.K - 0.0001, this.Z.A),
                maximum: new Vec3(this.X.B, this.K + 0.0001, this.Z.B));
            return true;
        }
    }
}
