using System;
using System.ComponentModel;
using System.Reflection;

namespace LoyaltyCard.App.Helpers
{
    public static class EnumExtensions
    {
        public static string DisplayName(this Enum value)
        {
            Type valueType = value.GetType();
            // Search DescriptionAttribute
            FieldInfo fi = valueType.GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
                return attributes[0].Description;
            // ToString()
            return value.ToString();
        }
    }
}
