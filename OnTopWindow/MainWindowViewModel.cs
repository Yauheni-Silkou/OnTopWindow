//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WpfExtensions.Mvvm;

//namespace OnTopWindow
//{
//    class MainWindowViewModel : WindowViewModelBase
//    {
//        VisualViewModel Visual { get; set; } = new VisualViewModel();



//        private int _inputWidth;
//        public int InputWidth
//        {
//            get { return _inputWidth; }
//            set
//            {
//                if (_inputWidth != value)
//                {
//                    _inputWidth = value;
//                    OnPropertyChanged();
//                }
//            }
//        }


//        private int _inputHeight;
//        public int InputHeight
//        {
//            get { return _inputHeight; }
//            set
//            {
//                if (_inputHeight != value)
//                {
//                    _inputHeight = value;
//                    OnPropertyChanged();
//                }
//            }
//        }


//        private string[] _mediaLinks;
//        public string[] MediaLinks
//        {
//            get { return _mediaLinks; }
//            set
//            {
//                if (_mediaLinks != value)
//                {
//                    _mediaLinks = value;
//                    OnPropertyChanged();
//                }
//            }
//        }


//        private int _mediaIndex;
//        public int MediaIndex
//        {
//            get { return _mediaIndex; }
//            set
//            {
//                if (_mediaIndex != value)
//                {
//                    _mediaIndex = value;
//                    OnPropertyChanged();
//                }
//            }
//        }






//        private RelayCommand _changeSizeCommand;
//        public RelayCommand ChangeSizeCommand
//        {
//            get
//            {
//                return _changeSizeCommand ??= new RelayCommand(parameter =>
//                    {
//                        Visual.Width = InputWidth;
//                        Visual.Height = InputHeight;
//                    },
//                    parameter => true);
//            }
//            set { _changeSizeCommand = value; }
//        }


//        private RelayCommand _playerRefreshCommand;
//        public RelayCommand PlayerRefreshCommand
//        {
//            get
//            {
//                return _playerRefreshCommand ??= new RelayCommand(parameter =>
//                    {
//                        MediaLinks = Directory.GetFiles(@"D:\Video\", "*.mp4");
//                        MediaIndex = 0;
//                        if (MediaLinks.Length > 0)
//                            try
//                            {
//                                Visual.MediaUri = new Uri(MediaLinks[MediaIndex]);
//                            }
//                            catch { }
//                    },
//                    parameter => true);
//            }
//            set { _playerRefreshCommand = value; }
//        }



//    }
//}
