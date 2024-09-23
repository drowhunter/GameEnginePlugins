using Microsoft.VisualStudio.TestTools.UnitTesting;
using YawVR_Game_Engine.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDUSCPlugin;

namespace YawVR_Game_Engine.Plugin.Tests
{
    [TestClass()]
    public class TDUSCPluginTests
    {
        [TestMethod()]
        public void GetInputDataTest()
        {
            var plugin = new TDUSCPlugin();

            var inputs = plugin.GetInputData();

            Assert.AreEqual(inputs.Length, 19);
        }

        [TestMethod]
        public void PatchGameShouldWork()
        {
            // Arrange
            var plugin = new YawVR_Game_Engine.Plugin.TDUSCPlugin();

            // Act
            try
            {
                plugin.PatchGame();
            }
            finally
            {
                // Assert
                Assert.Fail("This test should not throw an exception");
            }
        }


        [TestMethod]
        public void ReadTelemetryShouldWork()
        {
            // Arrange
            var plugin = new YawVR_Game_Engine.Plugin.TDUSCPlugin();

            // Act
            try
            {
                plugin.Init();
                //plugin.ReadFunction();
            }
            catch (Exception x)
            {

            }
            finally
            {
                plugin.Exit();
                // Assert
                Assert.Fail("This test should not throw an exception");
            }
        }

        [TestMethod()]
        public void InitTest()
        {
            var plugin = new TDUSCPlugin();

            plugin.Init();

            Console.ReadLine();

            //while (true)
            //{
            //    plugin.ReadFunction();
            //}
            Assert.IsTrue(true);
        }
    }
}