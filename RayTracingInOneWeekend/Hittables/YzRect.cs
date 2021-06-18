namespace RayTracing.Hittables
{
    using RayTracing.Materials;

    public class YzRect : Hittable
    {
        public YzRect((double A, double B) y, (double A, double B) z, double k, Material material)
        {
            this.Y = y;
            this.Z = z;
            this.K = k;
            this.Material = material;
        }

        public (double A, double B) Y { get; init; }

        public (double A, double B) Z { get; init; }

        public double K { get; init; }

        public Material Material { get; init; }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            double t = (this.K - ray.Origin.X) / ray.Direction.X;
            if (t < tMin || t > tMax)
            {
                return false;
            }

            double y = ray.Origin.Y + (t * ray.Direction.Y);
            double z = ray.Origin.Z + (t * ray.Direction.Z);
            if (y < this.Y.A || y > this.Y.B || z < this.Z.A || z > this.Z.B)
            {
                return false;
            }

            hitRecord.U = (y - this.Y.A) / (this.Y.B - this.Y.A);
            hitRecord.V = (z - this.Z.A) / (this.Z.B - this.Z.A);
            hitRecord.T = t;
            Vec3 outwardNormal = new(1, 0, 0);
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = this.Material;
            hitRecord.Point = ray.At(t);
            return true;
        }

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox boundingBox)
        {
            boundingBox = new AxisAlignedBoundingBox(
                minimum: new Vec3(this.K - 0.0001, this.Y.A, this.Z.A),
                maximum: new Vec3(this.K + 0.0001, this.Y.A, this.Z.B));
            return true;
        }
    }
}
