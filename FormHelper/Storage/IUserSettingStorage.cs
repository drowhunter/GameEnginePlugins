using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FormHelper
{
    public interface IUserSettingStorage
    {
        /// <summary>
        /// This must be set before calling Save or Load
        /// </summary>
        string PluginName { get; set; }

        Task SaveAsync(IEnumerable<UserSetting> settings, CancellationToken cancellationToken = default);

        Task<IEnumerable<UserSetting>> LoadAsync(CancellationToken cancellationToken = default);

    }

    
}
