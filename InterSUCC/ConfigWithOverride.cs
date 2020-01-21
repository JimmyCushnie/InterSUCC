using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SUCC;
using SUCC.Abstractions;
using System.IO;
using ClassImpl;

namespace InterSUCC
{
    public class ConfigWithOverride<TData> where TData : class
    {
        private DataFile<TData> BaseFile { get; }
        private DataFile<TData> OverrideFile { get; set; }
        private string OverrideFilesDefaultText { get; }

        public string FileName { get; }
        public TData Data { get; }
        public TData Override => OverrideFile?.Data;

        public ConfigWithOverride(string basePath, string baseFileDefaultText = null, string overrideFilesDefaultText = null)
        {
            BaseFile = new DataFile<TData>(basePath, defaultFileText: baseFileDefaultText);
            this.FileName = BaseFile.FileName;
            this.OverrideFilesDefaultText = overrideFilesDefaultText;

            Data = GenerateDataObject();
        }

        public void SetOverrideDirectory(string directory)
        {
            string path = Path.Combine(directory, FileName + "_override");
            OverrideFile = new DataFile<TData>(path, defaultFileText: OverrideFilesDefaultText);
        }


        private static TData ImplDataCache;
        private TData GenerateDataObject()
        {
            if (!typeof(TData).IsInterface)
                throw new Exception($"{nameof(TData)} must be an interface type");

            if (ImplDataCache == null)
            {
                var impl = new Implementer<TData>(this.GetType());

                foreach (var prop in impl.Properties)
                {
                    if (prop.GetMethod != null)
                    {
                        impl.Getter(prop).Callback(o =>
                        {
                            var files = o["__data"] as ConfigWithOverride<TData>;
                            DataFile<TData> appropriateFile;

                            if (files.OverrideFile.KeyExists(prop.Name))
                                appropriateFile = files.OverrideFile;
                            else
                                appropriateFile = files.BaseFile;

                            return appropriateFile.GetNonGeneric(
                                type: prop.PropertyType,
                                key: prop.Name,
                                defaultValue: prop.PropertyType.GetDefaultValue());
                        });
                    }

                    if (prop.SetMethod != null)
                    {
                        impl.Setter(prop, (value, data) =>
                        {
                            var files = (ConfigWithOverride<TData>)data;
                            files.BaseFile.SetNonGeneric(
                                type: prop.PropertyType,
                                key: prop.Name,
                                value);
                        });
                    }
                }

                ImplDataCache = impl.Finish();
            }

            return ClassUtils.Copy(ImplDataCache, this);
        }
    }
}
