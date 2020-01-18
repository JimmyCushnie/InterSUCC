using System;
using SUCC;
using SUCC.Abstractions;
using ClassImpl;

namespace InterSUCC
{
    internal static class Extensions
    {
        public static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }
    }
}
