using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormHelper
{
    internal static class Extensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }

        public static object Value(this Control ctl)
        {
            return ctl.GetType().GetProperty(ctl.GetNameOfValueProperty()).GetValue(ctl);
        }

        public static T Value<T>(this Control ctl)
        {
            return (T) Value(ctl);
        }

        public static void SetValue(this Control ctl, object value)
        {
            var prop = ctl.GetType().GetProperty(ctl.GetNameOfValueProperty());

            if (prop.PropertyType != value.GetType())
            {
                value = Convert.ChangeType(value, prop.PropertyType);
            }

            prop.SetValue(ctl, value);            
        }

        public static void Subscribe(this Control ctl, EventHandler handler)
        {
            var p = ctl.GetNameOfChangedProperty();
            ctl.GetType().GetEvent(p).AddEventHandler(ctl, handler);
        }

        public static void Unsubscribe(this Control ctl, EventHandler handler)
        {
            ctl.GetType().GetEvent(ctl.GetNameOfChangedProperty()).RemoveEventHandler(ctl, handler);
        }

        public static EventHandler Changed(this Control ctl)
        {
            return (EventHandler)ctl.GetType().GetProperty(ctl.GetNameOfChangedProperty()).GetValue(ctl);
        }

        public static string GetNameOfValueProperty(this Control ctl)
        {
            var controlValueProperties = new Dictionary<Type, string>
            {
                { typeof(TextBox), nameof(TextBox.Text) },
                { typeof(CheckBox), nameof(CheckBox.Checked) },
                { typeof(ComboBox), nameof(ComboBox.SelectedValue) },
                { typeof(NumericUpDown), nameof(NumericUpDown.Value)},
                { typeof(DateTimePicker), nameof(DateTimePicker.Value) }
            };

            if (controlValueProperties.TryGetValue(ctl.GetType(), out var propertyName))
            {
                return propertyName;
            }

            throw new NotImplementedException();
        }

        public static string GetNameOfChangedProperty(this Control ctl)
        {
            var controlChangedProperties = new Dictionary<Type, string>
            {
                { typeof(TextBox), nameof(TextBox.TextChanged) },
                { typeof(CheckBox), nameof(CheckBox.CheckedChanged) },
                { typeof(ComboBox), nameof(ComboBox.SelectedValueChanged) },
                { typeof(NumericUpDown), nameof(NumericUpDown.ValueChanged)},
                { typeof(DateTimePicker), nameof(DateTimePicker.ValueChanged) }
            };

            if (controlChangedProperties.TryGetValue(ctl.GetType(), out var propertyName))
            {
                return propertyName;
            }

            throw new NotImplementedException();
        }

    }
}
