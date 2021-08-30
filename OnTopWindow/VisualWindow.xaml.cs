using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OnTopWindow
{
    /// <summary>
    /// Interaction logic for VisualWindow.xaml
    /// </summary>
    public partial class VisualWindow : Window
    {
        readonly WindowInteropHelper _wih;

        const int HOUR = 3600;
        const int MINUTE = 60;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        public bool? IsStopWatchStopped { get; set; } = null;
        public bool IsTimerStarted { get; set; } = false;
        string currentTime = string.Empty;

        DispatcherTimer timer = new DispatcherTimer();

        VisualViewModel viewModel;

        public VisualWindow()
        {
            InitializeComponent();

            //  https://social.msdn.microsoft.com/Forums/vstudio/en-US/a813d92b-3d15-4f10-9984-5381517157d4/how-to-display-and-update-time-in-wpf-application?forum=wpf
            DispatcherTimer clock = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                /*if (DateTime.Now.Minute > 16) textblock_time.Foreground = new SolidColorBrush(Colors.Lime);
                else textblock_time.Foreground = new SolidColorBrush(Colors.Red);*/
                DateTime dt = DateTime.Now;
                textblock_time.Text = dt.ToString("HH:mm:ss"); // "HH:mmTongue Tieds"
                if (MainWindow.Main.cuckoo_CheckBox.IsChecked == true && dt.Minute == 0 && dt.Second == 0)
                    try
                    {
                        MainWindow.Main.MediaPlayer.Open(new Uri(MainWindow.Main.CuckooSoundPath));
                        MainWindow.Main.MediaPlayer.Play();
                    }
                    catch
                    {
                        SystemSounds.Beep.Play();
                    }
            }, Dispatcher);


            dispatcherTimer.Tick += new EventHandler(dt_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);


            _wih = new WindowInteropHelper(this);

            viewModel = DataContext as VisualViewModel;

            Loaded += (s, e) => viewModel.Init(_wih.Handle, GetRect());

            prGrid.SizeChanged += (s, e) => viewModel.SizeChanged(GetRect());
        }

        //  https://stackoverflow.com/questions/2842667/how-to-create-a-semi-transparent-window-in-wpf-that-allows-mouse-events-to-pass
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        //  https://stackoverflow.com/questions/43044880/creating-stopwatch-in-wpf-time-precison
        void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                currentTime = string.Format("{0}:{1:00}:{2:00}:{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                textblock_stopwatch.Text = currentTime;
            }
        }

        public void StartStopwatch()
        {
            stopWatch.Start();
            dispatcherTimer.Start();
            IsStopWatchStopped = false;
        }

        public void StopStopwatch()
        {
            if (IsStopWatchStopped == true) // reset
            {
                IsStopWatchStopped = null;
                stopWatch.Reset();
                textblock_stopwatch.Text = "0:00:00:000";
            }
            else if (stopWatch.IsRunning) // stop
            {
                stopWatch.Stop();
                IsStopWatchStopped = true;
            }
            //elapsedtimeitem.Items.Add(currentTime);
        }

        //  https://wcoder.github.io/notes/timer-in-wpf-app

        private void timer_Tick(object sender, EventArgs e)
        {
            if (MainWindow.Main.TimerTimeLeft <= 0)
            {
                TimerStopped();
            }
            else
            {
                MainWindow.Main.TimerTimeLeft--;
                ShowTime(MainWindow.Main.TimerTimeLeft);
            }
        }

        public void StartTimer()
        {
            MainWindow.Main.TimerTimeLeft = MainWindow.Main.TimerSeconds;
            ShowTime(MainWindow.Main.TimerTimeLeft);
            textblock_timer.Foreground = new SolidColorBrush(Colors.Yellow);
            timer.Start();
            IsTimerStarted = true;
        }

        public void StopTimer()
        {
            IsTimerStarted = false;
            timer.Stop();
            textblock_timer.Foreground = new SolidColorBrush(Colors.White);
            ShowTime(MainWindow.Main.TimerSeconds);
        }

        public void TimerStopped()
        {
            try
            {
                MainWindow.Main.MediaPlayer.Open(new Uri(MainWindow.Main.SoundPath));
                MainWindow.Main.MediaPlayer.Play();
            }
            catch
            {
                SystemSounds.Beep.Play();
            }
            finally
            {
                StopTimer();
            }
        }

        public void ShowTime(int time)
        {
            int h = time / HOUR;
            int m = time / MINUTE - h * MINUTE;
            int s = time - h * HOUR - m * MINUTE;
            if (h > 0) textblock_timer.Text = string.Format("{0}:{1:00}:{2:00}", h, m, s);
            else textblock_timer.Text = string.Format("{0}:{1:00}", m, s);

            if (MainWindow.Main.TimerSet != MainWindow.TimerSetting.Standard)
            {
                Run hRun = new Run { Text = $"\nHour: {h}", Foreground = new SolidColorBrush(Colors.White) };
                Run mRun = new Run { Text = $"\nMinutes: {m}", Foreground = new SolidColorBrush(Colors.White) };
                Run sRun = new Run { Text = $"\nSeconds: {s}", Foreground = new SolidColorBrush(Colors.White) };
                switch (MainWindow.Main.TimerSet)
                {
                    case MainWindow.TimerSetting.Hours: hRun.Foreground = new SolidColorBrush(Colors.Lime); break;
                    case MainWindow.TimerSetting.Minutes: mRun.Foreground = new SolidColorBrush(Colors.Lime); break;
                    case MainWindow.TimerSetting.Seconds: sRun.Foreground = new SolidColorBrush(Colors.Lime); break;
                }
                textblock_timer.Inlines.Add(hRun);
                textblock_timer.Inlines.Add(mRun);
                textblock_timer.Inlines.Add(sRun);
            }
        }

        public void SetVideoLink(string link)
        {
            mePlayer.Source = new Uri(link);
        }
        public void PauseVideo()
        {
            mePlayer.Pause();
        }
        public void PlayVideo()
        {
            
            mePlayer.Play();
        }
        public void RewindVideo(int seconds)
        {
            if (seconds >= 0) mePlayer.Position = mePlayer.Position.Add(new TimeSpan(0, 0, seconds));
            else mePlayer.Position = mePlayer.Position.Subtract(new TimeSpan(0, 0, -seconds));
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            //  https://stackoverflow.com/questions/3729369/topmost-is-not-topmost-always-wpf
            Topmost = true;
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            Activate();
        }


        Rect GetRect()
        {
            if (visual_Grid.VerticalAlignment == VerticalAlignment.Top)
            {
                if (visual_Grid.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    return new Rect(
                        20 + (int)visual_Grid.Margin.Left,
                        20 + (int)visual_Grid.Margin.Top,
                        20 + (int)visual_Grid.Margin.Left + (int)prGrid.ActualWidth,
                        20 + (int)visual_Grid.Margin.Top + (int)prGrid.ActualHeight);
                }
                else if (visual_Grid.HorizontalAlignment == HorizontalAlignment.Right)
                {
                    return new Rect(
                        (int)ActualWidth - (20 + (int)visual_Grid.Margin.Right + (int)prGrid.ActualWidth),
                        20 + (int)visual_Grid.Margin.Top,
                        (int)ActualWidth - (20 + (int)visual_Grid.Margin.Right),
                        20 + (int)visual_Grid.Margin.Top + (int)prGrid.ActualHeight);
                }
            }
            else if (visual_Grid.VerticalAlignment == VerticalAlignment.Bottom)
            {
                if (visual_Grid.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    return new Rect(
                        20 + (int)visual_Grid.Margin.Left,
                        (int)ActualHeight - (20 + (int)visual_Grid.Margin.Bottom + (int)prGrid.ActualHeight),
                        20 + (int)visual_Grid.Margin.Left + (int)prGrid.ActualWidth,
                        (int)ActualHeight - (20 + (int)visual_Grid.Margin.Bottom));
                }
                else if (visual_Grid.HorizontalAlignment == HorizontalAlignment.Right)
                {
                    return new Rect(
                        (int)ActualWidth - (20 + (int)visual_Grid.Margin.Right + (int)prGrid.ActualWidth),
                        (int)ActualHeight - (20 + (int)visual_Grid.Margin.Bottom + (int)prGrid.ActualHeight),
                        (int)ActualWidth - (20 + (int)visual_Grid.Margin.Right),
                        (int)ActualHeight - (20 + (int)visual_Grid.Margin.Bottom));
                }
            }
            //return new Rect(20, 20, (int)prGrid.ActualWidth, (int)prGrid.ActualHeight);
            throw new NotSupportedException();
        }

        public void Update()
        {
            viewModel.SizeChanged(GetRect());
        }
    }

    //============================================================================================

    public static class WindowsServices
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }
}

