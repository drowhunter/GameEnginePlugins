using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FormHelper
{
    internal class SettingsForm
    {
        public SettingsForm()
        {
            
        }
        public Task ShowFormAsync(List<string> settings, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
