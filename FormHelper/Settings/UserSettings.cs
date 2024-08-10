using FormHelper.Properties;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;





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

        public UserSetting Clone()
        {
            return JsonConvert.DeserializeObject<UserSetting>(JsonConvert.SerializeObject(this));
        }
    }
    


    public partial class UserSettingsManager
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

            var newSettings = ShowForm(defaultSettings.Select(s => s.Name));

            if (newSettings != null)
            {
                foreach (var setting in newSettings)
                {
                    var ss = _settings.Single(_ => _.Name == setting.Name);
                    if(ss.Value?.ToString() != setting.Value?.ToString())
                        ss.Value = setting.Value;

                }

                await _storage.Save(newSettings);
            }
            else
            {
                Console.WriteLine("User cancelled the form");
            }
        }



        private void RefreshControls(TrackedItemCollection items, Control changed = null)
        {
            foreach (var sett in items.Settings.Where(_ => _.EnabledWhen != null && _.EnabledWhen.Any() && (changed == null ? true : _.EnabledWhen.ContainsKey(changed.Name))))
            {
                items[sett.Name].Control.Enabled = sett.EnabledWhen.All(enabledKvp => Regex.IsMatch(items[enabledKvp.Key].Control.Value() + "", enabledKvp.Value, RegexOptions.IgnoreCase));

                foreach(var ctl in items[sett.Name].OtherControls)
                {
                    ctl.Enabled = items[sett.Name].Control.Enabled;
                }
            }
        }

        public List<UserSetting> ShowForm(IEnumerable<string> settings)
        {

            var trackedStuff = new TrackedItemCollection(settings.Select(_ => new TrackedItem { Name = _, Control = null, Setting = _settings.Single(s => s.Name == _).Clone() }));
            

            trackedStuff.OnValueChanged += (changedControl, e) => {

                RefreshControls(trackedStuff, changedControl);
            };



            //Height= settings.Count * 50 + 100,
            using (Form form = new Form() { Anchor = AnchorStyles.None, Width = 400, Height = 400, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowOnly,   Text = "Settings for " + _pluginName, FormBorderStyle = FormBorderStyle.Sizable  })
            {
                var fv = new FormValidation(form);
                var requiresValidation = new List<Control>();

                #region Validation
                form.FormClosing += (s, e) =>
                {
                    if (form.DialogResult == DialogResult.OK)
                    {

                        var settingsThatRequireValidation = trackedStuff.Settings.Where(_ => requiresValidation.Any(c => c.Name == _.Name)).ToList();

                        foreach (var setting in settingsThatRequireValidation)
                        {
                            var ctl = requiresValidation.Find(_ => _.Name == setting.Name); 
                            
                            if (ctl != null && ctl.Enabled)
                            {
                                bool mustValidate = true;


                                if (setting.ValidationEnabledWhen != null)
                                {
                                    var deps = setting.ValidationEnabledWhen.Select(_ => new Tuple<string, Control>(_.Value, form.Controls.Find(_.Key, true).FirstOrDefault()));

                                    mustValidate = deps.All(_ => {

                                        var (rgx, cntl) = _;

                                        var m = Regex.IsMatch(cntl.Value() + "", rgx, RegexOptions.IgnoreCase);
                                        return m;
                                    });

                                }

                                if (mustValidate)
                                {

                                    if(!fv.Validate(ctl, e, setting.ValidationRegex))
                                    {
                                        break;
                                    }

                                }
                            }
                        }                        
                    }
                };

                #endregion


                var pnl = new Panel() {  AutoScroll = true, Dock = DockStyle.Fill, Padding = new Padding(5), Margin = new Padding(5) , BorderStyle = BorderStyle.FixedSingle };
                form.Controls.Add(pnl);

                var gb = new GroupBox() {  Text = "Settings", Dock = DockStyle.Fill, Padding = new Padding(5), Margin = new Padding(5) };
                pnl.Controls.Add(gb);


                

                var save    = new Button()  { Text = "&OK"  ,  Top = form.Bottom - 80,  Anchor = AnchorStyles.None , DialogResult = DialogResult.OK };               
                save.Left = form.Width / 2 - save.Width / 2;

                save.Click += (s, e) => form.Close();
                
                gb.Controls.Add(save);


                


                foreach (var (setting,i)  in trackedStuff.Settings.WithIndex())
                {
                    
                    var pnlSetting = new FlowLayoutPanel() { 
                        FlowDirection = FlowDirection.LeftToRight, 
                        Dock = DockStyle.Top, 
                        Height = 50, 
                        Padding = new Padding(5), 
                        Margin = new Padding(5), 
                        BackColor = System.Drawing.Color.LightGray,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink
                    };

                    gb.Controls.Add(pnlSetting);

                    pnlSetting.BringToFront();

                    Control newControl = null;
                    
                    switch (setting.SettingType)
                    {
                        case SettingType.String:
                        //case SettingType.Number:
                            {
                                pnlSetting.Controls.Add(new Label() { Text = setting.DisplayName, TextAlign = ContentAlignment.MiddleLeft });

                                newControl = new TextBox() { Name = setting.Name };
                                newControl.SetValue(setting.Value?.ToString());
                                newControl.DataBindings.Add(newControl.GetNameOfValueProperty(), setting, nameof(UserSetting.Value));                               

                                if(setting.ValidationRegex != null)
                                {
                                    requiresValidation.Add(newControl);
                                }
                                
                                pnlSetting.Controls.Add(newControl);

                            }
                            break;
                        
                        case SettingType.Bool:
                            {
                                pnlSetting.Controls.Add(new Label() { Text = setting.DisplayName, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });

                                newControl = new CheckBox() { Name = setting.Name };
                                newControl.SetValue(setting.Value ?? false);
                                if(setting.ValidationRegex != null)
                                {
                                    requiresValidation.Add(newControl);
                                }

                                newControl.DataBindings.Add(newControl.GetNameOfValueProperty(), setting, nameof(UserSetting.Value));
                                pnlSetting.Controls.Add(newControl);
                            }
                            break;
                        case SettingType.File:
                            {
                                newControl = new TextBox() { Name = setting.Name, Enabled = false, Width = pnlSetting.Width - 10 };
                                newControl.SetValue(setting.Value);
                                newControl.DataBindings.Add(newControl.GetNameOfValueProperty(), setting, nameof(UserSetting.Value));
                                //tb.Validating += fv.FileExists;
                                
                               requiresValidation.Add(newControl);
                                
                               
                                pnlSetting.Controls.Add(newControl);

                                pnlSetting.SetFlowBreak(newControl, true);

                                var btn = new Button() { Text = "&Browse" };
                                pnlSetting.Controls.Add(btn);

                                trackedStuff[setting.Name].OtherControls.Add(btn);

                                btn.Click += (s, e) => {
                                    var ofd = new OpenFileDialog();
                                    ofd.FileName = newControl.Value<string>();
                                    ofd.CheckFileExists = true;

                                    if (File.Exists(ofd.FileName))
                                        ofd.InitialDirectory = Path.GetDirectoryName(ofd.FileName);
                                    
                                    
                                    if (ofd.ShowDialog() == DialogResult.OK)
                                    {
                                        newControl.SetValue(ofd.FileName);
                                    }
                                };
                            }
                            break;
                        case SettingType.Directory:
                            {
                                newControl = new TextBox() { Name = setting.Name, Enabled = false, Width = pnlSetting.Width - 10 };
                                newControl.SetValue(setting.Value?.ToString());
                                newControl.DataBindings.Add(newControl.GetNameOfValueProperty(), setting, nameof(UserSetting.Value));
                                //tb.Validating += fv.FolderExists;
                                requiresValidation.Add(newControl);

                                newControl.TextChanged += (s, e) => {
                                    var textBox1 = (Control)s;
                                    Size size = TextRenderer.MeasureText(textBox1.Text, textBox1.Font);
                                    textBox1.Width = size.Width;
                                    textBox1.Height = size.Height;
                                };
                                Size sz = TextRenderer.MeasureText(newControl.Text, newControl.Font);
                                newControl.Width = sz.Width;
                                newControl.Height = sz.Height;

                                pnlSetting.Controls.Add(newControl);

                                pnlSetting.SetFlowBreak(newControl, true);

                                var btn = new Button() { Text = "&Browse" };
                                pnlSetting.Controls.Add(btn);
                                trackedStuff[setting.Name].OtherControls.Add(btn);


                                btn.Click += (s, e) =>
                                {
                                    var fb = new FolderBrowserDialog();

                                    fb.SelectedPath = (string) newControl.Value();

                                    if (Directory.Exists(newControl.Text))
                                        fb.SelectedPath = newControl.Text;

                                    if (fb.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fb.SelectedPath))
                                    {
                                        newControl.SetValue(fb.SelectedPath);
                                    }
                                };
                            }
                            break;
                    }

                    if (newControl != null)
                    {
                        trackedStuff.AddTrackedControl(setting.Name, newControl);
                    }
                    
                }

                RefreshControls(trackedStuff);

                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    return trackedStuff.Settings.ToList();
                }
               
                return null;

            }

            
        }

        public T Get<T>(string name)
        {
            var setting = _settings.FirstOrDefault(s => s.Name == name)?.Value;
            return (T) setting;
        }

        override public string ToString()
        {
            return JsonConvert.SerializeObject(_settings);
        }
    }
}
