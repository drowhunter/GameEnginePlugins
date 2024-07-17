using System;
using System.Numerics;

using YawGEAPI;

using Quaternion = System.Numerics.Quaternion;

namespace YawVR_Game_Engine.Plugin
{
    //internal class Vector3
    //{
    //    public double x;
    //    public double y;
    //    public double z;

    //    public Vector3(double x, double y, double z)
    //    {
    //        this.x = x;
    //        this.y = y;
    //        this.z = z;
    //    }

    //    override public string ToString()
    //    {
    //        return $"({x}, {y}, {z})";
    //    }
    //}


    internal static class Maths
    {
        public static Quaternion ConvertQuat(this YawGEAPI.Quaternion q)
        {
            return new Quaternion((float)q.x, (float)q.y, (float)q.z, (float)q.w);
        }

        public static float RadianToDegree(float rad)
        {
            return (float) (rad * (180.0f / Math.PI));
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

        public static (float roll, float pitch, float yaw) roll_pitch_yaw(Quaternion q)
        {

            var q_norm = Quaternion.Normalize(q);




            // Compute the roll, pitch, and yaw angles in radians
            var loc_roll = (float) Math.Atan2(2 * (q_norm.X * q_norm.Y + q_norm.W * q_norm.Z), 1.0 - 2.0 * (q_norm.X * q_norm.X + q_norm.Z * q_norm.Z));

            var loc_pitch = (float)Math.Asin(2 * (q_norm.W * q_norm.X - q_norm.Y * q_norm.Z));

            var loc_yaw = (float) Math.Atan2(2 * (q_norm.X * q_norm.Z + q_norm.W * q_norm.Y), 1.0 - 2.0 * (q_norm.X * q_norm.X + q_norm.Y * q_norm.Y));

            // Convert the angles from radians to degrees
            var roll_deg = RadianToDegree(loc_roll);
            var pitch_deg = RadianToDegree(loc_pitch);
            var yaw_deg = RadianToDegree(loc_yaw);

            return (-roll_deg, -pitch_deg, -yaw_deg);

        }

        public static (float roll, float pitch, float yaw) roll_pitch_yawge(Quaternion q)
        {

            //var q_norm = q.Normalize();
            var q_norm = new YawGEAPI.Quaternion(q.X, q.Y, q.Z, q.W);



            // Compute the roll, pitch, and yaw angles in radians
            var loc_roll = (float) q_norm.toRollFromYUp();

            var loc_pitch = (float) q_norm.toPitchFromYUp();

            var loc_yaw = (float) q_norm.toRollFromYUp();

            // Convert the angles from radians to degrees
            var roll_deg = RadianToDegree(loc_roll);
            var pitch_deg = RadianToDegree(loc_pitch);
            var yaw_deg = RadianToDegree(loc_yaw);

            return (roll_deg, -pitch_deg, yaw_deg);

        }

        public static Vector3 WorldVelocity_to_LocalVelocity(Quaternion q, Vector3 vw)
        {

            Quaternion q_conj = Quaternion.Conjugate(q);

            // Convert quaternion to rotation matrix
            var r = Matrix4x4.CreateFromQuaternion(q_conj);            

            var retval = Vector3.Transform(vw, r);

            return retval;

            //var R = r.as_matrix();
            // Calculate local velocity vector
            //v_local = np.dot(R, v_world)
            //return v_local
        }

        


        public static Vector3 WorldVelocity_to_LocalVelocity_Quat(Quaternion q, Vector3 v_world)
        {
            //var q_norm= Quaternion.Normalize(q);
           

            Quaternion q_conj = Quaternion.Conjugate(q);

            Quaternion l_world = q * new Quaternion(v_world, 0) * q_conj;


            var v_local = new Vector3(l_world.X, l_world.Y, l_world.Z);

            return v_local;
            //var R = r.as_matrix();
            // Calculate local velocity vector
            //v_local = np.dot(R, v_world)


        }

    }

}
