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
    /// <summary>
    /// Allows you to have a master config file with values that can be optionally overridden.
    /// </summary>
    /// <typeparam name="TData">The type of data used by the files</typeparam>
    public class ConfigWithOverride<TData> : ConfigWithOverride<TData, TData> where TData : class
    {
        /// <summary>
        /// Creates a new <see cref="ConfigWithOverride{TBaseData}"/>
        /// </summary>
        /// <param name="masterFilePath">The path of the master file, including file name</param>
        /// <param name="masterFileDefaultText">The default text to use for the master file if it doesn't exist already</param>
        /// <param name="overrideFilesDefaultText">The default text to use for override files if they don't exist already</param>
        public ConfigWithOverride(string masterFilePath, string masterFileDefaultText = null, string overrideFilesDefaultText = null) : base(masterFilePath, masterFileDefaultText, overrideFilesDefaultText)
        {
        }
    }

    /// <summary>
    /// Allows you to have a master config file with values that can be optionally overridden.
    /// </summary>
    /// <typeparam name="TMasterData">The type of data in the master file. It can have values that are not available for overrides.</typeparam>
    /// <typeparam name="TOverrideData">The type of data in override files.</typeparam>
    public class ConfigWithOverride<TMasterData, TOverrideData>
        where TOverrideData : class
        where TMasterData : class, TOverrideData
    {
        private DataFile<TMasterData> MasterFile { get; }
        private DataFile<TOverrideData> OverrideFile { get; set; }
        private string OverrideFilesDefaultText { get; }

        private string MasterFileName => MasterFile.FileName;
        private string OverrideFilesName => MasterFileName + "_override";

        /// <summary>
        /// When you get values in this, you will get the values from the master file unless there is an override for that value in the current override file. <para/>
        /// When you set values in this, you always set them in the master file.
        /// </summary>
        public TMasterData Data { get; }

        /// <summary>
        /// Use this to set override values.
        /// </summary>
        public TOverrideData Override => OverrideFile?.Data;
        
        /// <summary> Does this <see cref="ConfigWithOverride{TMasterData, TOverrideData}"/> have a set override file? </summary>
        public bool OverrideExists => Override != null;

        /// <summary>
        /// Creates a new <see cref="ConfigWithOverride{TBaseData, TOverrideData}"/>
        /// </summary>
        /// <param name="masterFilePath">The path of the master file, including file name</param>
        /// <param name="masterFileDefaultText">The default text to use for the master file if it doesn't exist already</param>
        /// <param name="overrideFilesDefaultText">The default text to use for override files if they don't exist already</param>
        public ConfigWithOverride(string masterFilePath, string masterFileDefaultText = null, string overrideFilesDefaultText = null)
        {
            MasterFile = new DataFile<TMasterData>(masterFilePath, defaultFileText: masterFileDefaultText);
            this.OverrideFilesDefaultText = overrideFilesDefaultText;

            Data = GenerateDataObject();
        }

        /// <summary>
        /// Sets an override file
        /// </summary>
        /// <param name="directory">The directory (NO file name) to put the override file in</param>
        public void SetOverrideDirectory(string directory)
        {
            string path = Path.Combine(directory, OverrideFilesName);
            OverrideFile = new DataFile<TOverrideData>(path, defaultFileText: OverrideFilesDefaultText);
        }

        /// <summary>
        /// Removes the override file from this <see cref="ConfigWithOverride{TMasterData, TOverrideData}"/> if there was one
        /// </summary>
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
