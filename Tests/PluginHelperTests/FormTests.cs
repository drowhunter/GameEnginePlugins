using Microsoft.VisualStudio.TestTools.UnitTesting;

using FormHelper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Testing.Extensions.TrxReport.Abstractions;

namespace PluginHelperTests
{
    [TestClass]
    public class FormTests
    {

        public FormTests()
        {
                
        }


        [TestMethod]
        public async Task PluginShouldInit()
        {
            // Arrange
            var pluginName = "TestPlugin";

            var userSettingsManager = new UserSettingsManager<RegistrySettingsStorage>(pluginName);

            // Act
            await userSettingsManager.LoadAsync(defaultSettings);
            
            

            userSettingsManager.OnSettingsChanged += (s, e) =>
            {
                var val = userSettingsManager.Get<string>("ip");
                // Assert
                Assert.IsNotNull(val);
            };


            await userSettingsManager.ShowFormAsync(defaultSettings.Select(_ => _.Name).ToList());
            

        }

        List<UserSetting> defaultSettings = new List<UserSetting>
        {
            new UserSetting
            {
                DisplayName = "Udp Forwarding",
                Name = "forwardingEnabled",
                Description = "Enable UDP Forwarding",
                SettingType = SettingType.Bool,
                Value = false
            },
            new UserSetting
            {
                DisplayName = $"Udp Forwarding Port",
                Name = "forwardingPort",
                Description = "Port to forward UDP packets to.",
                SettingType = SettingType.NetworkPort,
                Value = null,
                ValidationEnabled = true,
                ValidationEnabledWhen = new Dictionary<string, string> { { "forwardingEnabled", "true" } },
                EnabledWhen = new Dictionary<string, string> { { "forwardingEnabled", "true" } }

            },
            new UserSetting
            {
                DisplayName = $"Console IP Address",
                Name = "ip",
                Description = "IP Address of the Playstation. (use 255.255.255.255 for auto discovery)",
                SettingType = SettingType.IPAddress,
                Value = "255.255.255.255",
                ValidationEnabled = true,
            },
            new UserSetting
            {
                DisplayName = "Game Folder",
                Name = "gamefolder",
                Description = "Path to the game folder",
                SettingType = SettingType.Directory,
                Value = @"C:\Program Files (x86)\Steam\Steamlibrary\steamapps\common\Overload",
                ValidationEnabled = true
            }
        };
    }
}
