﻿using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PluginHelper
{
    public static class Maths
    {
        const double radtodeg = 180 / Math.PI;

        const double degtorad = Math.PI / 180;

        /// <summary>
        /// Convert Radians to Degrees
        /// </summary>
        /// <param name="rad">radians</param>
        /// <returns>degrees</returns>
        public static double ToDegrees(double rad)
        {
            return rad * radtodeg;
        }

        public static float ToDegrees(float rad)
        {
            return (float)(rad * radtodeg);
        }

        /// <summary>
        /// Convert Degrees to Radians
        /// </summary>
        /// <param name="deg">degrees</param>
        /// <returns>radians</returns>
        public static double ToRadians(double deg)
        {
            return deg * degtorad;
        }

        public static float ToRadians(float deg)
        {
            return (float)(deg * degtorad);
        }


        public static Quaternion ToQuat(this Vector3 v, float degrees = 0f)
        {
            return VectorToQuaternion(v.X, v.Y, v.Z, degrees);
        }

        public static Quaternion VectorToQuaternion(float x = 0f, float y = 0f, float z = 0f, float angle = 0f, bool angleInDegrees = true)
        {
            Vector3 axis = new Vector3(x, y, z);

            var a = 0f;
            if (angleInDegrees)
                a = ToRadians(angle / 2);
            else
                a = angle / 2;


            return new Quaternion(
                 (float)Math.Sin(a) * axis.X,
                 (float)Math.Sin(a) * axis.Y,
                 (float)Math.Sin(a) * axis.Z,
                 (float)Math.Cos(a));
        }

       

        /// <summary>
        /// Convert quaternion to Euler angles
        /// </summary>
        /// <param name="q"></param>
        /// <returns>pitch(x-rotation), yaw (y-rotation) , roll (z-rotation)</returns>
        public static (float pitch, float yaw, float roll) ToEuler(this Quaternion q, bool returnDegrees = true)
        {

            var p = (float) q.ToPitch();
            var y = (float) q.ToYaw();
            var r = (float) q.ToRoll();

            if (!returnDegrees)
                return (p,y,r);

            // Convert the angles from radians to degrees
            return (ToDegrees(p), ToDegrees(y), ToDegrees(r));

        }


        /// <summary>
        /// Convert a world space vector to local space
        /// </summary>
        /// <param name="q">a normalized quaternion</param>
        /// <param name="v_world">a global vctor</param>
        /// <returns></returns>
        public static Vector3 WorldtoLocal(Quaternion q, Vector3 v_world)
            => (Quaternion.Conjugate(q) * new Quaternion(v_world, 0) * q).Vector();

        /// <summary>
        /// Convert a world space vector to local space
        /// </summary>
        /// <param name="q">a normalized quaternion</param>
        /// <param name="v_local">a global vctor</param>
        /// <returns></returns>
        public static Vector3 LocalToWorld(Quaternion q, Vector3 v_local)
            => (q * new Quaternion(v_local, 0) * Quaternion.Conjugate(q)).Vector();


        public static Vector3 rotate_vector_by_quaternion(Quaternion q, Vector3 v)
        {
            // Extract the vector part of the quaternion
            var u = new Vector3(q.X, q.Y, q.Z);

            // Extract the scalar part of the quaternion
            float s = q.W;

            // Do the math
            var vprime = 2.0f * Vector3.Dot(u, v) * u
              + (s * s - Vector3.Dot(u, u)) * v
              + 2.0f * s * Vector3.Cross(u, v);

            return vprime;
        }


        /// <summary>
        /// Return the vector part of a quaternion
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Vector3 Vector(this Quaternion q) => new Vector3(q.X, q.Y, q.Z);

        private static double ToPitch(this Quaternion q)
        {
            double num = 2.0 * (q.X * q.Y + q.W * q.Y);
            double num2 = 2.0 * (q.W * q.X - q.Y * q.Z);
            double num3 = 1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
            return Math.Atan2(num2, Math.Sqrt(num * num + num3 * num3));
        }

        private static double ToYaw(this Quaternion q) => Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Y), 1.0 - 2.0 * (q.X * q.X + q.Y * q.Y));

        private static double ToRoll(this Quaternion q) => Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Z), 1.0 - 2.0 * (q.X * q.X + q.Z * q.Z));

        

        

        // Convert a vector of yaw pitch and roll to a quaternion
        /// <summary>
        /// convert euler angles to a "y-up" quaternion
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <param name="roll"></param>
        /// <param name="returnDegrees">return angles in degrees instead of radians</param>
        /// <returns></returns>
        public static Quaternion QuaternionFromEuler(float pitch, float yaw, float roll, bool inDegrees = true)
        {
            if (!inDegrees)
                return Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

            return Quaternion.CreateFromYawPitchRoll((float)ToRadians(yaw), (float)ToRadians(pitch), (float)ToRadians(roll));
        }


        /// <summary>
        /// Test if two floats are withing a tolerance of each other
        /// </summary>
        /// <param name="f"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsEqualish(this float a, float b, float tolerance = 0.0001f)
        {

            return Math.Abs(a - b) < tolerance;
        }



        /// <summary>
        /// Assume y is up and convert euler angles to a direction vector
        /// </summary>
        /// <param name="pitch">rotation around x</param>
        /// <param name="yaw">rotation around y</param>
        /// <param name="roll">rotation around z</param>
        /// <param name="inDegrees"></param>
        /// <returns></returns>
        public static Vector3 EulerToDirection(float pitch, float yaw, float roll, bool inDegrees = true)
        {
            // Convert Euler angles to a quaternion
            Quaternion q = QuaternionFromEuler(pitch, yaw, roll, inDegrees);

            // Define the base direction vector (forward direction)
            Vector3 baseDirection = new Vector3(0, 0, 1);

            // Rotate the base direction vector using the quaternion
            //Vector3 direction = Vector3.Transform(baseDirection, q);
            Vector3 direction = rotate_vector_by_quaternion(q, baseDirection);

            return direction;
        }
    }
}


