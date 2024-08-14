using FormHelper.Properties;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormHelper
{
    public class UserSettingsManager<TStrorage> where TStrorage : IUserSettingStorage
    {
        private string _pluginName;

        private List<UserSetting> _defaultSettings = null;

        private List<UserSetting> _settings = null;

        private SettingsStorage<TStrorage> _storage;


        public delegate void SettingsChangedEventHandler(object sender, List<UserSetting> changed);
        
        public event SettingsChangedEventHandler OnSettingsChanged;

        public UserSettingsManager(string pluginName)
        {
            
            _pluginName = pluginName;
            _storage = new SettingsStorage<TStrorage>(pluginName);            
        }

        public async Task LoadAsync(IEnumerable<UserSetting> defaultSettings, CancellationToken cancellationToken = default)
        {
            _defaultSettings = defaultSettings.ToList();

            _settings = defaultSettings.Select(_ => _.Clone()).ToList();
            var changed = await _storage.LoadAsync(_settings, cancellationToken);
            
            await _storage.SaveAsync(_settings, true, cancellationToken);
            
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

        public Task ResetToDefaults(List<UserSetting> settings, CancellationToken cancellationToken = default)
        {
            (from n in settings
             join o in _defaultSettings on n.Name equals o.Name
             where n.Value?.ToString() != o.Value?.ToString()
             select (o, n)).ToList().ForEach(_ => _.n.Value = _.o.Value);


            
            return Task.CompletedTask;
        }


        /// <summary>
        /// Show the settings form
        /// </summary>
        /// <param name="settings">the names of the settings to show on the form</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ShowFormAsync(List<string> settings = null, CancellationToken cancellationToken = default)
        {
            if (settings == null)
            {
                settings = _settings.Select(_ => _.Name).ToList();
            }

            var trackedStuff = new TrackedItemCollection(settings.Select(_ => new TrackedItem { Name = _, Control = null, Setting = _settings.Single(s => s.Name == _).Clone() }));
            

            trackedStuff.OnValueChanged += (changedControl, e) => {

                RefreshControls(trackedStuff, changedControl);
            };



            //Height= settings.Count * 50 + 100,
            using (Form form = new Form() { Anchor = AnchorStyles.None, Width = 500, Height = 400, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowOnly,   Text = "Settings for " + _pluginName, FormBorderStyle = FormBorderStyle.Sizable  })
            {
                var err = new ErrorProvider();
                err.ContainerControl = form;

                var requiresValidation = new List<Control>();

                #region Validation
                form.FormClosing += (s, e) =>
                {
                    if (form.DialogResult == DialogResult.OK)
                    {

                        var itemsRequiringValidation = trackedStuff.Where(_ => _.Setting.ValidationEnabled && _.Setting.Name == _.Name).ToList();

                        foreach (var itm in itemsRequiringValidation)
                        {
                            var ctl = itm.Control;
                            var setting = itm.Setting;

                            //var ctl = requiresValidation.Find(_ => _.Name == setting.Name); 
                            
                            if (ctl != null && ctl.Enabled)
                            {
                                bool mustValidate = true;


                                if (setting.ValidationEnabledWhen != null)
                                {
                                    var deps = setting.ValidationEnabledWhen.Select(_ => new Tuple<string, Control>(_.Value, form.Controls.Find(_.Key, true).FirstOrDefault()));

                                    mustValidate = deps.All(_ => {

                                        var (rgx, cntl) = _;

                                        return Regex.IsMatch(cntl.Value() + "", rgx, RegexOptions.IgnoreCase);
                                    });

                                }

                                if (mustValidate)
                                {
                                    try
                                    {
                                        Validator.Validate(setting, ctl.Value());
                                    }
                                    catch (Exception ex)
                                    {
                                        err.SetError(ctl, ex.Message);
                                        e.Cancel = true;
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


                
                

                var save    = new Button()  { Text = "&Save"  ,  Top = form.Bottom - 80,  Anchor = AnchorStyles.None , DialogResult = DialogResult.OK };               
                save.Left = form.Width / 2 - save.Width ;
                save.Click += (s, e) => form.Close();
                gb.Controls.Add(save);

                var reset= new Button() { Text = "&Reset", Top = form.Bottom - 80, Anchor = AnchorStyles.None, DialogResult = DialogResult.None };
                reset.Left = form.Width / 2 + 20;//- reset.Width / 2;
                reset.Click += (s, e) => ResetToDefaults(trackedStuff.Settings);
                gb.Controls.Add(reset);

                

                for (var i=0;i < trackedStuff.Settings.Count; i++)
                {
                    var setting = trackedStuff.Settings[i];
                    var pnlSetting = new FlowLayoutPanel() { 
                        FlowDirection = FlowDirection.LeftToRight, 
                        Dock = DockStyle.Top, 
                        Height = 50, 
                        Padding = new Padding(5), 
                        Margin = new Padding(5), 
                        BackColor = Color.LightGray,
                        
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink
                    };

                    gb.Controls.Add(pnlSetting);

                    pnlSetting.BringToFront();

                    Control newControl = null;
                    
                    switch (setting.SettingType)
                    {
                        case SettingType.String: 
                        case SettingType.Number:
                        case SettingType.IPAddress:
                        case SettingType.NetworkPort:
                        {
                            pnlSetting.Controls.Add(LabelWithTooltip(setting));
                            newControl = CreateControl<TextBox>(setting, string.Empty);
                            pnlSetting.Controls.Add(newControl);
                        }
                        break;
                        
                        case SettingType.Bool:
                        {
                            pnlSetting.Controls.Add(LabelWithTooltip(setting));
                            newControl = CreateControl<CheckBox>(setting, false);                                
                            pnlSetting.Controls.Add(newControl);
                        }
                        break;
                        case SettingType.File:
                        {
                            pnlSetting.Controls.Add(LabelWithTooltip(setting));
                            newControl = CreateControl<TextBox>(setting, string.Empty);
                            pnlSetting.Controls.Add(newControl);
                            pnlSetting.SetFlowBreak(newControl, true);

                            var btn = new Button() { Text = "&Browse" };
                            pnlSetting.Controls.Add(btn);

                            trackedStuff[setting.Name].OtherControls.Add(btn);

                            btn.Click += (s, e) => {
                                using (var ofd = new OpenFileDialog())
                                {
                                    ofd.FileName = newControl.Value<string>();
                                    ofd.CheckFileExists = true;

                                    if (File.Exists(ofd.FileName))
                                        ofd.InitialDirectory = Path.GetDirectoryName(ofd.FileName);


                                    if (ofd.ShowDialog() == DialogResult.OK)
                                    {
                                        setting.Value = ofd.FileName;
                                    }

                                }
                            };
                        }
                        break;
                        case SettingType.Directory:
                            {
                                pnlSetting.Controls.Add(LabelWithTooltip(setting));
                                newControl = CreateControl<TextBox>(setting, string.Empty);
                                newControl.Width = pnlSetting.Width - 10;


                                pnlSetting.Controls.Add(newControl);

                                pnlSetting.SetFlowBreak(newControl, true);

                                var btn = new Button() { Text = "&Browse" };
                                pnlSetting.Controls.Add(btn);
                                trackedStuff[setting.Name].OtherControls.Add(btn);


                                btn.Click += (s, e) =>
                                {
                                    using (var fb = new FolderBrowserDialog())
                                    {

                                        fb.SelectedPath = (string)newControl.Value();

                                        if (Directory.Exists(newControl.Text))
                                            fb.SelectedPath = newControl.Text;

                                        if (fb.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fb.SelectedPath))
                                        {
                                            setting.Value = fb.SelectedPath;
                                        }
                                    }
                                };
                            }
                            break;
                    }

                    if (newControl != null)
                    {
                        newControl.EnabledChanged += (s, e) => {
                          var ctl = (Control)s;
                            if(ctl.Enabled)
                            {
                                ctl.BackColor = Color.White;
                                ctl.ForeColor = Color.Black;
                            }
                            else
                            {
                                ctl.ForeColor = Color.LightGray;
                                ctl.BackColor = Color.DarkGray;
                            }
                        };
                        
                        trackedStuff.AddTrackedControl(setting.Name, newControl);
                    }
                    
                }

                RefreshControls(trackedStuff);

                var result = form.ShowDialog();

                
                if (result == DialogResult.OK)
                {
                    
                    if (trackedStuff.Settings.Any())
                    {
                        var changed = (from n in trackedStuff.Settings
                                       join o in _settings on n.Name equals o.Name
                        where n.Value?.ToString() != o.Value?.ToString()
                        select (o,n)).ToList();
                        
                        
                        foreach (var (oldSetting,newSetting) in changed)
                        {                            
                            Console.WriteLine($"{oldSetting.Name} changed from {oldSetting.Value} to {newSetting.Value}");
                            oldSetting.Value = newSetting.Value;
                        }

                        await _storage.SaveAsync(_settings, cancellationToken: cancellationToken);

                        if (changed.Any())
                        {
                            OnSettingsChanged?.Invoke(this, changed.Select(_=>_.n).ToList());
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine("No changes");
                    }
                }
                else
                {
                    Console.WriteLine("User cancelled the form");
                }


            }

            
        }

        private void Control_ValueChanged(object sender, EventArgs e)
        {
            var textBox1 = (Control)sender;
            if (sender is TextBox)
            { 
                Size size = TextRenderer.MeasureText(textBox1.Text, textBox1.Font);
                textBox1.Width = size.Width;
                textBox1.Height = size.Height;
            }
        }

        private Label LabelWithTooltip(UserSetting setting)
        {
            var lbl = new Label() { Text = setting.DisplayName, TextAlign = ContentAlignment.MiddleLeft };
            AddTooltip(lbl, setting.Description);
            return lbl;
        }

        private T CreateControl<T>(UserSetting setting, object defaultvalue) where T:Control, new()
        {
            var tb = new T() { Name = setting.Name };
            try
            {
                tb.SetValue(setting.Value ?? defaultvalue);
                tb.DataBindings.Add(tb.GetNameOfValueProperty(), setting, nameof(UserSetting.Value));
                //tb.TextChanged += Control_ValueChanged;

                tb.GetType().GetEvent(tb.GetNameOfChangedProperty()).AddEventHandler(tb, new EventHandler(Control_ValueChanged));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Error creating control for {setting.Name}={setting.Value} - {ex.Message}", ex);
            }

            return tb;
        }

        

        private void AddTooltip(Control ctl, string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var tooltip = new ToolTip() { ToolTipTitle = "Info", ToolTipIcon = ToolTipIcon.Info, ShowAlways = true };
                tooltip.SetToolTip(ctl, text);
            }
        }




        public object this[string name]
        {
            get
            {
                return _settings.FirstOrDefault(s => s.Name == name)?.Value;
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
