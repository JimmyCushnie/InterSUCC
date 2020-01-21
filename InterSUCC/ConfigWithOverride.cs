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
    public class ConfigWithOverride<TData> : ConfigWithOverride<TData, TData> where TData : class
    {
        public ConfigWithOverride(string masterFilePath, string masterFileDefaultText = null, string overrideFilesDefaultText = null) : base(masterFilePath, masterFileDefaultText, overrideFilesDefaultText)
        {
        }
    }

    public class ConfigWithOverride<TMasterData, TOverrideData>
        where TOverrideData : class
        where TMasterData : class, TOverrideData
    {
        private DataFile<TMasterData> MasterFile { get; }
        private DataFile<TOverrideData> OverrideFile { get; set; }
        private string OverrideFilesDefaultText { get; }

        public string FileName { get; }
        public TMasterData Data { get; }

        public TOverrideData Override => OverrideFile?.Data;
        public bool OverrideExists => Override != null;

        public ConfigWithOverride(string masterFilePath, string masterFileDefaultText = null, string overrideFilesDefaultText = null)
        {
            MasterFile = new DataFile<TMasterData>(masterFilePath, defaultFileText: masterFileDefaultText);
            this.FileName = MasterFile.FileName;
            this.OverrideFilesDefaultText = overrideFilesDefaultText;

            Data = GenerateDataObject();
        }

        public void SetOverrideDirectory(string directory)
        {
            string path = Path.Combine(directory, FileName + "_override");
            OverrideFile = new DataFile<TOverrideData>(path, defaultFileText: OverrideFilesDefaultText);
        }

        public void RemoveOverride()
        {
            OverrideFile = null;
        }


        private static TMasterData ImplDataCache;
        private TMasterData GenerateDataObject()
        {
            if (!typeof(TMasterData).IsInterface)
                throw new Exception($"{nameof(TMasterData)} must be an interface type");

            if (ImplDataCache == null)
            {
                var impl = new Implementer<TMasterData>(this.GetType());

                foreach (var prop in impl.Properties)
                {
                    if (prop.GetMethod != null)
                    {
                        impl.Getter(prop).Callback(o =>
                        {
                            var files = o["__data"] as ConfigWithOverride<TMasterData, TOverrideData>;

                            if (files.OverrideExists && files.OverrideFile.KeyExists(prop.Name))
                            {
                                return files.OverrideFile.GetNonGeneric(
                                    type: prop.PropertyType,
                                    key: prop.Name,
                                    defaultValue: prop.PropertyType.GetDefaultValue());
                            }
                            else
                            {
                                return prop.GetMethod.Invoke(files.MasterFile.Data, null);
                            }
                        });
                    }

                    if (prop.SetMethod != null)
                    {
                        impl.Setter(prop, (value, data) =>
                        {
                            var files = (ConfigWithOverride<TMasterData, TOverrideData>)data;
                            prop.SetMethod.Invoke(files.MasterFile.Data, new object[] { value });
                        });
                    }
                }

                ImplDataCache = impl.Finish();
            }

            return ClassUtils.Copy(ImplDataCache, this);
        }
    }
}
