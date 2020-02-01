using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SUCC;
using SUCC.UnityStuff;
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


        /// <summary>
        /// Creates a new <see cref="DataFile{TData}"/> object, using a text file in Unity's Resources folder for the default file text.
        /// </summary>
        /// <param name="path"> The path of the file. Can be either absolute or relative to the default path. </param>
        /// <param name="defaultFile"> The path of the default file, relative to any Resources parent. </param>
        /// <returns></returns>
        public static new DataFile<TData> WithDefaultFile(string path, string defaultFile)
        {
            string defaultFileText = ResourcesUtilities.ReadTextFromFile(defaultFile);
            return new DataFile<TData>(path, defaultFileText);
        }
    }
}
