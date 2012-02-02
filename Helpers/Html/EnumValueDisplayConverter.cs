using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Gizmo.Helpers.Html
{
    public class EnumValueDisplayConverter : TypeConverter
    {

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(String))
            {
                throw new InvalidOperationException("Can only convert to string");
            }

            Type typeOfValue = value.GetType();
            FieldInfo[] fields = typeOfValue.GetFields(BindingFlags.Public | BindingFlags.Static);
            FieldInfo info = fields.Single(x => x.Name == value.ToString());
            EnumValueDisplayAttribute attr = info.GetCustomAttributes(typeof(EnumValueDisplayAttribute), false).FirstOrDefault() as EnumValueDisplayAttribute;
            string desc = attr == null ? info.Name : attr.Name;

            return desc;
        }
    }
}