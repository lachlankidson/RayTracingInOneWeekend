namespace RayTracing.Hittables
{
    using System.Collections;
    using System.Collections.Generic;

    public record HittableList : Hittable, IEnumerable<Hittable>
    {
        private List<Hittable> Hittables { get; init; } = new();

        public void Add(Hittable hittable)
        {
            this.Hittables.Add(hittable);
        }

        public override bool BoundingBox(float time0, float time1, out AxisAlignedBoundingBox? boundingBox)
        {
            boundingBox = null;
            if (this.Hittables.Count == 0)
            {
                return false;
            }

            bool firstBox = true;
            foreach (Hittable hittable in this.Hittables)
            {
                if (!hittable.BoundingBox(time0, time1, out AxisAlignedBoundingBox? tempBox) || tempBox is null)
                {
                    return false;
                }

                if (firstBox || boundingBox is null)
                {
                    boundingBox = tempBox;
                }
                else
                {
                    boundingBox = AxisAlignedBoundingBox.GetSurroundingBox(boundingBox, tempBox);
                }

                firstBox = false;
            }

            return true;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            HitRecord temp = default;
            bool hitAnything = false;
            float closestSoFar = tMax;
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
