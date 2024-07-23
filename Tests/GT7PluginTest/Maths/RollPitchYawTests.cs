using Microsoft.VisualStudio.TestTools.UnitTesting;
using YawVR_Game_Engine.Plugin;
using System;
//using YawGEAPI;
using Quaternion = System.Numerics.Quaternion;

namespace GT7PluginTest
{
    [TestClass]
    public class RollPitchYawTests
    {
        private const double Tolerance = 0.01;

        [TestMethod]
        public void TestIdentityQuaternion()
        {
            var identityQuat = new Quaternion(0, 0, 0, 1);
            var (yaw, pitch, roll) = identityQuat.ToEuler();
            Assert.AreEqual(0, roll, Tolerance);
            Assert.AreEqual(0, pitch, Tolerance);
            Assert.AreEqual(0, yaw, Tolerance);
        }

        [TestMethod]
        public void Test90DegreeRotationAroundX()
        {
            var quat = new Quaternion(0.7071f, 0, 0, 0.7071f); // 90 degrees around X
            var (yaw, pitch, roll) = quat.ToEuler();
            Assert.AreEqual(0, roll, Tolerance);
            Assert.AreEqual(-90, pitch, Tolerance);
            Assert.AreEqual(0, yaw, Tolerance);
        }

        [TestMethod]
        public void Test90DegreeRotationAroundY()
        {
            var quat = new Quaternion(0, 0.7071f, 0, 0.7071f); // 90 degrees around Y
            var (yaw, pitch, roll) = quat.ToEuler();
            Assert.AreEqual(0, roll, Tolerance);
            Assert.AreEqual(0, pitch, Tolerance);
            Assert.AreEqual(90, yaw, Tolerance);
        }

        [TestMethod]
        public void Test90DegreeRotationAroundZ()
        {
            var quat = new Quaternion(0, 0, 0.7071f, 0.7071f); // 90 degrees around Z
            var (yaw, pitch, roll) = quat.ToEuler();
            Assert.AreEqual(90, roll, Tolerance);
            Assert.AreEqual(0, pitch, Tolerance);
            Assert.AreEqual(0, yaw, Tolerance);
        }

        [TestMethod]
        public void TestNormalizationEffect()
        {
            var quat = new Quaternion(0, 0, 0, 2); // Non-unit quaternion
            var (yaw, pitch, roll) = quat.ToEuler();
            Assert.AreEqual(0, roll, Tolerance);
            Assert.AreEqual(0, pitch, Tolerance);
            Assert.AreEqual(0, yaw, Tolerance);
        }
    }
}
