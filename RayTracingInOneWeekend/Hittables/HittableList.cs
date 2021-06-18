namespace RayTracing.Hittables
{
    using System.Collections;
    using System.Collections.Generic;

    public class HittableList : Hittable, IEnumerable<Hittable>
    {
        public HittableList()
        {
        }

        public HittableList(Hittable hittable)
        {
            this.Hittables.Add(hittable);
        }

        public HittableList(IEnumerable<Hittable> hittables)
        {
            this.Hittables.AddRange(hittables);
        }

        private List<Hittable> Hittables { get; init; } = new();

        public void Add(Hittable hittable)
        {
            this.Hittables.Add(hittable);
        }

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox boundingBox)
        {
            boundingBox = new AxisAlignedBoundingBox();
            if (this.Hittables.Count == 0)
            {
                return false;
            }

            bool firstBox = true;
            foreach (Hittable hittable in this.Hittables)
            {
                if (!hittable.BoundingBox(time0, time1, out AxisAlignedBoundingBox tempBox))
                {
                    return false;
                }

                boundingBox = firstBox ? tempBox : AxisAlignedBoundingBox.GetSurroundingBox(boundingBox, tempBox);
                firstBox = false;
            }

            return true;
        }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            HitRecord temp = default;
            bool hitAnything = false;
            double closestSoFar = tMax;
            foreach (Hittable hittable in this.Hittables)
            {
                if (hittable.Hit(ray, tMin, closestSoFar, ref temp))
                {
                    hitAnything = true;
                    closestSoFar = temp.T;
                    hitRecord = temp;
                }
            }

            return hitAnything;
        }

        IEnumerator<Hittable> IEnumerable<Hittable>.GetEnumerator() =>
            this.Hittables.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            this.Hittables.GetEnumerator();
    }
}
