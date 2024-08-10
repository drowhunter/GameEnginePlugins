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
            
            settings.Add(new UserSetting
            {
                DisplayName = "Udp Forwarding",
                Name = "forwardingEnabled",
                SettingType = SettingType.Bool,
                Value = false
            });
            settings.Add(new UserSetting
            {
                DisplayName = $"Udp Forwarding Port",
                Name = "forwardingPort",
                SettingType = SettingType.String,
                Value = null,
                ValidationRegex = @"\d{1,5}",
                ValidationEnabledWhen = new Dictionary<string, string> { 
                    { "forwardingEnabled", "true" } 
                },
                EnabledWhen = new Dictionary<string, string> { 
                    { "forwardingEnabled", "true" } 
                }
            });

            settings.Add(new UserSetting
            {
                DisplayName = $"IP Address",
                Name = "ip",
                SettingType = SettingType.String,
                Value = "255.255.255.255",
                ValidationRegex = @"^(\d{1,3}\.){3}\d{1,3}$"

            });

            settings.Add(new UserSetting
            {
                DisplayName = "Game Folder",
                Name = "gamefolder",
                SettingType = SettingType.Directory,
                Value = @"C:\Program Files (x86)\Steam\Steamlibrary\steamapps\common\Overload",
                EnabledWhen = new Dictionary<string, string> { 
                    { "forwardingEnabled", "true" } 
                }
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
