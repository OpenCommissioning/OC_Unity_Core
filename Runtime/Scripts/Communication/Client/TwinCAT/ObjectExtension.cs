using System;
using System.ComponentModel;

namespace OC.Communication.TwinCAT
{
    public static class ObjectExtension
    {
         /// <summary>
        /// Converts an object into a given type.
        /// </summary>
        public static T ConvertTo<T>(this object value)
        {
            if (value is T variable) return variable;

            try
            {
                // Value is hex string
                if (typeof(T) == typeof(byte[]) && value is string hex)
                {
                    return (T)Convert.ChangeType(hex.ToByteArray(), typeof(T));
                }
            }
            catch
            {
                return (T)Convert.ChangeType(new byte[1], typeof(T));
            }
            
            try
            {
                // Nullable types, e.g. int, double, bool ...
                if (Nullable.GetUnderlyingType(typeof(T)) != null)
                {
                    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }
        
        /// <summary>
        /// Converts an object into a given type and return as converted object.
        /// </summary>
        public static object ConvertTo(this object value, Type type)
        {
            if (value.GetType() == type) return value;
            
            try
            {
                // Value is hex string
                if (type == typeof(byte[]) && value is string hex)
                {
                    return Convert.ChangeType(hex.ToByteArray(), type);
                }
            }
            catch
            {
                return Convert.ChangeType(new byte[1], type);
            }
            
            try
            {
                // Nullable types, e.g. int, double, bool ...
                return Nullable.GetUnderlyingType(type) != null ? TypeDescriptor.GetConverter(type).ConvertFrom(value) : Convert.ChangeType(value, type);
            }
            catch
            {
                return Convert.ChangeType(0, type);
            }
        }
    }
}