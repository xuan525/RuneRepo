﻿using JsonUtil;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RuneRepo.UI
{
    /// <summary>
    /// Interaction logic for RunePageItem.xaml
    /// </summary>
    public partial class RunePageItem : UserControl, INotifyPropertyChanged
    {
        public delegate void ApplyHendler(Json.Value value);
        public event ApplyHendler Apply;

        public delegate void DeleteHendler(RunePageItem runePageItem);
        public event DeleteHendler Delete;

        public delegate void UpdatedHendler(RunePageItem runePageItem);
        public event UpdatedHendler Updated;

        public Json.Value Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string PageName
        {
            get
            {
                return Value["name"];
            }
            set
            {
                Value["name"] = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PageName"));
                Updated?.Invoke(this);
            }
        }

        public BitmapSource Snapshot
        {
            get
            {
                InfoBorder.Visibility = Visibility.Hidden;
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)MainGrid.RenderSize.Width, (int)MainGrid.RenderSize.Height, 96, 96, PixelFormats.Default);
                renderTargetBitmap.Render(MainGrid);
                renderTargetBitmap.Freeze();
                InfoBorder.Visibility = Visibility.Visible;
                return renderTargetBitmap;
            }
        }

        public Point SnapShotMousePosition { get; private set; }

        public RunePageItem()
        {
            InitializeComponent();
            InfoBorder.Opacity = 0;
        }

        public RunePageItem(Json.Value value)
        {
            InitializeComponent();
            InfoBorder.Opacity = 0;

            Value = value;

            int primaryStyleId = value["primaryStyleId"];
            int subStyleId = value["subStyleId"];
            int perkId = value["selectedPerkIds"][0];

            SetEnvironment(primaryStyleId);
            SetSecond(primaryStyleId, subStyleId);
            SetConstruct(primaryStyleId);
            SetKeystone(primaryStyleId, perkId);

            this.DataContext = this;
        }

        public BitmapSource GetDraggingSnapshot(MouseEventArgs e, Point offset)
        {
            Point point = e.GetPosition(MainGrid);
            point.Offset(offset.X, offset.Y);
            SnapShotMousePosition = point;

            return Snapshot;
        }

        private void SetEnvironment(int primaryStyleId)
        {
            EnvironmentImage.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,/RuneRepo;component/images/construct/{0}/environment.jpg", primaryStyleId)));
        }

        private void SetSecond(int primaryStyleId, int subStyleId)
        {
            if (subStyleId == -1)
            {
                SecondImage.Source = null;
                return;
            }
            SecondImage.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,/RuneRepo;component/images/construct/{0}/second/{1}.png", primaryStyleId, subStyleId)));
        }

        private void SetConstruct(int primaryStyleId)
        {
            ConstructImage.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,/RuneRepo;component/images/construct/{0}/construct.png", primaryStyleId)));
        }

        private void SetKeystone(int primaryStyleId, int perkId)
        {
            if (perkId == -1)
            {
                KeystoneImage.Source = null;
                return;
            }
            KeystoneImage.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,/RuneRepo;component/images/construct/{0}/keystones/{1}.png", primaryStyleId, perkId)));
        }

        private void ApplyBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            Apply?.Invoke(Value);
        }

        private void DeleteBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            Delete?.Invoke(this);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (NameEditBox.IsFocused)
            {
                FrameworkElement parent = (FrameworkElement)this.Parent;
                while (!parent.Focus())
                {
                    parent = (FrameworkElement)parent.Parent;
                }
            }
        }

        private void InfoGrid_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}
