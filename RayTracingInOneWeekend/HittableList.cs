namespace RayTracing
{
    using System.Collections.Generic;

    public class HittableList : Hittable
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

        private List<Hittable> Hittables { get; init; } = new ();

        public void Add(Hittable hittable)
        {
            this.Hittables.Add(hittable);
        }

        public override bool Hit(Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            HitRecord temp = new ();
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
    }
}
