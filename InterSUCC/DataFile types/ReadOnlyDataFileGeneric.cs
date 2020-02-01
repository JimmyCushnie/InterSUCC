using System;
using SUCC;
using SUCC.Abstractions;
using SUCC.UnityStuff;
using ClassImpl;

namespace InterSUCC
{
    public class ReadOnlyDataFile<TData> : ReadOnlyDataFile where TData : class
    {
        public TData Data { get; }


        /// <summary>
        /// Creates a new <see cref="ReadOnlyDataFile{TData}"/> object, using a text file in Unity's Resources folder for the default file text.
        /// </summary>
        /// <param name="path"> The path of the file. Can be either absolute or relative to the default path. </param>
        /// <param name="defaultFile"> The path of the default file, relative to any Resources parent. </param>
        /// <returns></returns>
        public ReadOnlyDataFile(string path, string defaultFileText = null) : base(path, defaultFileText)
        {
            this.Data = DataUtility<TData>.GenerateDataObject(this);
        }
    }
}
