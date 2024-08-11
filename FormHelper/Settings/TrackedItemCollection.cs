using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;





namespace FormHelper
{

    internal class TrackedItem
    {
        public string Name { get; set; }
        public Control Control { get; set; }
        public UserSetting Setting { get; set; }

        public List<Control> OtherControls = new List<Control>();

        public void Deconstruct(out (string name, Control control, UserSetting setting, List<Control> other) o)
        {
            o = (Name, Control, Setting, OtherControls);
        }


    }

    
    internal class TrackedItemCollection : List<TrackedItem>, IDisposable
    {
        public TrackedItemCollection()
        {
                
        }
        public TrackedItemCollection(IEnumerable<TrackedItem> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
                if(item.Control != null)
                {
                    item.Control.Subscribe(changeHandler);
                }
            }
                
        }

        public void AddTrackedControl(string name, Control control)
        {
            var i = this.FindIndex(_ => _.Name == name);
            if (i < 0)
            {
                this.Add(new TrackedItem { Name = name, Control = control, Setting = null });
                control.Subscribe(changeHandler);
            }
            else
            {
                if (this[i].Control != null)
                {
                    if (this[i].Control.GetHashCode() != control.GetHashCode())
                    {
                        this[i].Control.Unsubscribe(changeHandler);

                        this[i].Control = control;

                        control.Subscribe(changeHandler);
                        //this[i].Setting = this[i].Setting ?? setting;
                    }
                }
                else
                {
                    this[i].Control = control;
                    control.Subscribe(changeHandler);
                }
            }
        }

        public TrackedItem this[string name]
        {
            get
            {
                return this.Single(_ => _.Name == name);
            }

            //set
            //{
            //    var idx = this.FindIndex(_ => _.Name == name);
            //    if (idx >= 0)
            //    {
            //        this[idx] = value;
            //    }
            //    else
            //    {
                        
            //        this.Add(value);
            //    }
            //}
        }

        public IEnumerable<Control> Controls => this.Select(_ => _.Control);
                
        public IEnumerable<UserSetting> Settings => this.Select(_ => _.Setting);

        public delegate void ValueChangedHandler (Control sender, EventArgs e);

        public event ValueChangedHandler OnValueChanged ;

        private void changeHandler(object sender, EventArgs e) {
            var changedControl = (Control)sender;

            if (this.OnValueChanged != null)
            {
                OnValueChanged.Invoke((Control)sender, e);
            }                
        }

        public void Dispose()
        {
            foreach(var c in this.Controls)
            {
                c.Unsubscribe(changeHandler);                    
            }

                
        }
    }
    
}
