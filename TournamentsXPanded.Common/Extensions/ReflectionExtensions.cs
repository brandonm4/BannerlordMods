using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TournamentsXPanded.Common.Extensions
{
   public static class ReflectionExtensions
    {
        public static T ReflectionGetFieldValue<T>(this object obj, string name)
        {
            FieldInfo field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return (T)((field != null ? field.GetValue(obj) : null));
        }          
        public static void ReflectionSetFieldValue<T>(this object obj, string name, T value)
        {
            FieldInfo property = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
            {
                property.SetValue(obj, value);
            }
        }
        public static T ReflectionGetPropertyValue<T>(this object obj, string name)
        {
            PropertyInfo field = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return (T)((field != null ? field.GetValue(obj) : null));
        }
        public static void ReflectionSetPropertyValue<T>(this object obj, string name, T value)
        {
            PropertyInfo property = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
            {
                property.SetValue(obj, value);
            }
        }        
    }
}
