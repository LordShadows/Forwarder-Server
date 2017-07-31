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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Forwarder_Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Объявление переменных для изменения размеров окна
        bool isRightResize = false;
        bool isLeftResize = false;
        bool isBottomResize = false;
        bool isTopResize = false;
        bool isRightBottomResize = false;
        bool isLeftTopResize = false;
        bool isRightTopResize = false;
        bool isLeftBottomResize = false;
        double positionRightResize = 0;
        double positionLeftResize = 0;
        double positionBottomResize = 0;
        double positionTopResize = 0;
        double positionXRightBottomResize = 0;
        double positionYRightBottomResize = 0;
        double positionXLeftTopResize = 0;
        double positionYLeftTopResize = 0;
        double positionXRightTopResize = 0;
        double positionYRightTopResize = 0;
        double positionXLeftBottomResize = 0;
        double positionYLeftBottomResize = 0;
        #endregion

        const double minWidth = 500;
        const double minHight = 300;

        Thread _serverThread;

        public static List<String> Journal = new List<String>();

        public MainWindow()
        {
            InitializeComponent();
            Properties.Settings.Default.Maximized = false;
            Properties.Settings.Default.Save();

            Sources.Functions.MAINWINDOW = this;
            _serverThread = new Thread(Sources.Server.StartServer)
            {
                Name = "Сервер Forwarder 1.0",
                IsBackground = true
            };
            _serverThread.Start();
        }

        #region Реализация кнопок управления
        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HeaderButton_MouseEnter(object sender, MouseEventArgs e)
        {
            close.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\close-hover.png", UriKind.Relative)));
            min.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\min-hover.png", UriKind.Relative)));
            max.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\max-hover.png", UriKind.Relative)));
        }

        private void HeaderButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsActive)
            {
                close.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\close-normal.png", UriKind.Relative)));
                min.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\min-normal.png", UriKind.Relative)));
                max.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\max-normal.png", UriKind.Relative)));
            }
            else
            {
                close.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\nofocus.png", UriKind.Relative)));
                min.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\nofocus.png", UriKind.Relative)));
                max.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\nofocus.png", UriKind.Relative)));
            }
        }

        private void Max_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.Maximized)
            {
                SetWindowStage("Normal");
            }
            else
            {
                SetWindowStage("Maximized");
            }

        }

        private void Min_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void SetWindowStage(String _stage)
        {
            switch (_stage)
            {
                case "Maximized":
                    {
                        Properties.Settings.Default.Height = this.Height;
                        Properties.Settings.Default.Width = this.Width;
                        Properties.Settings.Default.Top = this.Top;
                        Properties.Settings.Default.Left = this.Left;

                        this.Height = SystemParameters.WorkArea.Height;
                        this.Width = SystemParameters.WorkArea.Width;
                        this.Top = SystemParameters.WorkArea.Top;
                        this.Left = SystemParameters.WorkArea.Left;
                        this.body.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);

                        Properties.Settings.Default.Maximized = true;
                        Properties.Settings.Default.Save();
                        break;
                    }

                case "Normal":
                    {
                        this.body.Margin = new Thickness(20.0, 20.0, 20.0, 20.0);
                        this.Height = Properties.Settings.Default.Height;
                        this.Width = Properties.Settings.Default.Width;
                        this.Top = Properties.Settings.Default.Top;
                        this.Left = Properties.Settings.Default.Left;

                        Properties.Settings.Default.Maximized = false;
                        Properties.Settings.Default.Save();

                        break;
                    }
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                SetWindowStage("Maximized");
                this.WindowState = WindowState.Normal;
            }
        }
        #endregion

        #region Реализация перемещения окна
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (!Properties.Settings.Default.Maximized)
                {
                    SetWindowStage("Maximized");
                }
                else
                {
                    SetWindowStage("Normal");
                }
            }
            else if (!Properties.Settings.Default.Maximized) this.DragMove();
        }
        #endregion

        #region Реализация изменения размеров окна
        private void RightResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRightResize && !Properties.Settings.Default.Maximized)
            {
                double newWidth = this.Width + (e.GetPosition(this).X - positionRightResize);
                if (newWidth > minWidth + 40)
                {
                    this.Width = newWidth;
                    positionRightResize = e.GetPosition(this).X;
                }
            }
        }

        private void RightBottomResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRightBottomResize && !Properties.Settings.Default.Maximized)
            {
                double newWidth = this.Width + (e.GetPosition(this).X - positionXRightBottomResize);
                double newHeight = this.Height + (e.GetPosition(this).Y - positionYRightBottomResize);
                if (newWidth > minWidth + 40)
                {
                    this.Width = newWidth;
                    positionXRightBottomResize = e.GetPosition(this).X;
                }
                if (newHeight > minHight + 40)
                {
                    this.Height = newHeight;
                    positionYRightBottomResize = e.GetPosition(this).Y;
                }
            }
        }

        private void LeftTopResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLeftTopResize && !Properties.Settings.Default.Maximized)
            {
                double newWidth = this.Width - (e.GetPosition(this).X - positionXLeftTopResize);
                double newHeight = this.Height - (e.GetPosition(this).Y - positionYLeftTopResize);
                double newLeft = this.Left + (e.GetPosition(this).X - positionXLeftTopResize);
                double newTop = this.Top + (e.GetPosition(this).Y - positionYLeftTopResize);
                if (newWidth > minWidth + 40)
                {
                    this.Width = newWidth;
                    this.Left = newLeft;
                }
                if (newHeight > minHight + 40)
                {
                    this.Height = newHeight;
                    this.Top = newTop;
                }
            }
        }

        private void LeftBottomResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLeftBottomResize && !Properties.Settings.Default.Maximized)
            {
                double newWidth = this.Width - (e.GetPosition(this).X - positionXLeftBottomResize);
                double newHeight = this.Height + (e.GetPosition(this).Y - positionYLeftBottomResize);
                double newLeft = this.Left + (e.GetPosition(this).X - positionXLeftBottomResize);
                if (newWidth > minWidth + 40)
                {
                    this.Width = newWidth;
                    this.Left = newLeft;
                }
                if (newHeight > minHight + 40)
                {
                    this.Height = newHeight;
                    positionYLeftBottomResize = e.GetPosition(this).Y;
                }
            }
        }

        private void RightTopResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRightTopResize && !Properties.Settings.Default.Maximized)
            {
                double newWidth = this.Width + (e.GetPosition(this).X - positionXRightTopResize);
                double newHeight = this.Height - (e.GetPosition(this).Y - positionYRightTopResize);
                double newTop = this.Top + (e.GetPosition(this).Y - positionYRightTopResize);
                if (newWidth > minWidth + 40)
                {
                    this.Width = newWidth;
                    positionXRightTopResize = e.GetPosition(this).X;
                }
                if (newHeight > minHight + 40)
                {
                    this.Height = newHeight;
                    this.Top = newTop;
                }
            }
        }

        private void LeftResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (isLeftResize && !Properties.Settings.Default.Maximized)
            {
                double newWidth = this.Width - (e.GetPosition(this).X - positionLeftResize);
                double newLeft = this.Left + (e.GetPosition(this).X - positionLeftResize);
                if (newWidth > minWidth + 40)
                {
                    this.Width = newWidth;
                    this.Left = newLeft;
                }
            }
        }

        private void BottomResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (isBottomResize && !Properties.Settings.Default.Maximized)
            {
                double newHeight = this.Height + (e.GetPosition(this).Y - positionBottomResize);
                if (newHeight > minHight + 40)
                {
                    this.Height = newHeight;
                    positionBottomResize = e.GetPosition(this).Y;
                }
            }
        }

        private void TopResize_MouseMove(object sender, MouseEventArgs e)
        {
            if (isTopResize && !Properties.Settings.Default.Maximized)
            {
                double newHeight = this.Height - (e.GetPosition(this).Y - positionTopResize);
                double newTop = this.Top + (e.GetPosition(this).Y - positionTopResize);
                if (newHeight > minHight + 40)
                {
                    this.Height = newHeight;
                    this.Top = newTop;
                }
            }
        }

        private void Resize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.CaptureMouse();
            switch (rect.Name)
            {
                case "rightResize":
                    isRightResize = true;
                    positionRightResize = e.GetPosition(this).X;
                    break;
                case "leftResize":
                    isLeftResize = true;
                    positionLeftResize = e.GetPosition(this).X;
                    break;
                case "bottomResize":
                    isBottomResize = true;
                    positionBottomResize = e.GetPosition(this).Y;
                    break;
                case "topResize":
                    isTopResize = true;
                    positionTopResize = e.GetPosition(this).Y;
                    break;
                case "rightBottomResize":
                    isRightBottomResize = true;
                    positionXRightBottomResize = e.GetPosition(this).X;
                    positionYRightBottomResize = e.GetPosition(this).Y;
                    break;
                case "leftTopResize":
                    isLeftTopResize = true;
                    positionXLeftTopResize = e.GetPosition(this).X;
                    positionYLeftTopResize = e.GetPosition(this).Y;
                    break;
                case "rightTopResize":
                    isRightTopResize = true;
                    positionXRightTopResize = e.GetPosition(this).X;
                    positionYRightTopResize = e.GetPosition(this).Y;
                    break;
                case "leftBottomResize":
                    isLeftBottomResize = true;
                    positionXLeftBottomResize = e.GetPosition(this).X;
                    positionYLeftBottomResize = e.GetPosition(this).Y;
                    break;
            }
        }

        private void Resize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.ReleaseMouseCapture();
            switch (rect.Name)
            {
                case "rightResize":
                    isRightResize = false;
                    break;
                case "leftResize":
                    isLeftResize = false;
                    break;
                case "bottomResize":
                    isBottomResize = false;
                    break;
                case "topResize":
                    isTopResize = false;
                    break;
                case "rightBottomResize":
                    isRightBottomResize = false;
                    break;
                case "leftTopResize":
                    isLeftTopResize = false;
                    break;
                case "rightTopResize":
                    isRightTopResize = false;
                    break;
                case "leftBottomResize":
                    isLeftBottomResize = false;
                    break;
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Sources.Server.SendGlobalMessage(tbTextMessage.Text);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            close.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\close-normal.png", UriKind.Relative)));
            min.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\min-normal.png", UriKind.Relative)));
            max.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\max-normal.png", UriKind.Relative)));
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF454545"), 0.0));
            linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF404040"), 1.0));
            header.Background = linearGradientBrush;
            header.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF3C3C3C"));
            background.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF3C3C3C"));
            mainTitile.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#99FFFFFF"));
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            close.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\nofocus.png", UriKind.Relative)));
            min.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\nofocus.png", UriKind.Relative)));
            max.Fill = new ImageBrush(new BitmapImage(new Uri(@"Resources\nofocus.png", UriKind.Relative)));
            header.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF6F6F6"));
            header.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB0B0B0"));
            background.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB0B0B0"));
            mainTitile.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB0B0B0"));
        }

        private void ServerButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tcPages.SelectedIndex = 0;
        }

        private void UserBotton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tcPages.SelectedIndex = 1;
        }

        private void GlobalMessageBotton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tcPages.SelectedIndex = 2;
        }

        private void TCPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tcPages.SelectedIndex)
            {
                case 0:
                    serverButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC8C8C8"));
                    userBotton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
                    globalMessageBotton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
                    break;
                case 1:
                    userBotton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC8C8C8"));
                    serverButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
                    globalMessageBotton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
                    break;
                case 2:
                    globalMessageBotton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC8C8C8"));
                    serverButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
                    userBotton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
                    break;
            }
        }

        private void exitMessageBotton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            exitMessageBotton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC8C8C8"));
            Sources.Server.Shutdown();
            App.Current.Shutdown();
        }

        private void exitMessageBotton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            exitMessageBotton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
        }

        public class AccountDataContext
        {
            public AccountDataContext(string id, string name)
            {
                this.ID = id;
                if(name == " ()")
                    this.Name = "Пользователь не авторизован";
                else
                    this.Name = name;
            }

            public string ID { get; private set; }

            public string Name { get; private set; }
        }

        private void TBTextMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbMessages.Items.Clear();
            foreach (var item in Journal)
            {
                if (item.ToLower().Contains(tbTextMessage.Text.ToLower())) lbMessages.Items.Add(item);
            }
        }

        private void BClear_Click(object sender, RoutedEventArgs e)
        {
            Journal.Clear();
            lbMessages.Items.Clear();
        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog()
            {
                FileName = "Document",
                DefaultExt = ".text",
                Filter = "Text documents (.txt)|*.txt"
            };

            if(saveDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveDialog.FileName, Journal, Encoding.UTF8);
            }
        }

        private void BCloseConnection_Click(object sender, RoutedEventArgs e)
        {
            if(lbUsers.SelectedIndex >= 0)
            {
                Sources.Server.EndUser(Sources.Server.USERSLIST.Find(x => x.UserID == ((AccountDataContext)lbUsers.SelectedItem).ID));
            }
        }

        private void BSend_Click(object sender, RoutedEventArgs e)
        {
            if(tbMessage.Text != "")
            {
                Sources.Server.SendGlobalMessage(tbMessage.Text);
            }
        }
    }
}