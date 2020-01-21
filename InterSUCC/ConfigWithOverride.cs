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
    public class ConfigWithOverride<TBaseData, TOverrideData>
        where TOverrideData : class
        where TBaseData : class, TOverrideData
    {
        private DataFile<TBaseData> BaseFile { get; }
        private DataFile<TOverrideData> OverrideFile { get; set; }
        private string OverrideFilesDefaultText { get; }

        public string FileName { get; }
        public TOverrideData Data { get; }

        public TOverrideData Override => OverrideFile?.Data;

        public ConfigWithOverride(string basePath, string baseFileDefaultText = null, string overrideFilesDefaultText = null)
        {
            BaseFile = new DataFile<TBaseData>(basePath, defaultFileText: baseFileDefaultText);
            this.FileName = BaseFile.FileName;
            this.OverrideFilesDefaultText = overrideFilesDefaultText;

            Data = GenerateDataObject();
        }

        public void SetOverrideDirectory(string directory)
        {
            string path = Path.Combine(directory, FileName + "_override");
            OverrideFile = new DataFile<TOverrideData>(path, defaultFileText: OverrideFilesDefaultText);
        }


        private static TOverrideData ImplDataCache;
        private TOverrideData GenerateDataObject()
        {
            if (!typeof(TOverrideData).IsInterface)
                throw new Exception($"{nameof(TOverrideData)} must be an interface type");

            if (ImplDataCache == null)
            {
                var impl = new Implementer<TOverrideData>(this.GetType());

                foreach (var prop in impl.Properties)
                {
                    if (prop.GetMethod != null)
                    {
                        impl.Getter(prop).Callback(o =>
                        {
                            var files = o["__data"] as ConfigWithOverride<TBaseData, TOverrideData>;

                            if (files.OverrideFile.KeyExists(prop.Name))
                            {
                                return files.OverrideFile.GetNonGeneric(
                                    type: prop.PropertyType,
                                    key: prop.Name,
                                    defaultValue: prop.PropertyType.GetDefaultValue());
                            }
                            else
                            {
                                return files.BaseFile.GetNonGeneric(
                                    type: prop.PropertyType,
                                    key: prop.Name,
                                    defaultValue: prop.PropertyType.GetDefaultValue());
                            }
                        });
                    }

                    if (prop.SetMethod != null)
                    {
                        impl.Setter(prop, (value, data) =>
                        {
                            var files = (ConfigWithOverride<TBaseData, TOverrideData>)data;
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
