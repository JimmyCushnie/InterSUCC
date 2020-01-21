using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SUCC;
using SUCC.Abstractions;
using ClassImpl;

namespace InterSUCC
{
    internal static class DataUtility<TData>
    {
        private static TData ImplDataCache;

        internal static TData GenerateDataObject(object dataObject)
        {
            if (!typeof(TData).IsInterface)
                throw new Exception($"{nameof(TData)} must be an interface type");

            if (ImplDataCache == null)
            {
                var impl = new Implementer<TData>(dataObject.GetType());

                foreach (var prop in impl.Properties)
                {
                    if (prop.GetMethod != null)
                    {
                        impl.Getter(prop).Callback(o =>
                        {
                            var file = o["__data"] as ReadableDataFile;

                            return file.GetNonGeneric(
                                type: prop.PropertyType,
                                key: prop.Name,
                                defaultValue: prop.PropertyType.GetDefaultValue());
                        });
                    }

                    if (prop.SetMethod != null)
                    {
                        impl.Setter(prop, (value, data) =>
                        {
                            var file = (ReadableWritableDataFile)data;
                            file?.SetNonGeneric(
                                type: prop.PropertyType,
                                key: prop.Name,
                                value);
                        });
                    }
                }

                ImplDataCache = impl.Finish();
            }

            return ClassUtils.Copy(ImplDataCache, dataObject);
        }
    }
}
