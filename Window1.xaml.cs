using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Media;
using System.IO;

namespace DArT
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        MainWindow main;
        int[] timers;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        //private List<ImageSource> Images = new List<ImageSource>();
        string[] Images;
        private double timerspan;
        private int imgIndex = 0;

        SoundPlayer doneS ;
        SoundPlayer beep3S ;
        SoundPlayer halfS ;
        SoundPlayer startS ;

        public Window1(MainWindow win, string[] Imag, int[] checke)
        {
            InitializeComponent();
            //string test = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //test = test.Substring(0, test.Length - 8);
            //doneS = new SoundPlayer(test+"done.wav");
            //beep3S = new SoundPlayer(test + "beep3.wav");
            //halfS = new SoundPlayer(test + "half.wav");
            //startS = new SoundPlayer(test + "start.wav");

            doneS = new SoundPlayer(Application.GetResourceStream(new Uri(@"pack://application:,,,/DArT;Component/Resources/done.wav")).Stream);
            beep3S = new SoundPlayer(Application.GetResourceStream(new Uri(@"pack://application:,,,/DArT;Component/Resources/beep3.wav")).Stream);
            halfS = new SoundPlayer(Application.GetResourceStream(new Uri(@"pack://application:,,,/DArT;Component/Resources/half.wav")).Stream);
            startS = new SoundPlayer(Application.GetResourceStream(new Uri(@"pack://application:,,,/DArT;Component/Resources/start.wav")).Stream);
            doneS.Load();
            beep3S.Load();
            halfS.Load();
            startS.Load();

            main = win;
            Opacity = 0;
            toppanel.Opacity = 0;
            botpanel.Opacity = 0;

            Images = Imag;
            timers = checke;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);            
            
            CreateStoryboard();

            this.SizeChanged += MainWindow_SizeChanged;

            //nextImage();            
        }
        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }
        void calcmaxTime()
        {
            if (timers[4] == 1)
            {
                progress.IsIndeterminate = true;
                maxTime.Content = (new TimeSpan(0, 0, 0)).ToString(@"mm\:ss");
            }
            else
            {
                if (timers[0]>0)
                {
                    maxTime.Content = (new TimeSpan(0, 0, 30)).ToString(@"mm\:ss");
                    timers[0]--;
                }
                else
                {
                    if (timers[1] > 0)
                    {
                        maxTime.Content = (new TimeSpan(0, 1, 0)).ToString(@"mm\:ss");
                        timers[1]--;
                    }
                    else
                    {
                        if (timers[2] > 0)
                        {
                            maxTime.Content = (new TimeSpan(0, 5, 0)).ToString(@"mm\:ss");
                            timers[2]--;
                        }
                        else
                        {
                            if (timers[3] > 0)
                            {
                                maxTime.Content = (new TimeSpan(0, 10, 0)).ToString(@"mm\:ss");
                                timers[3]--;
                            }
                            else
                            {
                                maxTime.Content = (new TimeSpan(0, 0, 0)).ToString(@"mm\:ss");
                            }
                        }
                    }
                }
            }
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (timerspan>0)
            {//2 segs between images
                if (timerspan==1)
                {
                    startS.Play();
                }
                timerspan--;
            }
            else
            {
                TimeSpan cur = TimeSpan.Parse("0:" + curTime.Content.ToString());
                TimeSpan max = TimeSpan.Parse("0:"+maxTime.Content.ToString());
                if (max.CompareTo(new TimeSpan(0, 0, 0)) > 0)
                {
                    
                    int compare = cur.CompareTo(max);
                    if (compare < 0)
                    {
                        cur = cur.Add(new TimeSpan(0, 0, 1));
                        curTime.Content = cur.ToString(@"mm\:ss");

                        Duration duration = new Duration(TimeSpan.FromSeconds(1));
                        DoubleAnimation doubleanimation = new DoubleAnimation(cur.Ticks, duration);
                        progress.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);

                        TimeSpan half = new TimeSpan(max.Ticks / 2);
                        int precision = 0; // Specify how many digits past the decimal point
                        const int TIMESPAN_SIZE = 7; // it always has seven digits
                                                     // convert the digitsToShow into a rounding/truncating mask
                        int factor = (int)Math.Pow(10, (TIMESPAN_SIZE - precision));
                        TimeSpan roundedTimeSpan = new TimeSpan(((long)Math.Round((1.0 * half.Ticks / factor)) * factor));
                        int halfBeep = cur.CompareTo(roundedTimeSpan);
                        if (halfBeep == 0)
                        {//beep half time
                            halfS.Play();
                        }
                        TimeSpan span = max - cur;
                        if (span.Seconds < 5)
                        {//beep urgency
                            beep3S.Play();
                        }
                    }
                    else
                    {//beep completed
                        doneS.Play();
                        dispatcherTimer.Stop();
                        Btn_pause.IsChecked = false;
                        FadeIn_image();
                        FadeOut_image();//fade-switch images
                    }
                }else
                {
                    cur = cur.Add(new TimeSpan(0, 0, 1));
                    curTime.Content = cur.ToString(@"mm\:ss");
                }
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Btn_close_Click(object sender, RoutedEventArgs e)
        {
            FadeOut();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FadeIn();
        }
        Storyboard fadeInStoryboard;
        Storyboard fadeOutStoryboard;
        Storyboard fadeInStoryboard_baners;
        Storyboard fadeOutStoryboard_baners;
        Storyboard fadeInStoryboard_image;
        Storyboard fadeOutStoryboard_image;
        public void CreateStoryboard()
        {
            // Create the fade in storyboard
            fadeInStoryboard = new Storyboard();
            fadeInStoryboard.Completed += new EventHandler(fadeInStoryboard_Completed);
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
            Storyboard.SetTarget(fadeInAnimation, this);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
            fadeInStoryboard.Children.Add(fadeInAnimation);

            // Create the fade out storyboard (Close)
            fadeOutStoryboard = new Storyboard();
            fadeOutStoryboard.Completed += new EventHandler(fadeOutStoryboard_Completed);
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)));
            Storyboard.SetTarget(fadeOutAnimation, this);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));
            fadeOutStoryboard.Children.Add(fadeOutAnimation);

            // Create the fade in storyboard (Baners)
            fadeInStoryboard_baners = new Storyboard();
            fadeInStoryboard_baners.Completed += new EventHandler(fadeInStoryboard_baners_Completed);
            DoubleAnimation fadeInAnimation_baners = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
            DoubleAnimation fadeInAnimation_baners2 = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
            Storyboard.SetTarget(fadeInAnimation_baners, toppanel);
            Storyboard.SetTarget(fadeInAnimation_baners2, botpanel);
            Storyboard.SetTargetProperty(fadeInAnimation_baners, new PropertyPath(UIElement.OpacityProperty));
            Storyboard.SetTargetProperty(fadeInAnimation_baners2, new PropertyPath(UIElement.OpacityProperty));
            fadeInStoryboard_baners.Children.Add(fadeInAnimation_baners);
            fadeInStoryboard_baners.Children.Add(fadeInAnimation_baners2);

            // Create the fade out storyboard (Baners)
            fadeOutStoryboard_baners = new Storyboard();
            fadeOutStoryboard_baners.Completed += new EventHandler(fadeOutStoryboard_baners_Completed);
            DoubleAnimation fadeOutAnimation_baners = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)));
            DoubleAnimation fadeOutAnimation_baners2 = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)));
            Storyboard.SetTarget(fadeOutAnimation_baners, toppanel);
            Storyboard.SetTarget(fadeOutAnimation_baners2, botpanel);
            Storyboard.SetTargetProperty(fadeOutAnimation_baners, new PropertyPath(UIElement.OpacityProperty));
            Storyboard.SetTargetProperty(fadeOutAnimation_baners2, new PropertyPath(UIElement.OpacityProperty));
            fadeOutStoryboard_baners.Children.Add(fadeOutAnimation_baners);
            fadeOutStoryboard_baners.Children.Add(fadeOutAnimation_baners2);

            // Create the fade in storyboard (Image)
            fadeInStoryboard_image = new Storyboard();
            fadeInStoryboard_image.Completed += new EventHandler(fadeInStoryboard_image_Completed);
            DoubleAnimation fadeInAnimation_image = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
            Storyboard.SetTarget(fadeInAnimation_image, mainimage2);
            Storyboard.SetTargetProperty(fadeInAnimation_image, new PropertyPath(UIElement.OpacityProperty));
            fadeInStoryboard_image.Children.Add(fadeInAnimation_image);

            // Create the fade out storyboard (Image)
            fadeOutStoryboard_image = new Storyboard();
            fadeOutStoryboard_image.Completed += new EventHandler(fadeOutStoryboard_image_Completed);
            DoubleAnimation fadeOutAnimation_image = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)));
            Storyboard.SetTarget(fadeOutAnimation_image, mainimage);
            Storyboard.SetTargetProperty(fadeOutAnimation_image, new PropertyPath(UIElement.OpacityProperty));
            fadeOutStoryboard_image.Children.Add(fadeOutAnimation_image);
        }
        private void fadeOutStoryboard_image_Completed(object sender, EventArgs e)
        {
            fadeOutStoryboard_image.Stop();
        }

        private void fadeInStoryboard_image_Completed(object sender, EventArgs e)
        {
            fadeInStoryboard_image.Stop();
            nextImage();
            GC.Collect();
        }
        private void nextImage()
        {//change to next image            
            if (imgIndex < Images.Length)
            {
                bool fileOK = true;
                while (fileOK)
                {
                    try
                    {
                        if (imgIndex < Images.Length)
                        {
                            var bitmap = new BitmapImage();
                            var stream = File.OpenRead(Images[imgIndex]);
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            stream.Close();
                            stream.Dispose();
                            bitmap.Freeze();
                            fileOK = false;
                            curFile.Content = " " + System.IO.Path.GetFileNameWithoutExtension(Images[imgIndex]) + " ";
                            mainimage.Source = bitmap;
                            bitmap = null;
                        }
                        else
                        {
                            fileOK = false;
                        }
                    }
                    catch (Exception)
                    {
                        imgIndex++;
                    }
                }
                
                imgIndex++;
                fileOK = true;
                while (fileOK)
                {
                    try
                    {
                        if (imgIndex < Images.Length)
                        {
                            var bitmap = new BitmapImage();
                            var stream = File.OpenRead(Images[imgIndex]);
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            stream.Close();
                            stream.Dispose();
                            bitmap.Freeze();
                            fileOK = false;
                            mainimage2.Source = bitmap;
                            bitmap = null;
                        }
                        else
                        {
                            fileOK = false;
                        }
                    }
                    catch (Exception r)
                    {
                        imgIndex++;
                    }
                }

                mainimage.Height = SystemParameters.PrimaryScreenHeight;
                mainimage.UpdateLayout();
                this.Height = mainimage.ActualHeight;
                mainimage.UpdateLayout();
                this.Width = mainimage.ActualWidth;

                this.Top = (SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);

                timerspan = 2;

                calcmaxTime();
                TimeSpan max = TimeSpan.Parse("0:" + maxTime.Content.ToString());
                progress.Maximum = max.Ticks;

                Duration duration = new Duration(TimeSpan.FromSeconds(2));
                DoubleAnimation doubleanimation = new DoubleAnimation(0, duration);//progress.Value = 0;
                progress.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
                curTime.Content = "00:00";
                dispatcherTimer.Start();
                Btn_pause.IsChecked = true;
            }else
            {
                //end beep
            }
        }
        private void fadeOutStoryboard_baners_Completed(object sender, EventArgs e)
        {
            toppanel.Opacity = 0;
            botpanel.Opacity = 0;
            fadeOutStoryboard_baners.Stop();
        }
        private void fadeInStoryboard_baners_Completed(object sender, EventArgs e)
        {
            toppanel.Opacity = 100;
            botpanel.Opacity = 100;
            fadeInStoryboard_baners.Stop();            
        }
        private void fadeOutStoryboard_Completed(object sender, EventArgs e)
        {
            //main.Width = this.Width;
            //main.Height = this.Height;
            //main.Left = this.Left;
            //main.Top = this.Top;
            main.Show();
            main.FadeIn();
            fadeOutStoryboard.Stop();
            dispatcherTimer.Stop();
            doneS.Dispose();
            beep3S.Dispose();
            halfS.Dispose();
            startS.Dispose();
            mainimage = null;
            mainimage2 = null;
            GC.Collect();
            this.Close();
        }
        private void fadeInStoryboard_Completed(object sender, EventArgs e)
        {
            fadeInStoryboard.Stop();
            this.Opacity = 100;
            nextImage();
        }
        public void FadeIn()
        {
            // Begin fade in animation
            this.Dispatcher.BeginInvoke(new Action(fadeInStoryboard.Begin), DispatcherPriority.Render, null);
        }
        public void FadeOut()
        {
            // Begin fade out animation
            this.Dispatcher.BeginInvoke(new Action(fadeOutStoryboard.Begin), DispatcherPriority.Render, null);
        }
        public void FadeIn_baners()
        {
            // Begin fade in animation
            this.Dispatcher.BeginInvoke(new Action(fadeInStoryboard_baners.Begin), DispatcherPriority.Render, null);
        }
        public void FadeOut_baners()
        {
            // Begin fade out animation
            this.Dispatcher.BeginInvoke(new Action(fadeOutStoryboard_baners.Begin), DispatcherPriority.Render, null);
        }
        public void FadeIn_image()
        {
            // Begin fade in animation
            this.Dispatcher.BeginInvoke(new Action(fadeInStoryboard_image.Begin), DispatcherPriority.Render, null);
        }
        public void FadeOut_image()
        {
            // Begin fade out animation
            this.Dispatcher.BeginInvoke(new Action(fadeOutStoryboard_image.Begin), DispatcherPriority.Render, null);
        }

        private void Btn_back_Click(object sender, RoutedEventArgs e)
        {
            if (imgIndex > 1)
            {
                dispatcherTimer.Stop();
                Btn_pause.IsChecked = false;
                imgIndex -=2;
                bool fileOK = true;
                while (fileOK)
                {
                    try
                    {
                        if (imgIndex >= 0)
                        {
                            var bitmap = new BitmapImage();
                            var stream = File.OpenRead(Images[imgIndex]);
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            stream.Close();
                            stream.Dispose();
                            bitmap.Freeze();
                            fileOK = false;
                            mainimage2.Source = bitmap;
                            bitmap = null;
                        }
                        else
                        {
                            fileOK = false;
                            mainimage2.Source = mainimage.Source;
                        }
                    }
                    catch (Exception)
                    {
                        imgIndex--;
                    }
                }
                FadeIn_image();
                FadeOut_image();//fade-switch images
            }
        }

        private void Btn_pause_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
                Btn_pause.IsChecked = false;
            }else
            {
                dispatcherTimer.Start();
                Btn_pause.IsChecked = true;
            }            
        }

        private void Btn_next_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            Btn_pause.IsChecked = false;
            FadeIn_image();
            FadeOut_image();//fade-switch images
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            FadeIn_baners();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            FadeOut_baners();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            dispatcherTimer.Stop();
        }
    }
}