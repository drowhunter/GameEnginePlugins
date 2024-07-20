using Microsoft.VisualStudio.TestTools.UnitTesting;
using YawVR_Game_Engine.Plugin;
using System;
using Quaternion = System.Numerics.Quaternion;
using System.Numerics;


namespace GT7PluginTest
{

    [TestClass]
    public class WorldVelocity_to_LocalVelocityTests
    {
        [TestMethod]
        public void TestIdentityQuaternion()
        {
            var identityQuat = new Quaternion(0, 0, 0, 1);
            var velocity = new Vector3(1, 2, 3);
            var result = Maths.WorldVelocity_to_LocalVelocity_Quat(identityQuat, velocity);
            Assert.AreEqual(velocity.X, result.X);
            Assert.AreEqual(velocity.Y, result.Y);
            Assert.AreEqual(velocity.Z, result.Z);
        }

        [TestMethod]
        public void TestZeroVector()
        {
            var quat = new Quaternion(0.7071f, 0.7071f, 0, 0); // 90 degrees around Y
            var velocity = new Vector3(0, 0, 0);
            var result = Maths.WorldVelocity_to_LocalVelocity_Quat(quat, velocity);
            Assert.AreEqual(0, result.X);
            Assert.AreEqual(0, result.Y);
            Assert.AreEqual(0, result.Z);
        }

        [TestMethod]
        public void Test180DegreeRotation()
        {
            var quat = new Quaternion(0, 1, 0, 0); // 180 degrees around Y
            var velocity = new Vector3(1, 0, 0); // Should end up pointing opposite
            var result = Maths.WorldVelocity_to_LocalVelocity_Quat(quat, velocity);
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
            Vector3 axis = new Vector3(0, 0, 1);

            var ca = (float)Math.Cos(a / 2);
            var sa = (float)Math.Sin(a / 2);
            
            var R = new Quaternion(ca, sa * axis.X, sa * axis.Y, sa * axis.Z); // 90 degrees around Z

            //var R = new Quaternion((float)Math.Cos(a / 2), (float)Math.Sin(a / 2), 0, 0); // 90 degrees around X
            var Rn = Quaternion.Normalize(R);

            var quat = new Quaternion(0, 0.7071f, 0, 0.7071f); // 90 degrees around Z
            var velocity = new Vector3(1, 0, 0); // Should end up pointing along Y
            var result = Maths.WorldVelocity_to_LocalVelocity_Quat(R, velocity);
            Assert.IsTrue(Math.Abs(result.Y - 1) < 0.0001);
            Assert.IsTrue(Math.Abs(result.X) < 0.0001);
            Assert.IsTrue(Math.Abs(result.Z) < 0.0001);
        }

        [TestMethod]
        public void TestNormalization()
        {
            var quat = new Quaternion(0, 0, 0, 2); // Non-unit quaternion, equivalent to identity after normalization
            var velocity = new Vector3(1, 2, 3);
            var result = Maths.WorldVelocity_to_LocalVelocity_Quat(quat, velocity);
            Assert.AreEqual(velocity.X, result.X);
            Assert.AreEqual(velocity.Y, result.Y);
            Assert.AreEqual(velocity.Z, result.Z);
        }

        const float kmtom = 1000 / 60f / 60f;

        [TestMethod]
        public void TestCalculateLateralForces()
        {
            var plugin = new YawVR_Game_Engine.Plugin.GT7Plugin();
            var g = 45;

                //convert g to radians
                var g_rad = g * Math.PI / 180;
            var result = plugin.CalculateLateralForces(112 * kmtom, 180 * kmtom, -0.25f);

            Assert.IsTrue(result.sway > 0);

            

            var result4 = plugin.CalculateLateralForces(112 * kmtom, 180 * kmtom, 0.25f);

            Assert.IsTrue(result4.sway < 0);

        }
    }

}