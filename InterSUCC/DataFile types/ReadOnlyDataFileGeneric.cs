using System;
using SUCC;
using SUCC.Abstractions;
using ClassImpl;

namespace InterSUCC
{
    public class ReadOnlyDataFile<TData> : ReadOnlyDataFile where TData : class
    {
        public TData Data { get; }


        public ReadOnlyDataFile(string path, string defaultFileText = null, bool autoReload = false) : base(path, defaultFileText, autoReload)
        {
            this.Data = DataUtility<TData>.GenerateDataObject(this);
        }
    }
}
