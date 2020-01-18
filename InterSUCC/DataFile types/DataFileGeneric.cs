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


        public DataFile(string path, bool autoSave = true, bool autoReload = false, string defaultFileText = null) : this(path, FileStyle.Default, autoSave, autoReload, defaultFileText) 
        { 
        }

        public DataFile(string path, FileStyle style, bool autoSave = true, bool autoReload = false, string defaultFileText = null) : base(path, style, autoSave, autoReload, defaultFileText)
        {
            this.Data = DataUtility<TData>.GenerateDataObject(this);
        }
    }
}
