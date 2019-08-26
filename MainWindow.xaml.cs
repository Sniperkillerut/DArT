using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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

namespace DArT
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private List<ImageSource> Images = new List<ImageSource>();
        private string[] Images = { };
        int[] timers = { 0, 0, 0, 0 , 0};

        public MainWindow()
        {            
            InitializeComponent();            
            CreateStoryboard();
            
            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;
            this.Height = height;
            this.Width = width / 6;
            this.Left = 0;
            this.Top = 0;
            this.Opacity = 0;

            this.SizeChanged += MainWindow_SizeChanged;
        }
        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            FadeOut(true);
        }

        private void calculateTime()
        {
            if (Btn_free.IsChecked.Value)
            {
                OveralTime.Content = "Overal Time: Free";
                timers = new int[5] { 0, 0, 0, 0, 1 };
            }
            else
            {
                double overaltime = 0;
                int cont = 0;
                if (Btn_30s.IsChecked.Value)
                {
                    cont++;
                }
                if (Btn_1m.IsChecked.Value)
                {
                    cont++;
                }
                if (Btn_5m.IsChecked.Value)
                {
                    cont++;
                }
                if (Btn_10m.IsChecked.Value)
                {
                    cont++;
                }
                timers = new int[5] { 0, 0, 0, 0, 0};
                int imagecount = Images.Length;
                if (Btn_30s.IsChecked.Value)
                {
                    overaltime += (imagecount / cont) * 0.5;
                    timers[0] = (imagecount / cont);
                    imagecount -= timers[0];
                    cont--;
                }
                if (Btn_1m.IsChecked.Value)
                {
                    overaltime += (imagecount / cont) * 1;
                    timers[1] = (imagecount / cont);
                    imagecount -= timers[1];
                    cont--;
                }
                if (Btn_5m.IsChecked.Value)
                {
                    overaltime += (imagecount / cont) * 5;
                    timers[2] = (imagecount / cont);
                    imagecount -= timers[2];
                    cont--;
                }
                if (Btn_10m.IsChecked.Value)
                {
                    overaltime += (imagecount / cont) * 10;
                    timers[3] = (imagecount / cont);
                    imagecount -= timers[3];
                    cont--;
                }

                OveralTime.Content = "Overal Time: " + overaltime + " Minutes";
            }
        }

        private void Btn_SF_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.gif, *.bmp)|*.jpg;*.jpeg;*.jpe;*.jfif;*.png;*.gif;*.bmp";
            openFileDialog.ValidateNames = true;
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileNames.Length > 0)
                {
                    FolderDir.Content = System.IO.Path.GetDirectoryName(openFileDialog.FileNames[0]);
                    FileCount.Content = openFileDialog.FileNames.Length + " Files";

                   // var sources = from file in openFileDialog.FileNames
                    //              select CreateImageSource(file, true);

                    //Images.Clear();
                    //Images.AddRange(sources);
                    Images = openFileDialog.FileNames;
                    
                    calculateTime();
                }
            }

        }
        private ImageSource CreateImageSource(string file, bool forcePreLoad)
        {
            if (forcePreLoad)
            {
                var src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(file, UriKind.Absolute);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                src.Freeze();
                return src;
            }
            else
            {
                var src = new BitmapImage(new Uri(file, UriKind.Absolute));
                src.Freeze();
                return src;
            }
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if (Images.Length>0 && timers.Sum()>0)
            {
                FadeOut(false);
            }else
            {
                //play error sound
                //SystemSounds.Asterisk.Play();
                //SystemSounds.Beep.Play();
                //SystemSounds.Exclamation.Play();
                //SystemSounds.Hand.Play();
                //SystemSounds.Question.Play();
                Uri uri = new Uri(@"pack://application:,,,/DArT;Component/Resources/done.wav");
                SoundPlayer player = new SoundPlayer(Application.GetResourceStream(uri).Stream);
                player.Load();
                player.Play();

            }
        }

        private void Btn_30s_Click(object sender, RoutedEventArgs e)
        {
            calculateTime();
        }
        private void Btn_1m_Click(object sender, RoutedEventArgs e)
        {
            calculateTime();
        }
        private void Btn_5m_Click(object sender, RoutedEventArgs e)
        {
            calculateTime();
        }
        private void Btn_10m_Click(object sender, RoutedEventArgs e)
        {
            calculateTime();
        }
        private void Btn_free_Click(object sender, RoutedEventArgs e)
        {
            calculateTime();
            if (Btn_free.IsChecked.Value)
            {
                Btn_30s.IsChecked = false;
                Btn_1m.IsChecked = false;
                Btn_5m.IsChecked = false;
                Btn_10m.IsChecked = false;

                Btn_30s.IsEnabled = false;
                Btn_1m.IsEnabled = false;
                Btn_5m.IsEnabled = false;
                Btn_10m.IsEnabled = false;
                
            }
            else
            {
                Btn_30s.IsEnabled = true;
                Btn_1m.IsEnabled = true;
                Btn_5m.IsEnabled = true;
                Btn_10m.IsEnabled = true;
            }
        }
        private void Btn_Contact_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://co.linkedin.com/in/christian-gil-fl%C3%B3rez-3b01b966");
        }

        private void Btn_Edefex_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://picarto.tv/Edefex");
        }

        private void Btn_Sniperkillerut_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://co.linkedin.com/in/christian-gil-fl%C3%B3rez-3b01b966");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FadeIn();
        }
        Storyboard fadeInStoryboard;
        Storyboard fadeOutStoryboard;
        Storyboard fadeOutStoryboard2;
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

            // Create the fade out storyboard (Hide)
            fadeOutStoryboard2 = new Storyboard();
            fadeOutStoryboard2.Completed += new EventHandler(fadeOutStoryboard2_Completed);
            DoubleAnimation fadeOutAnimation2 = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)));
            Storyboard.SetTarget(fadeOutAnimation2, this);
            Storyboard.SetTargetProperty(fadeOutAnimation2, new PropertyPath(UIElement.OpacityProperty));
            fadeOutStoryboard2.Children.Add(fadeOutAnimation2);
        }

        private void fadeOutStoryboard_Completed(object sender, EventArgs e)
        {
            fadeOutStoryboard.Stop();
            this.Close();
        }
        private void fadeOutStoryboard2_Completed(object sender, EventArgs e)
        {            
            Window1 w1 = new Window1(this, Images, timers);
            w1.Width = this.Width;
            w1.Height = this.Height;
            w1.Left = this.Left;
            w1.Top = this.Top;
            w1.Show();
            fadeOutStoryboard.Stop();
            this.Hide();
        }

        private void fadeInStoryboard_Completed(object sender, EventArgs e)
        {
            fadeInStoryboard.Stop();
            this.Opacity = 100;
            calculateTime();
        }

        public void FadeIn()
        {
            // Begin fade in animation
            this.Dispatcher.BeginInvoke(new Action(fadeInStoryboard.Begin), DispatcherPriority.Render, null);
        }
        public void FadeOut(bool close)
        {
            // Begin fade out animation
            if (close)
            {
                this.Dispatcher.BeginInvoke(new Action(fadeOutStoryboard.Begin), DispatcherPriority.Render, null);
            }else
            {
                //hide and show image window
                this.Dispatcher.BeginInvoke(new Action(fadeOutStoryboard2.Begin), DispatcherPriority.Render, null);
            }
            
        }
    }
}
