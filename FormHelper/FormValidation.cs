using FormHelper.Properties;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Net.Mime.MediaTypeNames;

namespace FormHelper
{
    internal class FormValidation
    {
        private ErrorProvider ep;

        public FormValidation(Form form)
        {
            
            this.ep = new ErrorProvider();
            ep.ContainerControl = form;
        }

        public bool Validate(Control ctl,CancelEventArgs e, string validationExpression, string errorMessage = null)
        {
            if (ctl != null)
            {
                
                var val = ctl.Value() + "";
                if (validationExpression != null)
                {
                    if (!Regex.IsMatch(val, validationExpression, RegexOptions.IgnoreCase))
                    {
                        ep.SetError(ctl, errorMessage ?? "Invalid");
                        e.Cancel = true;
                    }
                }
            }
            return !e.Cancel;
        }

        public void Required(object sender, CancelEventArgs e)
        {
            var ctl = (Control)sender;
            if (string.IsNullOrWhiteSpace(ctl.Text))
            {
                ep.SetError(ctl, "Required");
                e.Cancel = true;
            }
        }

        public void MustBeNumber(object sender, CancelEventArgs e)
        {
            var ctl = (Control)sender;
            if (!int.TryParse(ctl.Text, out _))
            {
                ep.SetError(ctl, "Must be a number");
                e.Cancel = true;
            }
        }

        public void FileExists(object sender, CancelEventArgs e)
        {
            var ctl = (Control)sender;
            if (!File.Exists(ctl.Text))
            {
                ep.SetError(ctl, "File does not exist");
                e.Cancel = true;
            }
        }

        public void FolderExists(object sender, CancelEventArgs e)
        {
            var ctl = (Control)sender;
            if (!Directory.Exists(ctl.Text))
            {
                ep.SetError(ctl, "Folder does not exist");
                e.Cancel = true;
            }
        }

        public void IPAddress(object sender, CancelEventArgs e)
        {
            var ctl = (Control)sender;
            if (Regex.Match(ctl.Text, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Success)
            {
                ep.SetError(ctl, "Invalid Ip Address");
                e.Cancel = true;
            }
        }
    }
}
