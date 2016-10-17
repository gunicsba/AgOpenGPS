﻿//Please, if you use this, share the improvements

using System;

namespace AgOpenGPS
{
    /// <summary>
    /// Represents a three dimensional vector.
    /// </summary>
    public struct vec3
    {
        public double x;
        public double y;
        public double z;

        public double this[int index]
        {
            get
            {
                if (index == 0) return x;
                else if (index == 1) return y;
                else if (index == 2) return z;
                else throw new Exception("Out of range.");
            }
            set
            {
                if (index == 0) x = value;
                else if (index == 1) y = value;
                else if (index == 2) z = value;
                else throw new Exception("Out of range.");
            }
        }

        public vec3(double s)
        {
            x = y = z = s;
        }

        public vec3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public vec3(vec3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

    }

    //
    public struct vec2
    {
        public double x;
        public double z;


        public vec2(double s)
        {
            x = z = s;
        }

        public vec2(double x, double z)
        {
            this.x = x;
            this.z = z;
        }

        public vec2(vec2 v)
        {
            this.x = v.x;
            this.z = v.z;
        }

    }
}