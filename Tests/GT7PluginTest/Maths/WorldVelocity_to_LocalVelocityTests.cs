using Microsoft.VisualStudio.TestTools.UnitTesting;
using YawVR_Game_Engine.Plugin;
using System;
using Quaternion = System.Numerics.Quaternion;
using System.Numerics;
using System.Security.AccessControl;


namespace GT7PluginTest
{

    [TestClass]
    public class WorldVelocity_to_LocalVelocityTests
    {
        float cos (float deg) =>  (float) Math.Cos(Maths.DegToRad(deg) / 2);
        float sin (float deg) =>  (float) Math.Sin(Maths.DegToRad(deg) / 2);

        [TestMethod]
        public void TestIdentityQuaternion()
        {
            var identityQuat = new Quaternion(0, 0, 0, 1);
            var velocity = new Vector3(1, 2, 3);
            var result = Maths.WorldtoLocal(identityQuat, velocity);
            Assert.AreEqual(velocity.X, result.X);
            Assert.AreEqual(velocity.Y, result.Y);
            Assert.AreEqual(velocity.Z, result.Z);
        }

        [TestMethod]
        public void TestZeroVector()
        {
            var quat = new Quaternion(0, 0.7071f, 0, 0.7071f); // 90 degrees around Y
            var velocity = new Vector3(0, 0, 0);
            var result = Maths.WorldtoLocal(quat, velocity);
            Assert.AreEqual(0, result.X);
            Assert.AreEqual(0, result.Y);
            Assert.AreEqual(0, result.Z);
        }

        [TestMethod]
        public void Test180DegreeRotation()
        {
            var quat = new Quaternion(0, 1, 0, 0); // 180 degrees around Y
            var velocity = new Vector3(1, 0, 0); // Should end up pointing opposite
            var result = Maths.WorldtoLocal(quat, velocity);
            Assert.IsTrue(Math.Abs(result.X + 1) < 0.0001);
            Assert.IsTrue(Math.Abs(result.Y) < 0.0001);
            Assert.IsTrue(Math.Abs(result.Z) < 0.0001);
        }

        [TestMethod]
        public void Test90DegreeRotation()
        {
            /*
             a = angle to rotate
            [x, y, z] = axis to rotate around (unit vector)

            R = [cos(a/2), sin(a/2)*x, sin(a/2)*y, sin(a/2)*z]
             */
            var a = 90;
           

            Vector3 axis = new Vector3(0, 1, 0);

            var R = axis.ToQuat(a);

            var world_velocity = new Vector3(1, 0, 0); // Should end up pointing along x


            var result = Maths.WorldtoLocal(R, world_velocity);

            Assert.IsTrue(Math.Abs(result.Z - 1) < 0.0001);
            Assert.IsTrue(Math.Abs(result.X) < 0.0001);
            Assert.IsTrue(Math.Abs(result.Y) < 0.0001);
        }

        

    }

}