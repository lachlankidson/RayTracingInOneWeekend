namespace RayTracing
{
    using System;
    using System.Collections.Generic;
    using RayTracing.Hittables;

    public record BoxComparer(int Axis) : IComparer<Hittable>
    {
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

            AxisAlignedBoundingBox? boxA = x.BoundingBox(0, 0);
            AxisAlignedBoundingBox? boxB = x.BoundingBox(0, 0);
            if (boxA is null || boxB is null)
            {
                throw new ArgumentException($"No bounding box in {nameof(BvhNode)} constructor.");
            }

            return boxA.Minimum.Get(this.Axis).CompareTo(boxB.Minimum.Get(this.Axis));
        }
    }
}
