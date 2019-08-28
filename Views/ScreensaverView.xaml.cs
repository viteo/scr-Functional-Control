﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sharpsaver.Views
{
    public partial class ScreensaverView : Window
    {
        private bool isPreviewWindow;
        private Point lastMousePosition = default;

        public ScreensaverView()
        {
            InitializeComponent();
            isPreviewWindow = false;
        }
        public ScreensaverView(IntPtr previewHandle)
        {            
            InitializeComponent();
            WindowState = WindowState.Normal;
            isPreviewWindow = true;            

            IntPtr windowHandle = new WindowInteropHelper(GetWindow(this)).EnsureHandle();

            // Set the preview window as the parent of this window
            InteropHelper.SetParent(windowHandle, previewHandle);

            // Make this window a tool window while preview.
            // A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
            // GWL_EXSTYLE = -20, WS_EX_TOOLWINDOW = 0x00000080L
            InteropHelper.SetWindowLong(windowHandle, -20, 0x00000080L);
            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            InteropHelper.SetWindowLong(windowHandle, -16, 0x40000000L);

            // Place the window inside the parent
            InteropHelper.GetClientRect(previewHandle, out Rect parentRect);

            Width = parentRect.Width;
            Height = parentRect.Height;            
        }

        private void Draw()
        {
            Rectangle bkgRect = new Rectangle();
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Colors.White;
            bkgRect.Fill = brush;
            var size = this.Field.Width > this.Field.Height ? this.Field.Height : this.Field.Width;
            bkgRect.Width = size;
            bkgRect.Height = size;
            Canvas.SetLeft(bkgRect, (this.Field.Width - size) / 2);

            Ellipse ellipse = new Ellipse();
            ellipse.Width = size;
            ellipse.Height = size;
            ellipse.Stroke = new SolidColorBrush(Colors.Gray);
            ellipse.StrokeThickness = size/100;
            Canvas.SetLeft(ellipse, (this.Field.Width - size) / 2);
            this.Field.Children.Add(bkgRect);
            this.Field.Children.Add(ellipse);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPreviewWindow) return;

            Point pos = e.GetPosition(this);

            if (lastMousePosition != default)
            {
                if ((lastMousePosition - pos).Length > 3)
                {
                    Application.Current.Shutdown();
                }
            }
            lastMousePosition = pos;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isPreviewWindow) return;

            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (isPreviewWindow) return;

            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
        }
    }
}
