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

        public DataFile(string path, string defaultFileText = null) : base(path, defaultFileText)
        {
            this.Data = DataUtility<TData>.GenerateDataObject(this);
        }
    }
}
