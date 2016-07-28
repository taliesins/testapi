﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;

namespace SampleApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        void CheckStyleBox(object sender, RoutedEventArgs e)
        {
            CheckBox box=(CheckBox)sender;
            if (box.IsChecked==true)
            {
                BackgroundLayer.Visibility = Visibility.Visible;
            }
            else
            {
                BackgroundLayer.Visibility = Visibility.Hidden;
            }
        }

        private void AppendText(object sender, RoutedEventArgs e)
        {
            outputTextBox.Text = Append(outputTextBox.Text, inputTextBox.Text + "\n");
        }

        private string Append(string first, string second)
        {
            return first + second;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            // This code allows the content to be resized by the user
            // The app loads with a fixed grid size initially to allow
            // for visual verification testing
            this.SizeToContent = SizeToContent.Manual;
            mainGrid.Height = Double.NaN;
            mainGrid.Width = Double.NaN;
        }
        
    }
}
