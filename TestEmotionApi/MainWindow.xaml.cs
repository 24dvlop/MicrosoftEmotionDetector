using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TestEmotionApi.Helper;

namespace TestEmotionApi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    internal class EmotionResultDisplay
    {
        public string EmotionString
        {
            get;
            set;
        }
        public float Score
        {
            get;
            set;
        }

        public int OriginalIndex
        {
            get;
            set;
        }
    }
    /*
    internal class FaceResultDisplay
    {
        public string Gender
        {
            get;
            set;
        }
        public int Age
        {
            get;
            set;
        }

        public int OriginalIndex
        {
            get;
            set;
        }
    }
    */
    public partial class MainWindow : Window
    {
        //WebCam webcam;
        VideoCaptureDevice LocalWebCam;
        public FilterInfoCollection LoaclWebCamsCollection;
        string picName;
        bool checkSetting = false;
        int fileDelNum = 0;
        DispatcherTimer dispatcherTimer;
        int timeCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            // string root = @"C:\Temp";
            //string subdir = @"C:\Users\Jaturong\Pictures\emotionproject";
            // If directory does not exist, create it. 
            if (!Directory.Exists(GlobalValue.Instance.folderPath))
            {
                Directory.CreateDirectory(GlobalValue.Instance.folderPath);
            }

            GlobalValue.Instance.setSecond = 10;
            //GlobalValue.Instance.setKey = "aaf9b7e129d347f8bf1a4652bc988104";
            //GlobalValue.Instance.setFaceKey = "119bef3b973548e180dc553126317a70";
            secondTb.Text = GlobalValue.Instance.setSecond.ToString();
            keyTb.Text = GlobalValue.Instance.setKey;
            faceKeyTb.Text = GlobalValue.Instance.setFaceKey;
            //MessageBox.Show("1");
            //MessageBox.Show(DateTime.Now.ToString("HHmmss"));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //webcam = new WebCam();
            //webcam.InitializeWebCam(ref imgVideo);
            //MessageBox.Show("2");

            LoaclWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            //MessageBox.Show(LoaclWebCamsCollection.Count + "");
            foreach(FilterInfo videoDevice in LoaclWebCamsCollection)
            {
                deviceList.Items.Add(videoDevice.Name);
            }

            deviceList.SelectedIndex = 0;

            LocalWebCam = new VideoCaptureDevice(LoaclWebCamsCollection[deviceList.SelectedIndex].MonikerString);
            LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);

            LocalWebCam.Start();
            startTimer();


            //MessageBox.Show(GlobalValue.Instance.folderPath);
        }

        public void startTimer()
        {
            //  DispatcherTimer setup
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            Storyboard sbss = this.FindResource("clockSpin") as Storyboard;
            sbss.Begin();
        }

        private async Task<Emotion[]> UploadAndDetectEmotions(string imageFilePath)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            string subscriptionKey = GlobalValue.Instance.setKey;
            //string faceKey = GlobalValue.Instance.setFaceKey;

            //window.Log("EmotionServiceClient is created");

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Project Oxford Emotion API Service client
            //
            //MessageBox.Show(subscriptionKey);
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(subscriptionKey);

            //window.Log("Calling EmotionServiceClient.RecognizeAsync()...");
            try
            {
                Emotion[] emotionResult;
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    //
                    // Detect the emotions in the URL
                    //
                    emotionResult = await emotionServiceClient.RecognizeAsync(imageFileStream);
                    return emotionResult;
                }
            }
            catch (Exception exception)
            {
                //window.Log(exception.ToString());
                MessageBox.Show("Invalid Emotion API Key, please try again..");
                return null;
            }
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------

        }

        private async Task<Face[]> UploadAndDetectFace(string imageFilePath)
        {
            //MainWindow window = (MainWindow)Application.Current.MainWindow;
            string subscriptionKey = GlobalValue.Instance.setFaceKey;
            //string faceKey = GlobalValue.Instance.setFaceKey;

            //window.Log("EmotionServiceClient is created");

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Project Oxford Emotion API Service client
            //
            //MessageBox.Show(subscriptionKey);
            FaceServiceClient faceServiceClient = new FaceServiceClient(subscriptionKey);

            //window.Log("Calling EmotionServiceClient.RecognizeAsync()...");
            try
            {
                Face[] faceResult;
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    //
                    // Detect the emotions in the URL
                    //
                    faceResult = await faceServiceClient.DetectAsync(imageFileStream, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses });
                    return faceResult;
                }
            }
            catch (Exception exception)
            {
                //window.Log(exception.ToString());
                MessageBox.Show("Invalid Face API Key, please try again..");
                return null;
            }
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------

        }
        /*
        private async void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.Filter = "JPEG Image(*.jpg)|*.jpg";
            bool? result = openDlg.ShowDialog(window);

            if (!(bool)result)
            {
                return;
            }

            //string imageFilePath = openDlg.FileName;
            string imageFilePath = ((BitmapFrame)showImg.Source).Decoder.ToString();
            Uri fileUri = new Uri(imageFilePath);

            BitmapImage bitmapSource = new BitmapImage();
            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            //_emotionDetectionUserControl.ImageUri = fileUri;
            //_emotionDetectionUserControl.Image = bitmapSource;
            showImg.Source = bitmapSource;
            //
            // Create EmotionServiceClient and detect the emotion with URL
            //
            //window.ScenarioControl.ClearLog();
            //_detectionStatus.Text = "Detecting...";
            
            Emotion[] emotionResult = await UploadAndDetectEmotions(imageFilePath);
            System.Diagnostics.Debug.WriteLine("Shitt");
            System.Diagnostics.Debug.WriteLine(emotionResult);
            // _detectionStatus.Text = "Detection Done";
            System.Diagnostics.Debug.WriteLine("damn");
            //
            // Log detection result in the log window
            //
            DrawFaceRectangle(showImg, bitmapSource, emotionResult);
            ListEmotionResult(fileUri, _resultListBox, emotionResult);


            //_emotionDetectionUserControl.Emotions = emotionResult;
        }
        */
        public void DrawFaceRectangle(Image image, BitmapImage bitmapSource, Emotion[] emotionResult)
        {
            if (emotionResult != null && emotionResult.Length > 0)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();

                drawingContext.DrawImage(bitmapSource,
                    new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));

                double dpi = bitmapSource.DpiX;
                double resizeFactor = 96 / dpi;

                foreach (var emotion in emotionResult)
                {
                    Microsoft.ProjectOxford.Common.Rectangle faceRect = emotion.FaceRectangle;

                    drawingContext.DrawRectangle(
                        Brushes.Transparent,
                        new Pen(Brushes.Cyan, 4),
                        new Rect(
                            faceRect.Left * resizeFactor,
                            faceRect.Top * resizeFactor,
                            faceRect.Width * resizeFactor,
                            faceRect.Height * resizeFactor)
                    );
                }

                drawingContext.Close();

                RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                    (int)(bitmapSource.PixelWidth * resizeFactor),
                    (int)(bitmapSource.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);

                faceWithRectBitmap.Render(visual);

                image.Source = faceWithRectBitmap;
            }
        }

        public void ListEmotionResult(Uri imageUri, ListBox resultListBox, Emotion[] emotionResult, Face[] faceResult)
        {
            if ((emotionResult != null) && (emotionResult.Length != 0))
            {
                EmotionResultDisplay[] resultDisplay = new EmotionResultDisplay[8];
                List<EmotionResultDisplayItem> itemSource = new List<EmotionResultDisplayItem>();
                for (int i = 0; i < emotionResult.Length; i++)
                {
                    Emotion emotion = emotionResult[i];
                    resultDisplay[0] = new EmotionResultDisplay { EmotionString = "Anger", Score = emotion.Scores.Anger };
                    resultDisplay[1] = new EmotionResultDisplay { EmotionString = "Contempt", Score = emotion.Scores.Contempt };
                    resultDisplay[2] = new EmotionResultDisplay { EmotionString = "Disgust", Score = emotion.Scores.Disgust };
                    resultDisplay[3] = new EmotionResultDisplay { EmotionString = "Fear", Score = emotion.Scores.Fear };
                    resultDisplay[4] = new EmotionResultDisplay { EmotionString = "Happiness", Score = emotion.Scores.Happiness };
                    resultDisplay[5] = new EmotionResultDisplay { EmotionString = "Neutral", Score = emotion.Scores.Neutral };
                    resultDisplay[6] = new EmotionResultDisplay { EmotionString = "Sadness", Score = emotion.Scores.Sadness };
                    resultDisplay[7] = new EmotionResultDisplay { EmotionString = "Surprise", Score = emotion.Scores.Surprise };
                    
                    Array.Sort(resultDisplay, CompareDisplayResults);

                    String[] emotionStrings = new String[3];
                    for (int j = 0; j < 3; j++)
                    {
                        emotionStrings[j] = resultDisplay[j].EmotionString + ":" + resultDisplay[j].Score.ToString("0.000000"); ;
                    }

                    Face face = faceResult[i];

                    String[] s = emotionStrings[0].Split(':');
                    String[] sa = emotionStrings[1].Split(':');
                    String[] sb = emotionStrings[2].Split(':');

                    string status = "";
                    string statusA = "";
                    string statusB = "";
                    string statusType="";
                    string statusAdditional = "";
                    string gifPath = "";
                    string genderThai = "";

                    if(s[0].Equals("Happiness"))
                    {
                        status = "มีความสุข";
                        gifPath = "Assets/happiness01.gif";
                    }
                    else if(s[0].Equals("Neutral"))
                    {
                        status = "เฉยๆ";
                        gifPath = "Assets/neutral01.gif";
                    }
                    else if (s[0].Equals("Contempt"))
                    {
                        status = "หยิ่ง";
                        gifPath = "Assets/contempt01.gif";
                    }
                    else if (s[0].Equals("Disgust"))
                    {
                        status = "ขยะแขยง";
                        gifPath = "Assets/disgust01.gif";
                    }
                    else if (s[0].Equals("Sadness"))
                    {
                        status = "เสียใจ";
                        gifPath = "Assets/sadness01.gif";
                    }
                    else if (s[0].Equals("Anger"))
                    {
                        status = "โกรธ";
                        gifPath = "Assets/anger01.gif";
                    }
                    else if (s[0].Equals("Fear"))
                    {
                        status = "กลัว";
                        gifPath = "Assets/sadness02.gif";
                    }
                    else if (s[0].Equals("Surprise"))
                    {
                        status = "ประหลาดใจ";
                        gifPath = "Assets/surprise01.gif";
                    }
                    //////////////////////////////////////////////
                    if (sa[0].Equals("Happiness"))
                    {
                        statusA = "มีความสุข";
                        //gifPath = "Assets/happiness01.gif";
                    }
                    else if (sa[0].Equals("Neutral"))
                    {
                        statusA = "เฉยๆ";
                        //gifPath = "Assets/neutral01.gif";
                    }
                    else if (sa[0].Equals("Contempt"))
                    {
                        statusA = "หยิ่ง";
                        //gifPath = "Assets/contempt01.gif";
                    }
                    else if (sa[0].Equals("Disgust"))
                    {
                        statusA = "ขยะแขยง";
                        //gifPath = "Assets/disgust01.gif";
                    }
                    else if (sa[0].Equals("Sadness"))
                    {
                        statusA = "เสียใจ";
                        //gifPath = "Assets/sadness01.gif";
                    }
                    else if (sa[0].Equals("Anger"))
                    {
                        statusA = "โกรธ";
                        //gifPath = "Assets/anger01.gif";
                    }
                    else if (sa[0].Equals("Fear"))
                    {
                        statusA = "กลัว";
                        //gifPath = "Assets/sadness02.gif";
                    }
                    else if (sa[0].Equals("Surprise"))
                    {
                        statusA = "ประหลาดใจ";
                        //gifPath = "Assets/surprise01.gif";
                    }
                    ////////////////////////////////////////////////
                    if (sb[0].Equals("Happiness"))
                    {
                        statusB = "มีความสุข";
                        //gifPath = "Assets/happiness01.gif";
                    }
                    else if (sb[0].Equals("Neutral"))
                    {
                        statusB = "เฉยๆ";
                        //gifPath = "Assets/neutral01.gif";
                    }
                    else if (sb[0].Equals("Contempt"))
                    {
                        statusB = "หยิ่ง";
                        //gifPath = "Assets/contempt01.gif";
                    }
                    else if (sb[0].Equals("Disgust"))
                    {
                        statusB = "ขยะแขยง";
                        //gifPath = "Assets/disgust01.gif";
                    }
                    else if (sb[0].Equals("Sadness"))
                    {
                        statusB = "เสียใจ";
                        //gifPath = "Assets/sadness01.gif";
                    }
                    else if (sb[0].Equals("Anger"))
                    {
                        statusB = "โกรธ";
                        //gifPath = "Assets/anger01.gif";
                    }
                    else if (sb[0].Equals("Fear"))
                    {
                        statusB = "กลัว";
                        //gifPath = "Assets/sadness02.gif";
                    }
                    else if (sb[0].Equals("Surprise"))
                    {
                        statusB = "ประหลาดใจ";
                        //gifPath = "Assets/surprise01.gif";
                    }

                    //////////////////////////////////////////////////////////

                    double getRate = double.Parse(s[1], System.Globalization.CultureInfo.InvariantCulture);
                    if (getRate >= 1.0)
                    {
                        statusType = "ที่สุดในโลก";
                    }
                    else if ((getRate < 1.0) && (getRate >= 0.8))
                    {
                        statusType = "มาก";
                    }
                    else if((getRate < 0.8)&&(getRate >= 0.5))
                    {
                        statusType = "ปานกลาง";
                    }
                    else if (getRate < 0.5)
                    {
                        statusType = "นิดหน่อย";
                    }

                    double getRateA = double.Parse(sa[1], System.Globalization.CultureInfo.InvariantCulture);
                    double getRateB = double.Parse(sb[1], System.Globalization.CultureInfo.InvariantCulture);
                    if ((getRateA>=0.1)&&(getRateB >= 0.08))
                    {
                        statusAdditional = " แต่ในขณะเดียวกัน คุณก็รู้สึก"+statusA + " และ" + statusB+"บ้างเล็กน้อย";
                    }
                    else if ((getRateA >= 0.1) && (getRateB < 0.08))
                    {
                        statusAdditional = " แต่ในขณะเดียวกัน คุณก็รู้สึก" + statusA+ "บ้างเล็กน้อย";
                    }
                    else if ((getRateA < 0.1) && (getRateB >= 0.08))
                    {
                        statusAdditional = " แต่ในขณะเดียวกัน คุณก็รู้สึก" + statusB + "บ้างเล็กน้อย";
                    }
                    
                    //System.Diagnostics.Debug.WriteLine(getRate + "");
                    //if(s[1])
                    if (face.FaceAttributes.Gender.Equals("male"))
                    {
                        genderThai = "ผู้ชาย";
                    }
                    else
                    {
                        genderThai = "ผู้หญิง";
                    }

                    double roundAge = Math.Round(face.FaceAttributes.Age, MidpointRounding.AwayFromZero);

                    itemSource.Add(new EmotionResultDisplayItem
                    {
                        ImageSource = imageUri,
                        UIRect = new Int32Rect(emotion.FaceRectangle.Left, emotion.FaceRectangle.Top, emotion.FaceRectangle.Width, emotion.FaceRectangle.Height),
                        EmotionText = "คุณรู้สึก"+status + statusType+ statusAdditional,
                        Emotion1 = s[0]+" "+s[1],
                        Emotion2 = sa[0] + " " + sa[1],
                        Emotion3 = sb[0] + " " + sb[1],
                        GifPath = gifPath,
                        Gender = "คุณเป็น "+genderThai,
                        Age = "อายุ "+ roundAge + " ปี"

                    });
                }
                resultListBox.ItemsSource = itemSource;
            }
            else
            {
                blankPopup.Visibility = Visibility.Visible;
            }
        }

        private int CompareDisplayResults(EmotionResultDisplay result1, EmotionResultDisplay result2)
        {
            return ((result1.Score == result2.Score) ? 0 : ((result1.Score < result2.Score) ? 1 : -1));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //webcam.Continue();
            
        }


        public void CaptureImages()
        {
            blankPopup.Visibility = Visibility.Collapsed;
            showImg.Source = imgVideo.Source.Clone();
            pRing.IsActive = true;
            pRingList.IsActive = true;
            /*
            if(checkImgNum==false)
            {
                picName = @"C:\Users\Jaturong\Pictures\test1.jpg";
                checkImgNum = true;
            }
            else if (checkImgNum == true)
            {
                picName = @"C:\Users\Jaturong\Pictures\test2.jpg";
                checkImgNum = false;
            }
            */
            picName = GlobalValue.Instance.folderPath+@"\emopic" + DateTime.Now.ToString("HHmmss") + ".jpg";

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)showImg.Source));
            using (FileStream stream = new FileStream(picName, FileMode.Create))
                encoder.Save(stream);

            System.IO.DirectoryInfo di = new DirectoryInfo(GlobalValue.Instance.folderPath);
            if(di.GetFiles().Length>20)
            {
                DelOverFlowImg();
            }

            upload2server();
        }


        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                System.Drawing.Image img = (System.Drawing.Bitmap)eventArgs.Frame.Clone();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    imgVideo.Source = bi;
                }));
            }
            catch (Exception ex)
            {
            }
        }

        public async void upload2server()
        {

            //string imageFilePath = openDlg.FileName;
            string imageFilePath = picName;
            Uri fileUri = new Uri(imageFilePath);

            BitmapImage bitmapSource = new BitmapImage();
            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            //_emotionDetectionUserControl.ImageUri = fileUri;
            //_emotionDetectionUserControl.Image = bitmapSource;
            showImg.Source = bitmapSource;
            //
            // Create EmotionServiceClient and detect the emotion with URL
            //
            //window.ScenarioControl.ClearLog();
            //_detectionStatus.Text = "Detecting...";

            Emotion[] emotionResult = await UploadAndDetectEmotions(imageFilePath);
            Face[] faceResult = await UploadAndDetectFace(imageFilePath);
            
            //System.Diagnostics.Debug.WriteLine("Shitt");
            System.Diagnostics.Debug.WriteLine(emotionResult);
            // _detectionStatus.Text = "Detection Done";
            //System.Diagnostics.Debug.WriteLine("damn");
            //
            // Log detection result in the log window
            //
            DrawFaceRectangle(showImg, bitmapSource, emotionResult);
            ListEmotionResult(fileUri, _resultListBox, emotionResult, faceResult);
            pRing.IsActive = false;
            pRingList.IsActive = false;

            //_emotionDetectionUserControl.Emotions = emotionResult;
        }

        public void DelOverFlowImg()
        {
            //MessageBox.Show("Delete over images");
            System.IO.DirectoryInfo di = new DirectoryInfo(GlobalValue.Instance.folderPath);
            //MessageBox.Show(di.GetFiles().Length+"");


            foreach (FileInfo file in di.GetFiles())
            {
                if (fileDelNum < 5)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch(Exception io)
                    {
                        MessageBox.Show(io.ToString());
                    }
                    
                    fileDelNum++;
                }
                //MessageBox.Show(file.Name);

            }

            fileDelNum = 0;
        }

        private void cap(object sender, RoutedEventArgs e)
        {
            CaptureImages();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            timeCount++;
            int t = GlobalValue.Instance.setSecond - timeCount;
            timeCT.Text = t.ToString();
            if (timeCount == GlobalValue.Instance.setSecond)
            {
                CaptureImages();
                timeCount = 0;
            }

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        private void settingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (checkSetting == false)
            {
                Storyboard sb = this.FindResource("SettingGoIn") as Storyboard;
                sb.Begin();
                checkSetting = true;
            }
            else if (checkSetting == true)
            {
                Storyboard sbs = this.FindResource("SettingGoOut") as Storyboard;
                sbs.Begin();
                checkSetting = false;
            }
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            secondTb.IsEnabled = true;
            keyTb.IsEnabled = true;
            faceKeyTb.IsEnabled = true;
            editBtn.Visibility = Visibility.Collapsed;
            editPane.Visibility = Visibility.Visible;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            GlobalValue.Instance.setSecond = Int32.Parse(secondTb.Text.ToString());
            GlobalValue.Instance.setKey = keyTb.Text;
            GlobalValue.Instance.setFaceKey = faceKeyTb.Text;
            secondTb.IsEnabled = false;
            keyTb.IsEnabled = false;
            faceKeyTb.IsEnabled = false;
            editBtn.Visibility = Visibility.Visible;
            editPane.Visibility = Visibility.Collapsed;

            dispatcherTimer.Stop();
            timeCount = 0;
            Storyboard sbss = this.FindResource("clockSpin") as Storyboard;
            sbss.Stop();

            startTimer();
            sbss.Begin();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            secondTb.Text = GlobalValue.Instance.setSecond.ToString();
            keyTb.Text = GlobalValue.Instance.setKey;
            faceKeyTb.Text = GlobalValue.Instance.setFaceKey;
            secondTb.IsEnabled = false;
            keyTb.IsEnabled = false;
            faceKeyTb.IsEnabled = false;
            editBtn.Visibility = Visibility.Visible;
            editPane.Visibility = Visibility.Collapsed;
        }

        private void shutdownBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
            //this.Close();
            //Application.Current.Shutdown();
        }

        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                LocalWebCam.Stop();

                LocalWebCam = new VideoCaptureDevice(LoaclWebCamsCollection[deviceList.SelectedIndex].MonikerString);
                LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);

                LocalWebCam.Start();
            }
            catch(Exception oe)
            {
                //MessageBox.Show("in select");
            }
        }
    }
}
