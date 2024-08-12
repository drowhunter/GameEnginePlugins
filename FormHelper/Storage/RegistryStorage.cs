using FormHelper.Properties;
using Microsoft.Win32;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FormHelper.Storage
{
    public class RegistryStorage : IUserSettingStorage
    {
        private string _registryKeyPath => "Software\\YawVR\\PluginSettings";



        public string PluginName { get; }

        public RegistryStorage(string pluginName)
        {
            PluginName = pluginName;
        }

        public Task SaveAsync(Dictionary<string, object> settings, bool overwrite = false, CancellationToken cancellationToken = default)
        {

            //if (overwrite)
            //{
            //    await DeleteAllAsync(cancellationToken);
            //}
            try
            {
                using (var pluginKey = CreateRegistryKeyIfNotExists())
                {
                    if (pluginKey != null)
                    {
                        pluginKey.SetValue(PluginName, JsonConvert.SerializeObject(settings));
                        //foreach (var kvp in settings)
                        //{
                        //    try
                        //    {
                        //        pluginKey.SetValue(kvp.Key, kvp.Value ?? new byte[0]);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Console.WriteLine($"Error setting {kvp.Key} - {kvp.Value} - {ex.Message}");
                        //        throw;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving settings to registry for plugin " + PluginName + " - " + ex.Message);
                //throw;
                return Task.FromException(ex);
            }

            return Task.CompletedTask;
        }

        public Task<Dictionary<string, object>> LoadAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Loading settings from registry for plugin " + PluginName);

            Dictionary<string, object> retval = null;
            try
            {
                using (var pluginKey = Registry.CurrentUser.OpenSubKey(_registryKeyPath))
                {
                    if (pluginKey != null)
                    {
                        //retval = pluginKey.GetValueNames().ToDictionary(k => k, v => pluginKey.GetValue(v));                    
                        var json = pluginKey.GetValue(PluginName) as string;
                        if (json != null)
                            retval = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading settings from registry for plugin " + PluginName + " - " + ex.Message);
                //throw;
                return Task.FromException<Dictionary<string, object>>(ex);
            }

            return Task.FromResult(retval ?? new Dictionary<string,object>());
        }

        public Task DeleteAllAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Deleting settings from registry for plugin " + PluginName);

            using (var pluginKey = Registry.CurrentUser.OpenSubKey(_registryKeyPath, true))
            {
                if (pluginKey != null)
                {
                    var k = pluginKey.GetValue(PluginName) as string;
                    if(k != null) 
                    {
                        pluginKey.DeleteValue(PluginName);
                    }
                }
            }
            
            return Task.CompletedTask;
        }

        private RegistryKey CreateRegistryKeyIfNotExists()
        {

            var key = Registry.CurrentUser.OpenSubKey(_registryKeyPath, true);

            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(_registryKeyPath);
                Console.WriteLine("Created registry key: " + _registryKeyPath);
            }

            return key;
        }

        
    }
}
