using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEmotionApi.Helper
{
    public class EmotionResultDisplayItem
    {
        public Uri ImageSource { get; set; }

        public System.Windows.Int32Rect UIRect { get; set; }
        public string EmotionText { get; set; }
        public string Emotion1 { get; set; }
        public string Emotion2 { get; set; }
        public string Emotion3 { get; set; }
        public string GifPath { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
    }
}
