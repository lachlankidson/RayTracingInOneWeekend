namespace RayTracing
{
    using System;
    using System.Collections.Generic;
    using RayTracing.Hittables;

    public class BoxComparer : IComparer<Hittable>
    {
        public int Axis { get; init; }

        public BoxComparer(int axis)
        {
            this.Axis = axis;
        }

        int IComparer<Hittable>.Compare(Hittable? x, Hittable? y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            bool condA = !x.BoundingBox(0, 0, out AxisAlignedBoundingBox boxA);
            bool condB = !x.BoundingBox(0, 0, out AxisAlignedBoundingBox boxB);

            if (condA || condB)
            {
                throw new ArgumentException($"No bounding box in {nameof(BvhNode)} constructor.");
            }

            return boxA.Minimum[this.Axis].CompareTo(boxB.Minimum[this.Axis]);
        }
    }
}
