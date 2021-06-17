namespace RayTracing.Hittables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetFabric.Hyperlinq;

    public class BvhNode : Hittable
    {
        public BvhNode()
        {
        }

        public BvhNode(
            IEnumerable<Hittable> source,
            ulong start,
            ulong end,
            double time0,
            double time1)
        {
            Hittable[] hittables = source.ToArray();
            int axis = new Random().Next(0, 2);
            IComparer<Hittable> boxComparer = new BoxComparer(axis);
            ulong objectSpan = end - start;
            if (objectSpan == 1)
            {
                this.Left = this.Right = hittables[start];
            }
            else if (objectSpan == 2)
            {
                bool compare = boxComparer.Compare(hittables[start], hittables[start + 1]) == -1;
                this.Left = hittables[compare ? start : start + 1];
                this.Right = hittables[compare ? start + 1 : start];
            }
            else
            {
                hittables = hittables.OrderBy(x => x, boxComparer).ToArray();
                ulong mid = start + (objectSpan / 2);
                this.Left = new BvhNode(hittables, start, mid, time0, time1);
                this.Right = new BvhNode(hittables, mid, end, time0, time1);
            }
        }

        public Hittable Left { get; init; }

        public Hittable Right { get; init; }

        AxisAlignedBoundingBox Box { get; init; }

        public override bool BoundingBox(double time0, double time1, out AxisAlignedBoundingBox boundingBox)
        {
            boundingBox = this.Box;
            return true;
        }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            if (!this.Box.Hit(ray, tMin, tMax))
            {
                return false;
            }

            bool hitLeft = this.Left.Hit(ray, tMin, tMax, ref hitRecord);
            bool hitRight = this.Right.Hit(ray, tMin, hitLeft ? hitRecord.T : tMax, ref hitRecord);
            return hitLeft || hitRight;
        }
    }
}
