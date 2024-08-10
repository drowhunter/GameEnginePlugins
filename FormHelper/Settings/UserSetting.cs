using Newtonsoft.Json;

using System.Collections.Generic;
using System.Diagnostics;





namespace FormHelper
{
    public enum SettingType
    {
        String,
        //IPAddress,
        //Number,
        Bool,
        File,
        Directory
    }

    //public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    //{
    //    return source.Select((item, index) => (item, index));
    //}
    [DebuggerDisplay("{Name}( {SettingType}) = {Value}")]
    public class UserSetting
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public SettingType SettingType { get; set; }

        public object Value { get; set; }

        public string Description { get; set; }

        public string ValidationRegex { get; set; }

        public Dictionary<string, string> ValidationEnabledWhen { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> EnabledWhen { get; set; } = new Dictionary<string, string>();
        public string ErrorMessage { get; set; }

        public UserSetting Clone()
        {
            return JsonConvert.DeserializeObject<UserSetting>(JsonConvert.SerializeObject(this));
        }
    }
}
