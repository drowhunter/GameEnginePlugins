using Microsoft.VisualStudio.TestTools.UnitTesting;

using FormHelper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PluginHelperTests
{
    [TestClass]
    public class FormTests
    {
        [TestMethod]
        public async Task PluginShouldInit()
        {
            // Arrange
            var pluginName = "TestPlugin";
            var settings = new List<UserSetting>();
            //Enumerable.Range(1,2).Select(i => new UserSetting
            //{
            //    DisplayName = $"Test Setting {i}",
            //    Name = $"TestSetting{i}",
            //    SettingType = SettingType.String,
            //    Value = $"Test Value {i}"
            //}).ToList();
           
            settings.Add(new UserSetting
            {
                DisplayName = "Udp Forwarding",
                Name = "udp",
                SettingType = SettingType.Bool,
                Value = false
            });
            settings.Add(new UserSetting
            {
                DisplayName = $"Udp Port",
                Name = "port",
                SettingType = SettingType.Number,
                Value = null
            });
            settings.Add(new UserSetting
            {
                DisplayName = $"IP Address",
                Name = "ip",
                SettingType = SettingType.IPAddress,
                Value = "255.255.255.255"
            });

            settings.Add(new UserSetting
            {
                DisplayName = "Game Folder",
                Name = "gamefolder",
                SettingType = SettingType.Directory,
                Value = @"C:\Program Files (x86)\Steam\Steamlibrary\steamapps\common\Overload"
            });


            var userSettingsManager = new UserSettingsManager(pluginName);

            // Act
            await userSettingsManager.InitAsync(settings);

            var val = userSettingsManager.Get<string>("TestSetting");


            // Assert
            Assert.IsNotNull(val);
            Assert.AreNotEqual("Test Value", val);

        }
    }
}
