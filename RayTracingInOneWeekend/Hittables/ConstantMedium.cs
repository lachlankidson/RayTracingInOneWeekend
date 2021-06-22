﻿namespace RayTracing.Hittables
{
    using System;
    using System.Numerics;
    using RayTracing.Materials;
    using RayTracing.Textures;

    public class ConstantMedium : Hittable
    {
        public ConstantMedium(Hittable boundary, float density, Texture texture)
        {
            this.Boundary = boundary;
            this.Density = density;
            this.PhaseFunction = new Isotropic(texture);
        }

        public Hittable Boundary { get; init; }

        public float Density { get; init; }

        public Material PhaseFunction { get; init; }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            HitRecord rec1 = default;
            HitRecord rec2 = default;
            if (!this.Boundary.Hit(ray, float.NegativeInfinity, float.PositiveInfinity, ref rec1))
            {
                return false;
            }

            if (!this.Boundary.Hit(ray, rec1.T + 0.0001f, float.PositiveInfinity, ref rec2))
            {
                return false;
            }

            if (rec1.T < tMin)
            {
                rec1.T = tMin;
            }

            if (rec2.T < tMax)
            {
                rec2.T = tMax;
            }

            if (rec1.T >= rec2.T)
            {
                return false;
            }

            if (rec1.T < 0)
            {
                rec1.T = 0;
            }

            float rayLength = ray.Direction.Length();
            float distanceInsideBoundary = (rec2.T - rec1.T) * rayLength;
            float hitDistance = -1 / this.Density * (float)Math.Log(new Random().NextDouble());
            if (hitDistance > distanceInsideBoundary)
            {
                return false;
            }

            hitRecord.T = rec1.T + (hitDistance / rayLength);
            hitRecord.Point = ray.At(hitRecord.T);
            hitRecord.Normal = Vector3.UnitX;
            hitRecord.FrontFace = true;
            hitRecord.Material = this.PhaseFunction;
            return true;
        }

        public override bool BoundingBox(float time0, float time1, out AxisAlignedBoundingBox? boundingBox)
            => this.Boundary.BoundingBox(time0, time1, out boundingBox);
    }
}