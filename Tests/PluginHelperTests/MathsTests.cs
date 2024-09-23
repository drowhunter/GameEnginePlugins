using PluginHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

namespace PluginHelper.Tests
{
    [TestClass()]
    public class MathsTests
    {
        [TestMethod()]
        public void EulerToDirectionTest()
        {
            var result = Maths.EulerToDirection(0, 0, 0);

            Assert.AreEqual(0, result.X, 0.0001f);
            Assert.AreEqual(0, result.Y, 0.0001f);
            Assert.AreEqual(1, result.Z, 0.0001f);
        }

        [TestMethod()]
        public void EulerToDirectionTest2()
        {
            var result = Maths.EulerToDirection(0, 90, 0);



            Assert.AreEqual(1, result.X, 0.0001f);
            Assert.AreEqual(0, result.Y, 0.0001f);
            Assert.AreEqual(0, result.Z, 0.0001f);
        }

        [TestMethod()]
        public void WorldToLocal2Test()
        {
            Vector3 someVEctor = new Vector3(-1, 0, 0);


            var q = Quaternion.Normalize( new Quaternion(someVEctor, 0));

            var l =Maths.WorldtoLocal(q, new Vector3(0,1,1));




            //var localFwd = Maths.WorldToLocal2(someVEctor);

            Assert.IsNotNull(q);



        }

        //[TestMethod()]
        //public void EulerToDirectionTest3()
        //{
        //    var result = Maths.EulerToDirection(0, 1.4f, 0, false);


        //    Assert.AreEqual(0, result.Y, 0.0001f);

        //    Assert.IsTrue(result.X > 1f);

        //}


    }
}