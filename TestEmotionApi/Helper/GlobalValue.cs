using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEmotionApi.Helper
{
    public class GlobalValue
    {
        private static GlobalValue _mInstance;

        public static GlobalValue Instance
        {
            get { return _mInstance ?? (_mInstance = new GlobalValue()); }
        }

        /// <summary>
        /// You can send values through this string.
        /// </summary>
        //public int seconds { get; set; }
        public string SomeString { get; set; }
        public int setSecond { get; set; }
        public string setKey { get; set; }
        public string setFaceKey { get; set; }

        public int seconds = 10;
        public string folderPath = @"C:\emotionproject";
    }
}
