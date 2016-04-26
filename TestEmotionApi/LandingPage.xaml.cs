using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TestEmotionApi.Helper;

namespace TestEmotionApi
{
    /// <summary>
    /// Interaction logic for LandingPage.xaml
    /// </summary>
    public partial class LandingPage : Window
    {
        DispatcherTimer dispatcherTimer;
        public LandingPage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sbs = this.FindResource("LogoAppear") as Storyboard;
            sbs.Begin();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            emotionApiTb.IsEnabled = true;
            faceApiTb.IsEnabled = true;
            saveBtn.IsEnabled = true;
            //exitBtn.IsEnabled = true;
            pRingList.IsActive = false;
            Storyboard sbs = this.FindResource("LogoAppear") as Storyboard;
            sbs.Stop();
            // Updating the Label which displays the current second
            var newForm = new MainWindow(); //create your new form.
            newForm.Show();
            this.Close();

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            GlobalValue.Instance.setKey = emotionApiTb.Text;
            GlobalValue.Instance.setFaceKey = faceApiTb.Text;
            emotionApiTb.IsEnabled = false;
            faceApiTb.IsEnabled = false;
            saveBtn.IsEnabled = false;
            //exitBtn.IsEnabled = false;
            pRingList.IsActive = true;

            if(CheckForInternetConnection()==true)
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
                dispatcherTimer.Start();
            }
            else
            {
                MessageBox.Show("Please, check your internet connection and try again..");
                emotionApiTb.IsEnabled = true;
                faceApiTb.IsEnabled = true;
                saveBtn.IsEnabled = true;
                //exitBtn.IsEnabled = true;
                pRingList.IsActive = false;
            }

            
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
