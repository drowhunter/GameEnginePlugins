using Newtonsoft.Json;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;





namespace FormHelper
{
    public enum SettingType
    {
        String,
        IPAddress,
        NetworkPort,
        Number,
        Bool,
        File,
        Directory
    }

    //public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    //{
    //    return source.Select((item, index) => (item, index));
    //}
    [DebuggerDisplay("{Name}( {SettingType}) = {Value}")]
    public class UserSetting : INotifyPropertyChanged
    {
        private object _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public string DisplayName { get; set; }

        public string Name { get; set; }

        public SettingType SettingType { get; set; }

        /// <summary>
        /// Enable Validation for this setting, ,will validate based on the SettingType
        /// </summary>
        public bool ValidationEnabled { get; set; }

        public string ValidationRegex { get; set; }


        public object Value
        {
            get => _value; 
            set
            {
                if(_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description { get; set; }

        public Dictionary<string, string> ValidationEnabledWhen { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> EnabledWhen { get; set; } = new Dictionary<string, string>();
        

        public UserSetting Clone()
        {
            return JsonConvert.DeserializeObject<UserSetting>(JsonConvert.SerializeObject(this));
        }
    }
}
