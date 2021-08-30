using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DesktopWPFAppLowLevelKeyboardHook;
using Microsoft.Win32;
using System.IO;

namespace OnTopWindow
{
    public enum VideoPlayerMode
    {
        Stopped,
        Played,
        Paused
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int prWidth = 100;
        int prHeight = 100;

        public string SoundPath { get; set; } = System.AppDomain.CurrentDomain.BaseDirectory + "sound.mp3";
        public string CuckooSoundPath { get; set; } = System.AppDomain.CurrentDomain.BaseDirectory + "cuckoo.mp3";
        public string[] SomeTextArray { get; set; }
        public List<string> ImagesArray { get; set; }
        public string TextPath { get; set; } = System.AppDomain.CurrentDomain.BaseDirectory + "sometext.txt";
        public int TextIndex { get; set; } = 0;
        public string ImagesFolderPath { get; set; } = System.AppDomain.CurrentDomain.BaseDirectory + "Images";
        public int ImageIndex { get; set; } = 0;
        public string[] VideoLinks { get; set; }
        public int VideoIndex { get; set; }
        public VideoPlayerMode VideoPlayerMode = VideoPlayerMode.Stopped;
        public MediaPlayer MediaPlayer { get; set; } = new MediaPlayer();
        LowLevelKeyboardListener _listener;

        public static MainWindow Main { get; set; }
        static VisualWindow Visual { get; set; }

        const int INCREASE = 10;
        public int TxtLeft { get; set; } = 0;
        public int TxtRight { get; set; } = 0;
        public int TxtTop { get; set; } = 0;
        public int TxtBottom { get; set; } = 0;

        const int HOUR = 3600;
        const int MINUTE = 60;
        public int TimerSeconds { get; set; } = (0 * HOUR) + (1 * MINUTE) + 0;
        public int TimerTimeLeft { get; set; }
        public enum TimerSetting
        {
            Standard,
            Hours,
            Minutes,
            Seconds
        }
        public TimerSetting TimerSet { get; set; } = TimerSetting.Standard;

        public MainWindow()
        {
            InitializeComponent();
            Main = this;
            Visual = new VisualWindow();

            string strbuffer = "";
            strbuffer = File.ReadAllText(TextPath);
            SomeTextArray = strbuffer.Split(new string[] { "\r\n@newtext\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            ImagesArray = Directory.GetFiles(ImagesFolderPath, "*.png", SearchOption.AllDirectories).ToList();
            ImagesArray.AddRange(Directory.GetFiles(ImagesFolderPath, "*.jpg", SearchOption.AllDirectories).ToList());
            if (SomeTextArray.Length == 0) Visual.textblock_message.Text = tbx1_msg.Text = "Hello World";
            else Visual.textblock_message.Text = tbx1_msg.Text = SomeTextArray[TextIndex];
            if (ImagesArray.Count > 0)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(ImagesArray[ImageIndex]);
                bmp.EndInit();
                Visual.image.Source = bmp;
                if (bmp.PixelWidth > 0.8 * System.Windows.SystemParameters.PrimaryScreenWidth ||
                    bmp.PixelHeight > 0.8 * System.Windows.SystemParameters.PrimaryScreenHeight)
                {
                    Visual.image.Width = 0.8 * bmp.PixelWidth;
                    Visual.image.Height = 0.8 * bmp.PixelHeight;
                }
                else
                {
                    Visual.image.Width = bmp.PixelWidth;
                    Visual.image.Height = bmp.PixelHeight;
                }
            }


            rad2_tim.IsChecked = true;

            Visual.Width = System.Windows.SystemParameters.PrimaryScreenWidth;

            Visual.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            Visual.Top = Visual.Left = 0;

            Visual.Topmost = true;

            Visual.textblock_stopwatch.Text = "0:00:00:000";

            Visual.ShowTime(TimerSeconds);

            //InitializeChromium();
        }

        private void windowShowHide_ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (dontPlayVideo_CheckBox.IsChecked == false)
            {
                if (VideoLinks != null && VideoLinks.Length > 0)
                    try
                    {
                        VideoPlayerMode = VideoPlayerMode.Played;
                        Visual.RewindVideo(-6);
                        Visual.PlayVideo();
                    }
                    catch { }
            }
            Visual.Topmost = true;
            Visual.WindowStyle = WindowStyle.None;
            Visual.WindowState = WindowState.Maximized;
            //Visual.Activate();
            Visual.Show();
        }

        private void windowShowHide_ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (dontPlayVideo_CheckBox.IsChecked == false)
            {
                if (VideoPlayerMode == VideoPlayerMode.Played)
                {
                    VideoPlayerMode = VideoPlayerMode.Paused;
                    Visual.PauseVideo();
                }
            }
            if (Visual != null)
            {
                Visual.Topmost = false;
                Visual.Hide();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;

            _listener.HookKeyboard();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Visual != null) Visual.Close();

            _listener.UnHookKeyboard();
        }

        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            //MessageBox.Show(e.KeyPressed.ToString());
            /*/if (false)/**/
            if (e.KeyPressed == Key.NumPad0) windowShowHide_ToggleButton.IsChecked = !windowShowHide_ToggleButton.IsChecked;
            else if (windowShowHide_ToggleButton.IsChecked == true) switch (e.KeyPressed)
                {
                    case Key.Decimal:
                        if (rad1_msg.IsChecked == true)
                        {
                            rad1_msg.IsChecked = false;
                            rad2_tim.IsChecked = true;
                        }
                        else if (rad2_tim.IsChecked == true)
                        {
                            rad2_tim.IsChecked = false;
                            rad3_stw.IsChecked = true;
                        }
                        else if (rad3_stw.IsChecked == true)
                        {
                            rad3_stw.IsChecked = false;
                            rad4_tmr.IsChecked = true;
                        }
                        else if (rad4_tmr.IsChecked == true)
                        {
                            rad4_tmr.IsChecked = false;
                            rad5_img.IsChecked = true;
                        }
                        else if (rad5_img.IsChecked == true)
                        {
                            rad5_img.IsChecked = false;
                            rad6_bws.IsChecked = true;
                        }
                        else if (rad6_bws.IsChecked == true)
                        {
                            rad6_bws.IsChecked = false;
                            rad7_vpl.IsChecked = true;
                        }
                        else
                        {
                            rad7_vpl.IsChecked = false;
                            rad1_msg.IsChecked = true;
                        }
                        break;

                    case Key.Divide:
                        if (rad4_tmr.IsChecked == true)
                        {
                            if (TimerSet == TimerSetting.Standard)
                                TimerSet = TimerSetting.Seconds;
                            else TimerSet--;
                            Visual.ShowTime(TimerSeconds);
                        }
                        if (rad7_vpl.IsChecked == true)
                        {
                            if (VideoPlayerMode == VideoPlayerMode.Played)
                            {
                                VideoPlayerMode = VideoPlayerMode.Paused;
                                Visual.PauseVideo();
                            }
                            else if (VideoLinks != null && VideoLinks.Length > 0)
                                try
                                {
                                    VideoPlayerMode = VideoPlayerMode.Played;
                                    Visual.PlayVideo();
                                }
                                catch { }
                        }
                        break;

                    case Key.Multiply:
                        if (rad4_tmr.IsChecked == true)
                        {
                            if (TimerSet == TimerSetting.Seconds)
                                TimerSet = TimerSetting.Standard;
                            else TimerSet++;
                            Visual.ShowTime(TimerSeconds);
                        }
                        if (rad7_vpl.IsChecked == true)
                        {
                            VideoIndex = VideoIndex >= VideoLinks.Length - 1 ? 0 : VideoIndex + 1;
                            if (VideoLinks.Length > 0)
                                try
                                {
                                    Visual.SetVideoLink(VideoLinks[VideoIndex]);
                                }
                                catch { }
                        }
                        break;

                    case Key.Add:
                        if (rad1_msg.IsChecked == true)
                        {
                            TextIndex++; if (TextIndex == SomeTextArray.Length) TextIndex = 0;
                            if (SomeTextArray.Length > 0) Visual.textblock_message.Text = tbx1_msg.Text = SomeTextArray[TextIndex];
                        }
                        if (rad2_tim.IsChecked == true) cuckoo_CheckBox.IsChecked = true;
                        if (rad3_stw.IsChecked == true) Visual.StartStopwatch();
                        if (rad4_tmr.IsChecked == true)
                        {
                            int h = TimerSeconds / HOUR;
                            int m = TimerSeconds / MINUTE - h * MINUTE;
                            int s = TimerSeconds - h * HOUR - m * MINUTE;

                            switch (TimerSet)
                            {
                                case TimerSetting.Standard: if (Visual.IsTimerStarted == false) Visual.StartTimer(); break;
                                case TimerSetting.Hours: TimerSeconds += HOUR; break;
                                case TimerSetting.Minutes: if (m < 59) TimerSeconds += MINUTE; break;
                                case TimerSetting.Seconds: if (s < 59) TimerSeconds++; break;
                            }
                            if (TimerSet != TimerSetting.Standard) Visual.ShowTime(TimerSeconds);
                        }
                        if (rad5_img.IsChecked == true)
                        {
                            ImageIndex++; if (ImageIndex == ImagesArray.Count) ImageIndex = 0;
                            if (ImagesArray.Count > 0)
                            {
                                BitmapImage bmp = new BitmapImage();
                                bmp.BeginInit();
                                bmp.UriSource = new Uri(ImagesArray[ImageIndex]);
                                bmp.EndInit();
                                Visual.image.Source = bmp;
                                if (bmp.PixelWidth > 0.8 * System.Windows.SystemParameters.PrimaryScreenWidth ||
                                    bmp.PixelHeight > 0.8 * System.Windows.SystemParameters.PrimaryScreenHeight)
                                {
                                    Visual.image.Width = 0.8 * bmp.PixelWidth;
                                    Visual.image.Height = 0.8 * bmp.PixelHeight;
                                }
                                else
                                {
                                    Visual.image.Width = bmp.PixelWidth;
                                    Visual.image.Height = bmp.PixelHeight;
                                }
                            }
                        }
                        if (rad7_vpl.IsChecked == true)
                        {
                            if (VideoPlayerMode == VideoPlayerMode.Played)
                                Visual.RewindVideo(5);
                        }
                        break;

                    case Key.Subtract:
                        if (rad1_msg.IsChecked == true)
                        {
                            TextIndex--; if (TextIndex == -1) TextIndex = SomeTextArray.Length - 1;
                            if (SomeTextArray.Length > 0) Visual.textblock_message.Text = tbx1_msg.Text = SomeTextArray[TextIndex];
                        }
                        if (rad2_tim.IsChecked == true) cuckoo_CheckBox.IsChecked = false;
                        if (rad3_stw.IsChecked == true) Visual.StopStopwatch();
                        if (rad4_tmr.IsChecked == true)
                        {
                            int h = TimerSeconds / HOUR;
                            int m = TimerSeconds / MINUTE - h * MINUTE;
                            int s = TimerSeconds - h * HOUR - m * MINUTE;

                            switch (TimerSet)
                            {
                                case TimerSetting.Standard: Visual.StopTimer(); break;
                                case TimerSetting.Hours: if (h > 0) TimerSeconds -= HOUR; break;
                                case TimerSetting.Minutes: if (m > 0) TimerSeconds -= MINUTE; break;
                                case TimerSetting.Seconds: if (s > 0) TimerSeconds--; break;
                            }
                            if (TimerSeconds < 10) TimerSeconds = 10;
                            if (TimerSet != TimerSetting.Standard) Visual.ShowTime(TimerSeconds);
                        }
                        if (rad5_img.IsChecked == true)
                        {
                            ImageIndex--; if (ImageIndex == -1) ImageIndex = ImagesArray.Count - 1;
                            if (ImagesArray.Count > 0)
                            {
                                BitmapImage bmp = new BitmapImage();
                                bmp.BeginInit();
                                bmp.UriSource = new Uri(ImagesArray[ImageIndex]);
                                bmp.EndInit();
                                Visual.image.Source = bmp;
                                if (bmp.PixelWidth > 0.8 * System.Windows.SystemParameters.PrimaryScreenWidth ||
                                    bmp.PixelHeight > 0.8 * System.Windows.SystemParameters.PrimaryScreenHeight)
                                {
                                    Visual.image.Width = 0.8 * bmp.PixelWidth;
                                    Visual.image.Height = 0.8 * bmp.PixelHeight;
                                }
                                else
                                {
                                    Visual.image.Width = bmp.PixelWidth;
                                    Visual.image.Height = bmp.PixelHeight;
                                }
                            }
                        }
                        if (rad7_vpl.IsChecked == true)
                        {
                            if (VideoPlayerMode == VideoPlayerMode.Played)
                                Visual.RewindVideo(-5);
                        }
                        break;

                    case Key.NumPad8:
                        if (TxtTop > INCREASE) TxtTop -= INCREASE; else TxtTop = 0;
                        TxtBottom += INCREASE;
                        Visual.visual_Grid.Margin = new Thickness(TxtLeft, TxtTop, TxtRight, TxtBottom);
                        Visual.Update();
                        break;

                    case Key.NumPad2:
                        if (TxtBottom > INCREASE) TxtBottom -= INCREASE; else TxtBottom = 0;
                        TxtTop += INCREASE;
                        Visual.visual_Grid.Margin = new Thickness(TxtLeft, TxtTop, TxtRight, TxtBottom);
                        Visual.Update();
                        break;

                    case Key.NumPad4:
                        if (TxtLeft > INCREASE) TxtLeft -= INCREASE; else TxtLeft = 0;
                        TxtRight += INCREASE;
                        Visual.visual_Grid.Margin = new Thickness(TxtLeft, TxtTop, TxtRight, TxtBottom);
                        Visual.Update();
                        break;

                    case Key.NumPad6:
                        if (TxtRight > INCREASE) TxtRight -= INCREASE; else TxtRight = 0;
                        TxtLeft += INCREASE;
                        Visual.visual_Grid.Margin = new Thickness(TxtLeft, TxtTop, TxtRight, TxtBottom);
                        Visual.Update();
                        break;

                    case Key.NumPad7:
                        TxtLeft = TxtRight = TxtTop = TxtBottom = 0;
                        Visual.visual_Grid.HorizontalAlignment = HorizontalAlignment.Left;
                        Visual.visual_Grid.VerticalAlignment = VerticalAlignment.Top;
                        Visual.visual_Grid.Margin = new Thickness(TxtLeft, TxtTop, TxtRight, TxtBottom);
                        Visual.Update();
                        break;

                    case Key.NumPad9:
                        TxtLeft = TxtRight = TxtTop = TxtBottom = 0;
                        Visual.visual_Grid.HorizontalAlignment = HorizontalAlignment.Right;
                        Visual.visual_Grid.VerticalAlignment = VerticalAlignment.Top;
                        Visual.visual_Grid.Margin = new Thickness(TxtLeft, TxtTop, TxtRight, TxtBottom);
                        Visual.Update();
                        break;

                    case Key.NumPad1:
                        TxtLeft = TxtRight = TxtTop = TxtBottom = 0;
                        Visual.visual_Grid.HorizontalAlignment = HorizontalAlignment.Left;
                        Visual.visual_Grid.VerticalAlignment = VerticalAlignment.Bottom;
                        Visual.visual_Grid.Margin = new Thickness(TxtLeft, TxtTop, TxtRight, TxtBottom);
                        Visual.Update();
                        break;

                    case Key.NumPad3:
                        TxtLeft = TxtRight = TxtTop = TxtBottom = 0;
                        Visual.visual_Grid.HorizontalAlignment = HorizontalAlignment.Right;
                        Visual.visual_Grid.VerticalAlignment = VerticalAlignment.Bottom;
                        Visual.visual_Grid.Margin = new Thickness(TxtLeft, TxtTop, TxtRight, TxtBottom);
                        Visual.Update();
                        break;
                }

        }

        private void tbx1_msg_TextChanged(object sender, TextChangedEventArgs e)
        {
            Visual.textblock_message.Text = (sender as TextBox).Text;
        }

        private void tbx4_tmr_TextChanged(object sender, TextChangedEventArgs e)
        {
            uint x = 0;
            uint.TryParse((sender as TextBox).Text, out x);
        }

        private void rad_Checked(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton).Tag.ToString())
            {
                case "1": Visual.textblock_message.Visibility = Visibility.Visible; break;
                case "2": Visual.textblock_time.Visibility = Visibility.Visible; break;
                case "3": Visual.textblock_stopwatch.Visibility = Visibility.Visible; break;
                case "4": Visual.textblock_timer.Visibility = Visibility.Visible; break;
                case "5": Visual.image.Visibility = Visibility.Visible; break;
                case "6":
                    Visual.browserGrid.Visibility = Visual.prGrid.Visibility = Visibility.Visible;
                    Visual.prGrid.Width = prWidth;
                    Visual.prGrid.Height = prHeight;
                    Visual.Update();
                    break;
                case "7":
                    Visual.videoplayerGrid.Visibility = Visibility.Visible;
                    Visual.videoplayerGrid.Width = prWidth;
                    Visual.videoplayerGrid.Height = prHeight;
                    break;
            }
        }

        private void rad_Unchecked(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton).Tag.ToString())
            {
                case "1": Visual.textblock_message.Visibility = Visibility.Collapsed; break;
                case "2": Visual.textblock_time.Visibility = Visibility.Collapsed; break;
                case "3": Visual.textblock_stopwatch.Visibility = Visibility.Collapsed; break;
                case "4": Visual.textblock_timer.Visibility = Visibility.Collapsed; break;
                case "5": Visual.image.Visibility = Visibility.Collapsed; break;
                case "6": Visual.browserGrid.Visibility = Visual.prGrid.Visibility = Visibility.Collapsed;
                    Visual.prGrid.Width = 0;
                    Visual.prGrid.Height = 0;
                    Visual.Update();
                    break;
                case "7": Visual.videoplayerGrid.Visibility = Visibility.Collapsed;
                    Visual.videoplayerGrid.Width = 0;
                    Visual.videoplayerGrid.Height = 0;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int w = 100, h = 100;
            int.TryParse(brWidthTb.Text, out w);
            int.TryParse(brHeightTb.Text, out h);
            Visual.prGrid.Width = prWidth = w; Visual.prGrid.Height = prHeight = h;
            Visual.videoplayerGrid.Width = prWidth = w; Visual.videoplayerGrid.Height = prHeight = h;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            programComboBox.ItemsSource = Visual.programComboBox.ItemsSource;
        }

        private void ProgramComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Visual.programComboBox.SelectedIndex = programComboBox.SelectedIndex;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            VideoLinks = Directory.GetFiles(@"D:\Video\", "*.mp4");
            VideoIndex = 0;
            if (VideoLinks.Length > 0)
                try
                {
                    Visual.SetVideoLink(VideoLinks[VideoIndex]);
                }
                catch { }
        }
    }
}
