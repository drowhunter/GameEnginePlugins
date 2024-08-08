using Microsoft.VisualBasic;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormHelper
{
    public enum SettingType 
    {        
        String,
        Number,
        Bool,
        File,
        Directory        
    }
    //public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    //{
    //    return source.Select((item, index) => (item, index));
    //}
    public class UserSetting
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public SettingType SettingType { get; set; }

        public object Value { get; set; }

        public UserSetting Clone()
        {
            return JsonConvert.DeserializeObject<UserSetting>(JsonConvert.SerializeObject(this));
        }
    }
    


    public class UserSettingsManager
    {
        private string _pluginName;

        private List<UserSetting> _settings = null;

        private IUserSettingStorage _storage;

        public UserSettingsManager(string pluginName)
        {
            
            _pluginName = pluginName;
            _storage = new RegistrySettingsStorage(pluginName);


            
        }

        public async Task InitAsync(IEnumerable<UserSetting> defaultSettings, CancellationToken cancellationToken = default)
        {
            

            var settings = ( await _storage.Load()).ToList();

            foreach (var s in settings)
            {
                var ds = defaultSettings.SingleOrDefault(_ => _.Name == s.Name);

                if (ds != null) { 
                    ds.Value = s.Value;
                }
            }

            _settings = defaultSettings.ToList();

            await ShowFormAsync(defaultSettings.Select(s => s.Name).ToList() , cancellationToken);

            
        }

        public async Task ShowFormAsync(List<string> settings, CancellationToken cancellationToken = default)
        {
            

            using (Form form = new Form() { Width = 400, Height= settings.Count * 50 + 100,  Text = "Settings for " + _pluginName, FormBorderStyle = FormBorderStyle.Sizable, Dock = DockStyle.Fill  })
            {
                List<UserSetting> selectedSettings = new List<UserSetting>();

                var save    = new Button()  { Text = "&OK"      , Anchor = AnchorStyles.Bottom | AnchorStyles.Right , DialogResult = DialogResult.OK };
                var cancel  = new Button()  { Text = "&Cancel"  , Anchor = AnchorStyles.Bottom | AnchorStyles.Right, DialogResult = DialogResult.Cancel };

                
                //save.Top = form.Height - save.Height - 20;
                cancel.Top = save.Top;
                //save.Left = form.Width - save.Width - 10;
                cancel.Left = save.Right + 10;

                save.Click += (s, e) => form.Close();
                cancel.Click += (s, e) => form.Close();

                form.Controls.Add(save);

                form.FormClosed += (s, e) =>
                {
                    Console.WriteLine("close reason : " + e.CloseReason.ToString());
                    if (e.CloseReason == CloseReason.UserClosing)
                    {
                        Console.WriteLine("User closed the form");

                        //selectedSettings.Clear();
                    }
                    //else
                    //{
                    //    Console.WriteLine("User clicked save");
                    //    foreach (var setting in selectedSettings)
                    //    {
                    //        var ss = _settings.Single(_ => _.Name == setting.Name).Value;

                    //    }

                    //};
                };

                selectedSettings = _settings.Where(_ => settings.Contains(_.Name)).Select(_ => _.Clone()).ToList();


                for (var i = 0; i < selectedSettings.Count; i++)
                {
                    var setting = selectedSettings[i];


                    switch (setting.SettingType)
                    {
                        case SettingType.String:
                            //setting.Value = Interaction.InputBox("Enter the host address of Assetto Corsa\nLeave default value if its running on this PC", "Endpoint", "127.0.0.1");
                            form.Controls.Add(new Label() { Text = setting.DisplayName, Top = 10 + (i * 50), Left = 10 });

                            var tb = new TextBox() { Name = setting.Name, Text = setting.Value.ToString(), Top = 10 + (i * 50), Left = 100, Width = 200, TextAlign = HorizontalAlignment.Left };
                            tb.DataBindings.Add("Text", setting, "Value");
                            form.Controls.Add(tb);

                            break;
                        case SettingType.Number:
                            break;
                        case SettingType.Bool:
                            break;
                        case SettingType.File:
                            break;
                        case SettingType.Directory:
                            break;
                    }


                }

                try
                {

                    var result = form.ShowDialog();

                    if(result == DialogResult.OK)
                    {
                        foreach (var setting in selectedSettings)
                        {
                            var ss = _settings.Single(_ => _.Name == setting.Name);

                            ss.Value = setting.Value;

                        }

                        await _storage.Save(selectedSettings);
                    }
                    else
                    {
                        Console.WriteLine("User cancelled the form");
                    }

                }
                catch (Exception x)
                {

                    Console.WriteLine(x.Message);
                }



            }

            
        }

        public T Get<T>(string name)
        {
            var setting = _settings.FirstOrDefault(s => s.Name == name)?.Value;
            return (T) setting;
        }

        override public string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(_settings);
        }
    }
}
