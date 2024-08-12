using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FormHelper
{
    public interface IUserSettingStorage
    {
        string PluginName { get; }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="settings">settings to save</param>
        /// <param name="overwrite">if true will clobber all existing settings before saving new ones</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveAsync( Dictionary<string,object> settings,bool overwrite = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Dictionary<string,object>> LoadAsync( CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAllAsync(CancellationToken cancellationToken = default);


    }    
}
