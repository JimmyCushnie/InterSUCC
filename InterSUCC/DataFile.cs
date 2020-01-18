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
        public TData Data { get; }


        public DataFile(string path, string defaultFileText = null, bool autoSave = true, bool autoReload = false) : this(path, FileStyle.Default, defaultFileText, autoSave, autoReload) { }

        public DataFile(string path, FileStyle style, string defaultFileText = null, bool autoSave = true, bool autoReload = false) : base(path, style, defaultFileText, autoSave, autoReload)
        {
            this.Data = DataUtility<TData>.GenerateDataObject(this);
        }
    }
}
