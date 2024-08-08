using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormHelper
{
    internal interface IUserSettingStorage
    {
        string PluginName { get; }
        Task Save(IEnumerable<UserSetting> settings);

        Task<IEnumerable<UserSetting>> Load();

    }
}
