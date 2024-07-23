using System;
using System.Numerics;

using YawGEAPI;

using Quaternion = System.Numerics.Quaternion;

namespace YawVR_Game_Engine.Plugin
{
    internal static class Maths
    {
        const double radtodeg = 180 / Math.PI;

        const double degtorad = Math.PI/180 ;

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

        public static YawGEAPI.Quaternion Conjugate(this YawGEAPI.Quaternion Q) => new YawGEAPI.Quaternion(-Q.x, -Q.y, -Q.z, Q.w);

        public static YawGEAPI.Quaternion Normalize(this YawGEAPI.Quaternion q)
        {

            var q_norm = new YawGEAPI.Quaternion(q.x, q.y, q.z, q.w);

            var norm = Math.Sqrt(q_norm.z * q_norm.z + q_norm.x * q_norm.x + q_norm.y * q_norm.y + q_norm.w * q_norm.w);
            q_norm.w /= norm;
            q_norm.z /= norm;
            q_norm.x /= norm;
            q_norm.y /= norm;

            return q_norm;

        }

        public static Quaternion ToQuat(this Vector3 v, float degrees = 0f)
        {
            return VectorToQuaternion(v.X, v.Y, v.Z, degrees);
        }

        public static Quaternion VectorToQuaternion(float x = 0f, float y = 0f, float z = 0f, float degrees = 0f)
        {
            Vector3 axis = new Vector3(x , y ,z);
            var R = new Quaternion(
                (float)Math.Sin(DegToRad(degrees/2)) * axis.X, 
                (float)Math.Sin(DegToRad(degrees / 2)) * axis.Y, 
                (float)Math.Sin(DegToRad(degrees / 2)) * axis.Z, 
                (float)Math.Cos(DegToRad(degrees / 2)));

            return R;
        }

        /// <summary>
        /// Convert quaternion to Euler angles
        /// </summary>
        /// <param name="q"></param>
        /// <returns>(yaw, pitch, roll)</returns>
        public static (float yaw, float pitch, float roll) ToEuler(this Quaternion q)
        {

            var q_norm = new YawGEAPI.Quaternion(q.X, q.Y, q.Z, q.W);


            // Compute the roll, pitch, and yaw angles in radians
            var loc_roll = q_norm.toRollFromYUp();

            var loc_pitch = q_norm.toPitchFromYUp();

            var loc_yaw = q_norm.toYawFromYUp();

            // Convert the angles from radians to degrees
            var roll_deg = RadToDeg(loc_roll);
            var pitch_deg = RadToDeg(loc_pitch);
            var yaw_deg = RadToDeg(loc_yaw);

            return ((float)yaw_deg,(float) - pitch_deg,(float) roll_deg);

        }


        public static Vector3 WorldVelocity_to_LocalVelocity(Quaternion q, Vector3 vw)
        {
            var qn = Quaternion.Normalize(q);

            Quaternion q_conj = Quaternion.Conjugate(qn);


            
            // Convert quaternion to rotation matrix
            var r = Matrix4x4.CreateFromQuaternion(q_conj);
            
            


            var retval = Vector3.Transform( Vector3.Transform(vw, q), q_conj);

            return retval;

            //var R = r.as_matrix();
            // Calculate local velocity vector
            //v_local = np.dot(R, v_world)
            //return v_local
        }

        

        /// <summary>
        /// Convert a world space vector to local space
        /// </summary>
        /// <param name="q">a normalized quaternion</param>
        /// <param name="v_world">a global vctor</param>
        /// <returns></returns>
        public static Vector3 WorldtoLocal(Quaternion q, Vector3 v_world)
        {
            Quaternion q_conj = Quaternion.Conjugate(q);

            Quaternion l_world = q_conj * new Quaternion(v_world, 0) * q;

            return l_world.VectorPart();
        }

        /// <summary>
        /// Convert a world space vector to local space
        /// </summary>
        /// <param name="q">a normalized quaternion</param>
        /// <param name="v_local">a global vctor</param>
        /// <returns></returns>
        public static Vector3 LocalToWorld(Quaternion q, Vector3 v_local)
        {
            Quaternion q_conj = Quaternion.Conjugate(q);

            Quaternion q_world = q * new Quaternion(v_local, 0) * q_conj;

            return q_world.VectorPart();
        }

        /// <summary>
        /// Return the vector part of a quaternion
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Vector3 VectorPart(this Quaternion q)
        {
            return new Vector3(q.X, q.Y, q.Z);
        }


        public static Vector3 CrossProduct(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        public static Vector3 DotProduct(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static float Magnitude(this Vector3 a)
        {
            return (float) Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
        }

        public static float SquaredMagnitude(this Vector3 a) 
        {
            return a.X * a.X + a.Y * a.Y + a.Z * a.Z;
        }

    }

}
