using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormHelper
{
    internal class RegistrySettingsStorage: IUserSettingStorage
    {
        public string PluginName { get; }

        public RegistrySettingsStorage(string pluginName)
        {
            PluginName = pluginName;
        }

        public Task<IEnumerable<UserSetting>> Load()
        {
            return Task.FromResult((IEnumerable<UserSetting>)new List<UserSetting>());
        }

        public Task Save(IEnumerable<UserSetting> settings)
        {
           Console.WriteLine("Saving settings to registry");
            return Task.CompletedTask;
        }
    }
}
