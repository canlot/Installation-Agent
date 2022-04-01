﻿using Installation.Models;
using Installation_Agent.Controller;
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

namespace Installation_Agent
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewController viewController = new ViewController();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewController;
            ListBoxJobs.ItemsSource = viewController.Apps;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewController.Apps.Add(new Job
            {
                
            }.WithNewGuiD());

        }

        private void ButtonInstallSoftware_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)sender);
            DependencyObject parentparent = VisualTreeHelper.GetParent((DependencyObject)parent);
            DependencyObject parentparentparent = VisualTreeHelper.GetParent((DependencyObject)parentparent);
            DependencyObject parentparentparentparent = VisualTreeHelper.GetParent((DependencyObject)parentparentparent);
            ListBoxItem item = (ListBoxItem)parentparentparentparent;
            Job job = (Job)item.DataContext;
            viewController.RunJob(job);
        }
    }
}