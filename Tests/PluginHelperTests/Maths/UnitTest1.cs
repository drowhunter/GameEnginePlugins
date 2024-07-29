using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using PluginHelper;

namespace PluginHelperTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConvertToEurlerAndBack()
        {
            var deg = Math.PI / 2;

            var Qright90 = new System.Numerics.Quaternion((float)Math.Sin(deg / 2), 0, 0, (float) Math.Cos(deg/2));

            var (pitch,yaw,roll) = Qright90.ToEuler();


            Assert.AreEqual(pitch, 90, 0.0001f);
            Assert.AreEqual(yaw, 0, 0.0001f);            
            Assert.AreEqual(roll, 0, 0.0001f);


           
           
        }


    }
}
