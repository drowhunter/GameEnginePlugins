using FormHelper;
using FormHelper.Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YawVR_Game_Engine.Plugin
{
    internal partial class DirtShowdownPlugin
    {
        private const string FORWARDING_ENABLED = "forwardingEnabled";
        private const string FORWARDING_PORT = "forwardingPort";
        private const string INCOMING_PORT = "incomingPort";

        private UserSettingsManager<RegistryStorage> _settings;
        private UserSettingsManager<RegistryStorage> settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new UserSettingsManager<RegistryStorage>(this.GetType().Name);
                    _settings.LoadAsync(defaultSettings, _cancellationTokenSource.Token).Wait();
                }

                return _settings;
            }
        }

        private CancellationTokenSource _cancellationTokenSource;
        private int _gameport;

        public DirtShowdownPlugin()
        {
            _cancellationTokenSource = new CancellationTokenSource();

        }

        List<UserSetting> defaultSettings = new List<UserSetting>
        {
            new UserSetting
            {
                DisplayName = "Udp Forwarding",
                Name = FORWARDING_ENABLED,
                Description = "Enable UDP Forwarding",
                SettingType = SettingType.Bool,
                Value = false
            },
            new UserSetting
            {
                DisplayName = $"Udp Forwarding Port",
                Name = FORWARDING_PORT,
                Description = "Port to forward UDP packets to.",
                SettingType = SettingType.NetworkPort,
                Value = 20778,
                ValidationEnabled = true,
                ValidationEnabledWhen = new Dictionary<string, string> { { "forwardingEnabled", "true" } },
                EnabledWhen = new Dictionary<string, string> { { "forwardingEnabled", "true" } }

            },
            new UserSetting
            {
                DisplayName = $"Udp Data Port",
                Name = INCOMING_PORT,
                Description = "The default port for incoming data. (Default: 20777)",
                SettingType = SettingType.NetworkPort,
                Value = 20777,
                ValidationEnabled = true

            }
        };
        private async Task PromptUserAsync(CancellationToken cancellationToken = default)
        {
            await settings.LoadAsync(defaultSettings, cancellationToken);

            await settings.ShowFormAsync(cancellationToken: cancellationToken);
        }

    }
}
