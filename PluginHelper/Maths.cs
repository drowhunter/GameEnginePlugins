using System;
using System.Numerics;

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
        public static double RadToDeg(double rad)
        {
            return rad * radtodeg;
        }

        /// <summary>
        /// Convert Degrees to Radians
        /// </summary>
        /// <param name="deg">degrees</param>
        /// <returns>radians</returns>
        public static double DegToRad(double deg)
        {
            return deg * degtorad;
        }



        public static Quaternion ToQuat(this Vector3 v, float degrees = 0f)
        {
            return VectorToQuaternion(v.X, v.Y, v.Z, degrees);
        }

        public static Quaternion VectorToQuaternion(float x = 0f, float y = 0f, float z = 0f, float degrees = 0f)
        {
            Vector3 axis = new Vector3(x, y, z);
            var R = new Quaternion(
                (float)Math.Sin(DegToRad(degrees / 2)) * axis.X,
                (float)Math.Sin(DegToRad(degrees / 2)) * axis.Y,
                (float)Math.Sin(DegToRad(degrees / 2)) * axis.Z,
                (float)Math.Cos(DegToRad(degrees / 2)));

            return R;
        }

        /// <summary>
        /// Convert quaternion to Euler angles
        /// </summary>
        /// <param name="q"></param>
        /// <returns>pitch(x-rotation), yaw (y-rotation) , roll (z-rotation)</returns>
        public static (float pitch, float yaw, float roll) ToEuler(this Quaternion q)
        {

            //var q_norm = new YawGEAPI.Quaternion(q.X, q.Y, q.Z, q.W);
            var loc_roll = q.ToRoll();

            var loc_pitch = q.ToPitch();

            var loc_yaw = q.ToYaw();

            // Convert the angles from radians to degrees
            var roll_deg = RadToDeg(loc_roll);
            var pitch_deg = RadToDeg(loc_pitch);
            var yaw_deg = RadToDeg(loc_yaw);

            return ((float) pitch_deg, (float)yaw_deg, (float)roll_deg);

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



        /// <summary>
        /// Return the vector part of a quaternion
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Vector3 Vector(this Quaternion q) => new Vector3(q.X, q.Y, q.Z);

        public static double ToPitch(this Quaternion q)
        {
            double num = 2.0 * (q.X * q.Y + q.W * q.Y);
            double num2 = 2.0 * (q.W * q.X - q.Y * q.Z);
            double num3 = 1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
            return Math.Atan2(num2, Math.Sqrt(num * num + num3 * num3));
        }

        public static double ToYaw(this Quaternion q) => Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Y), 1.0 - 2.0 * (q.X * q.X + q.Y * q.Y));

        public static double ToRoll(this Quaternion q) => Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Z), 1.0 - 2.0 * (q.X * q.X + q.Z * q.Z));


        // Convert a vector of yaw pitch and roll to a quaternion
        public static Quaternion YawPitchRollToQuaternion(Vector3 ypr)
        {
            return Quaternion.CreateFromYawPitchRoll((float)DegToRad(ypr.X), (float)DegToRad(ypr.Y), (float)DegToRad(ypr.Z));
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
    }
}


