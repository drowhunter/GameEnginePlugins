using Microsoft.VisualStudio.TestTools.UnitTesting;
using YawVR_Game_Engine.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YawVR_Game_Engine.Plugin.Tests
{
    [TestClass()]
    public class OverloadPluginTests
    {
        [TestMethod()]
        public void PatchGameTest()
        {
            var plugin = new OverloadPlugin();

            plugin.PatchGame();
        }
    }
}