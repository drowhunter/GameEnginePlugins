using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FormHelper
{
    public class RegistrySettingsStorage: IUserSettingStorage
    {
        public string PluginName { get; set; }

        public RegistrySettingsStorage()
        {
            
        }

        public Task<IEnumerable<UserSetting>> LoadAsync( CancellationToken cancellationToken = default)
        {
           if (string.IsNullOrEmpty(PluginName))
            {
                throw new InvalidOperationException("PluginName is not set");
            }

           Console.WriteLine("Loading settings from registry for plugin " + PluginName);

            return Task.FromResult((IEnumerable<UserSetting>)new List<UserSetting>());
        }

        public Task SaveAsync(IEnumerable<UserSetting> settings, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(PluginName))
            {
                throw new InvalidOperationException("PluginName is not set");
            }
            Console.WriteLine("Saving "+ Newtonsoft.Json.JsonConvert.SerializeObject(settings) +" to registry for plugin " + PluginName);
            return Task.CompletedTask;
        }
    }
}
