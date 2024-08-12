using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace FormHelper
{
    public class SettingsStorage<T> where T:IUserSettingStorage
    {
        public string PluginName { get; private set; }

        IUserSettingStorage _storage;

        public SettingsStorage(string pluginName)
        {
            PluginName = pluginName;
            _storage = _storage = (IUserSettingStorage)Activator.CreateInstance(typeof(T), pluginName);

        }

        public async Task<List<string>> LoadAsync(List<UserSetting> settings, CancellationToken cancellationToken = default)
        {
            var settingsDict = await _storage.LoadAsync(cancellationToken);
            List<string> changed = new List<string>();
            if (settingsDict.Any())
            {
                foreach (var setting in settings)
                {
                    if (settingsDict.TryGetValue(setting.Name, out object value))
                    {
                        if (setting.Value +"" != value + "")
                        {
                            changed.Add(setting.Name);
                        }
                        setting.Value = value;
                        
                    }
                }
            }

            return changed;
        }

        public async Task SaveAsync(List<UserSetting> settings, bool overwrite = false, CancellationToken cancellationToken = default)
        {            
            await _storage.SaveAsync(settings.ToDictionary(_ => _.Name, _ => _.Value), overwrite, cancellationToken);            
        }

        

        
    }
}
