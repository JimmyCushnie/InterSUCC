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
    public class MemoryDataFile<TData> : MemoryDataFile where TData : class
    {
        public TData Data { get; }


        public MemoryDataFile() : this(string.Empty) { }

        public MemoryDataFile(string rawFileText, bool autoSave = true) : this(rawFileText, FileStyle.Default, autoSave) { }

        public MemoryDataFile(string rawFileText, FileStyle style, bool autoSave = true) : base(rawFileText, style, autoSave)
        {
            this.Data = DataUtility<TData>.GenerateDataObject(this);
        }
    }
}
