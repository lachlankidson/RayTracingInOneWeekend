namespace RayTracing.Hittables
{
    using RayTracing.Materials;

    public class XyRect : Hittable
    {
        public XyRect((double A, double B) x, (double A, double B) y, double k, Material material)
        {
            this.X = x;
            this.Y = y;
            this.K = k;
            this.Material = material;
        }

        public (double A, double B) X { get; init; }

        public (double A, double B) Y { get; init; }

        public double K { get; init; }

        public Material Material { get; init; }

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox boundingBox)
        {
            boundingBox = new AxisAlignedBoundingBox(
                minimum: new Vec3(this.X.A, this.Y.A, this.K - 0.0001),
                maximum: new Vec3(this.X.B, this.Y.B, this.K + 0.0001));
            return true;
        }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            double t = (this.K - ray.Origin.Z) / ray.Direction.Z;
            if (t < tMin || t > tMax)
            {
                return false;
            }

            double x = ray.Origin.X + (t * ray.Direction.X);
            double y = ray.Origin.Y + (t * ray.Direction.Y);
            if (x < this.X.A || x > this.X.B || y < this.Y.A || y > this.Y.B)
            {
                return false;
            }

            hitRecord.U = (x - this.X.A) / (this.X.B - this.X.A);
            hitRecord.V = (y - this.Y.A) / (this.Y.B - this.Y.A);
            hitRecord.T = t;
            Vec3 outwardNormal = new(0, 0, 1);
            hitRecord.SetFaceNormal(ray, outwardNormal);
            hitRecord.Material = this.Material;
            hitRecord.Point = ray.At(t);
            return true;
        }
    }
}