namespace RayTracing.Hittables
{
    using System;
    using RayTracing.Materials;

    public class Box : Hittable
    {
        public Box((Vec3 A, Vec3 B) coords, Material material)
        {
            this.Coordinates = coords;
            this.Material = material;
            this.Sides = new HittableList()
            {
                new Rect(
                    RectOrientation.XY,
                    a: (this.Coordinates.A.X, this.Coordinates.B.X),
                    b: (this.Coordinates.A.Y, this.Coordinates.B.Y),
                    k: this.Coordinates.B.Z,
                    this.Material),
                new Rect(
                    RectOrientation.XY,
                    a: (this.Coordinates.A.X, this.Coordinates.B.X),
                    b: (this.Coordinates.A.Y, this.Coordinates.B.Y),
                    k: this.Coordinates.A.Z,
                    this.Material),
                new Rect(
                    RectOrientation.XZ,
                    a: (this.Coordinates.A.X, this.Coordinates.B.X),
                    b: (this.Coordinates.A.Z, this.Coordinates.B.Z),
                    k: this.Coordinates.B.Y,
                    this.Material),
                new Rect(
                    RectOrientation.XZ,
                    a: (this.Coordinates.A.X, this.Coordinates.B.X),
                    b: (this.Coordinates.A.Z, this.Coordinates.B.Z),
                    k: this.Coordinates.A.Y,
                    this.Material),
                new Rect(
                    RectOrientation.YZ,
                    a: (this.Coordinates.A.Y, this.Coordinates.B.Y),
                    b: (this.Coordinates.A.Z, this.Coordinates.B.Z),
                    k: this.Coordinates.B.X,
                    this.Material),
                new Rect(
                    RectOrientation.YZ,
                    a: (this.Coordinates.A.Y, this.Coordinates.B.Y),
                    b: (this.Coordinates.A.Z, this.Coordinates.B.Z),
                    k: this.Coordinates.A.X,
                    this.Material),
            };
        }

        public (Vec3 A, Vec3 B) Coordinates { get; init; }

        public Material Material { get; init; }

        public HittableList Sides { get; init; }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
            => this.Sides.Hit(ray, tMin, tMax, ref hitRecord);

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox boundingBox)
        {
            boundingBox = new AxisAlignedBoundingBox(
                minimum: this.Coordinates.A,
                maximum: this.Coordinates.B);
            return true;
        }
    }
}
