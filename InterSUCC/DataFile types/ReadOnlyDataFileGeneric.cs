using System;
using SUCC;
using SUCC.Abstractions;
using ClassImpl;

namespace InterSUCC
{
    public class ReadOnlyDataFile<TData> : ReadOnlyDataFile where TData : class
    {
        public TData Data { get; }


        public ReadOnlyDataFile(string path, string defaultFileText = null) : base(path, defaultFileText)
        {
            this.Data = DataUtility<TData>.GenerateDataObject(this);
        }
    }
}
