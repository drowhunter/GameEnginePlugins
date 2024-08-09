using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;





namespace FormHelper
{
    public enum SettingType 
    {        
        String,
        IPAddress,
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

            //Height= settings.Count * 50 + 100,
            using (Form form = new Form() { Width = 400, Height = 400, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowOnly,   Text = "Settings for " + _pluginName, FormBorderStyle = FormBorderStyle.Sizable  })
            {
                var fv = new FormValidation(form);

                //var ep = new ErrorProvider();
                //ep.ContainerControl = form;


                var pnl = new Panel() {  AutoScroll = true, Dock = DockStyle.Fill, Padding = new Padding(5), Margin = new Padding(5) , BorderStyle = BorderStyle.FixedSingle };
                form.Controls.Add(pnl);

                var gb = new GroupBox() {  Text = "Settings", Dock = DockStyle.Fill, Padding = new Padding(5), Margin = new Padding(5) };
                pnl.Controls.Add(gb);


                

                var save    = new Button()  { Text = "&OK"  ,  Top = form.Bottom - 80,  Anchor = AnchorStyles.None , DialogResult = DialogResult.OK };               
                save.Left = form.Width / 2 - save.Width / 2;

                save.Click += (s, e) => form.Close();
                

                gb.Controls.Add(save);


                var selectedSettings = _settings.Where(_ => settings.Contains(_.Name)).Select(_ => _.Clone());


                foreach (var (setting,i)  in selectedSettings.WithIndex())
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

                    
                    switch (setting.SettingType)
                    {
                        case SettingType.String:
                            {
                                pnlSetting.Controls.Add(new Label() { Text = setting.DisplayName, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });

                                var tb = new TextBox() { Name = setting.Name, Text = setting.Value?.ToString() };
                                tb.DataBindings.Add("Text", setting, "Value");
                                

                                tb.Validating += fv.Required;

                                pnlSetting.Controls.Add(tb);

                            }
                            break;
                        case SettingType.Number:
                            {
                                pnlSetting.Controls.Add(new Label() { Text = setting.DisplayName, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });

                                var tb = new TextBox() { Name = setting.Name, Text = setting.Value?.ToString(), CausesValidation = true,  };
                                tb.Validating += fv.MustBeNumber;

                                tb.DataBindings.Add("Text", setting, "Value");
                                pnlSetting.Controls.Add(tb);

                            }
                            break;
                        case SettingType.Bool:
                            {
                                pnlSetting.Controls.Add(new Label() { Text = setting.DisplayName, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });

                                var cb = new CheckBox() { Name = setting.Name, Checked = (bool)(setting.Value ?? false) };
                                cb.DataBindings.Add("Checked", setting, "Value");
                                pnlSetting.Controls.Add(cb);
                            }
                            break;
                        case SettingType.File:
                            {
                                //var v = new GroupBox { Text = setting.DisplayName, Margin = new Padding(5), Dock = DockStyle.Fill };
                                //pnlSetting.Controls.Add(v);

                                
                                //pnlSetting.Controls.Add(new Label() { Text = setting.DisplayName, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });

                                var tb = new TextBox() { Name = setting.Name, Text = setting.Value?.ToString(), Enabled = false, Width = pnlSetting.Width - 10 };
                                tb.DataBindings.Add("Text", setting, "Value");
                                tb.Validating += fv.FileExists;

                                pnlSetting.Controls.Add(tb);

                                pnlSetting.SetFlowBreak(tb, true);

                                var btn = new Button() { Text = "&Browse" };
                                pnlSetting.Controls.Add(btn);

                                

                                btn.Click += (s, e) => {
                                    var ofd = new OpenFileDialog();
                                    ofd.FileName = tb.Text;
                                    ofd.CheckFileExists = true;

                                    if (File.Exists(tb.Text))
                                        ofd.InitialDirectory = Path.GetDirectoryName(tb.Text);
                                    
                                    
                                    if (ofd.ShowDialog() == DialogResult.OK)
                                    {
                                        tb.Text = ofd.FileName;
                                    }
                                };
                            }
                            break;
                        case SettingType.Directory:
                            {
                                var tb = new TextBox() { Name = setting.Name, Text = setting.Value?.ToString(), Enabled = false, Width = pnlSetting.Width - 10 };
                                tb.DataBindings.Add("Text", setting, "Value");
                                tb.Validating += fv.FolderExists;
                                tb.TextChanged += (s, e) => {
                                    var textBox1 = (Control)s;
                                    Size size = TextRenderer.MeasureText(textBox1.Text, textBox1.Font);
                                    textBox1.Width = size.Width;
                                    textBox1.Height = size.Height;
                                };
                                Size sz = TextRenderer.MeasureText(tb.Text, tb.Font);
                                tb.Width = sz.Width;
                                tb.Height = sz.Height;

                                pnlSetting.Controls.Add(tb);

                                pnlSetting.SetFlowBreak(tb, true);

                                var btn = new Button() { Text = "&Browse" };
                                pnlSetting.Controls.Add(btn);



                                btn.Click += (s, e) =>
                                {
                                    var fb = new FolderBrowserDialog();
                                    fb.SelectedPath = tb.Text;

                                    if (Directory.Exists(tb.Text))
                                        fb.SelectedPath = tb.Text;

                                    if (fb.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fb.SelectedPath))
                                    {
                                        tb.Text = fb.SelectedPath;
                                    }
                                };
                            }
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
