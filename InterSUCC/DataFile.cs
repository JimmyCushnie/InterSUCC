using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SUCC;
using SUCC.Abstractions;
using ClassImpl;

namespace InterSUCC
{
    public class DataFile<TData> : DataFile where TData : class
    {
        private static TData ImplDataCache;
        private static PropertyInfo[] DataProperties;

        public TData Data { get; }


        public DataFile(string path, string defaultFileText = null, bool autoSave = true, bool autoReload = false) : this(path, FileStyle.Default, defaultFileText, autoSave, autoReload) { }

        public DataFile(string path, FileStyle style, string defaultFileText = null, bool autoSave = true, bool autoReload = false) : base(path, style, defaultFileText, autoSave, autoReload)
        {
            if (!typeof(TData).IsInterface)
                throw new Exception($"{nameof(TData)} in {nameof(DataFile<TData>)} must be an interface type");

            if (ImplDataCache == null)
            {
                var impl = new Implementer<TData>(this.GetType());
                DataProperties = impl.Properties;

                foreach (var prop in DataProperties)
                {
                    if (prop.GetMethod != null)
                    {
                        impl.Getter(prop).Callback(o =>
                        {
                            var file = o["__data"] as DataFile<TData>;
                            return file.GetNonGeneric(
                                type: prop.PropertyType,
                                key: prop.Name,
                                DefaultValue: prop.PropertyType.GetDefaultValue());
                        });
                    }

                    if (prop.SetMethod != null)
                    {
                        impl.Setter(prop, (value, data) =>
                        {
                            var file = (DataFile<TData>)data;
                            file.SetNonGeneric(
                                type: prop.PropertyType,
                                key: prop.Name,
                                value);
                        });
                    }
                }

                ImplDataCache = impl.Finish();
            }

            this.Data = ClassUtils.Copy(ImplDataCache, this);
        }
    }
}
