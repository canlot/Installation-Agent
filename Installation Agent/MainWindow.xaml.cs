﻿using Installation.Models;
using Installation_Agent.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Installation.Models.Executables;

namespace Installation_Agent
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ViewController viewController;
        public MainWindow()
        {
            InitializeComponent();
            viewController = new ViewController();
            this.DataContext = viewController;
            
            
        }
        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await viewController.RunAsync();
            ListBoxJobs.ItemsSource = viewController.Scripts;
            //DebugBox.Text = "Test";
            ////viewController.Apps.Add(new Job
            ////{

            ////}.WithNewGuiD());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonInstallSoftware_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)sender);
            DependencyObject parentparent = VisualTreeHelper.GetParent((DependencyObject)parent);
            DependencyObject parentparentparent = VisualTreeHelper.GetParent((DependencyObject)parentparent);
            DependencyObject parentparentparentparent = VisualTreeHelper.GetParent((DependencyObject)parentparentparent);
            ListBoxItem item = (ListBoxItem)parentparentparentparent;
            ScriptExecutable scriptExecutable = (ScriptExecutable)item.DataContext;
            //Job job = (Job)item.DataContext;
            await viewController.RunJob(scriptExecutable);
        }
    }
}
