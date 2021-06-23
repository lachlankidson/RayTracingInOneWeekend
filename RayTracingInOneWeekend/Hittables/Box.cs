namespace RayTracing.Hittables
{
    using System.Numerics;
    using RayTracing.Materials;

    public record Box : Hittable
    {
        public Box((Vector3 A, Vector3 B) coords, Material material)
        {
            this.Coordinates = coords;
            this.Material = material;
            this.Sides = new HittableList()
            {
                new Rect(
                    RectOrientation.XY,
                    (this.Coordinates.A.X, this.Coordinates.B.X),
                    (this.Coordinates.A.Y, this.Coordinates.B.Y),
                    this.Coordinates.B.Z,
                    this.Material),
                new Rect(
                    RectOrientation.XY,
                    (this.Coordinates.A.X, this.Coordinates.B.X),
                    (this.Coordinates.A.Y, this.Coordinates.B.Y),
                    this.Coordinates.A.Z,
                    this.Material),
                new Rect(
                    RectOrientation.XZ,
                    (this.Coordinates.A.X, this.Coordinates.B.X),
                    (this.Coordinates.A.Z, this.Coordinates.B.Z),
                    this.Coordinates.B.Y,
                    this.Material),
                new Rect(
                    RectOrientation.XZ,
                    (this.Coordinates.A.X, this.Coordinates.B.X),
                    (this.Coordinates.A.Z, this.Coordinates.B.Z),
                    this.Coordinates.A.Y,
                    this.Material),
                new Rect(
                    RectOrientation.YZ,
                    (this.Coordinates.A.Y, this.Coordinates.B.Y),
                    (this.Coordinates.A.Z, this.Coordinates.B.Z),
                    this.Coordinates.B.X,
                    this.Material),
                new Rect(
                    RectOrientation.YZ,
                    (this.Coordinates.A.Y, this.Coordinates.B.Y),
                    (this.Coordinates.A.Z, this.Coordinates.B.Z),
                    this.Coordinates.A.X,
                    this.Material),
            };
        }

        public (Vector3 A, Vector3 B) Coordinates { get; init; }

        public Material Material { get; init; }

        public HittableList Sides { get; init; }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
            => this.Sides.Hit(ray, tMin, tMax, ref hitRecord);

        public override bool BoundingBox(float time0, float time1, out AxisAlignedBoundingBox boundingBox)
        {
            boundingBox = new AxisAlignedBoundingBox(
                minimum: this.Coordinates.A,
                maximum: this.Coordinates.B);
            return true;
        }
    }
}
