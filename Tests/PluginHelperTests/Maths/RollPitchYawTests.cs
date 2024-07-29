using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluginHelper;

using Quaternion = System.Numerics.Quaternion;

namespace PluginHelperTests
{
    [TestClass]
    public class RollPitchYawTests
    {
        private const double Tolerance = 0.01;

        [TestMethod]
        public void TestIdentityQuaternion()
        {
            // Arrange
            var identityQuat = new Quaternion(0, 0, 0, 1);

            // Act
            var (pitch, yaw, roll) = Maths.ToEuler(identityQuat);

            // Assert
            Assert.AreEqual(0, pitch, Tolerance);
            Assert.AreEqual(0, yaw, Tolerance);
            Assert.AreEqual(0, roll, Tolerance);
        }

        [TestMethod]
        public void Test90DegreeRotationAroundX()
        {
            // Arrange
            var quat = new Quaternion(0.7071f, 0, 0, 0.7071f); // 90 degrees around X

            // Act
            var (pitch, yaw, roll) = Maths.ToEuler(quat);

            // Assert
            Assert.AreEqual(90, pitch, Tolerance);
            Assert.AreEqual(0, yaw, Tolerance);
            Assert.AreEqual(0, roll, Tolerance);
            
            
        }

        [TestMethod]
        public void Test90DegreeRotationAroundY()
        {
            // Arrange
            var quat = new Quaternion(0, 0.7071f, 0, 0.7071f); // 90 degrees around Y

            // Act
            var (pitch, yaw, roll) = Maths.ToEuler(quat);

            // Assert
            
            Assert.AreEqual(0, pitch, Tolerance);
            Assert.AreEqual(90, yaw, Tolerance);
            Assert.AreEqual(0, roll, Tolerance);
        }

        [TestMethod]
        public void Test90DegreeRotationAroundZ()
        {
            // Arrange
            var quat = new Quaternion(0, 0, 0.7071f, 0.7071f); // 90 degrees around Z

            // Act
            var (pitch, yaw, roll) = Maths.ToEuler(quat);

            // Assert
           
            Assert.AreEqual(0, pitch, Tolerance);
            Assert.AreEqual(0, yaw, Tolerance);
            Assert.AreEqual(90, roll, Tolerance);
        }

        [TestMethod]
        public void TestNormalizationEffect()
        {
            // Arrange
            var quat = new Quaternion(0, 0, 0, 2); // Non-unit quaternion

            // Act
            var (pitch, yaw, roll) = Maths.ToEuler(quat);

            // Assert
            Assert.AreEqual(0, pitch, Tolerance);
            Assert.AreEqual(0, yaw, Tolerance);
            Assert.AreEqual(0, roll, Tolerance);
        }
    }
}
