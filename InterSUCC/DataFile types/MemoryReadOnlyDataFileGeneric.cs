using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SUCC;
using SUCC.MemoryFiles;
using SUCC.Abstractions;
using ClassImpl;

namespace InterSUCC
{
    public class MemoryReadOnlyDataFile<TData> : MemoryReadOnlyDataFile where TData : class
    {
        public TData Data { get; }


        public MemoryReadOnlyDataFile() : this(string.Empty) { }

        public MemoryReadOnlyDataFile(string rawFileText) : base(rawFileText)
        {
            this.Data = DataUtility<TData>.GenerateDataObject(this);
        }
    }
}
