using Installation.Models;
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
using Serilog;

namespace Installation_Agent
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //public ViewController viewController = new ViewController();
        private bool closingFromContextMenu = false;
        public MainWindow()
        {
            //this.DataContext = viewController;
            InitializeComponent();
            initializeTrayIcon();
            
            //ListBoxJobs.ItemsSource = viewController.ExecutableCollection;
            //ListBoxJobs.IsSynchronizedWithCurrentItem = true;
#if DEBUG
            closingFromContextMenu = true;
#endif

        }

        private void initializeTrayIcon()
        {
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add("Beenden");
            contextMenu.MenuItems[0].Click +=
                delegate (object sender, EventArgs args)
                {
                    closingFromContextMenu = true;
                    this.Close();
                };
            ni.ContextMenu = contextMenu;
            ni.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closingFromContextMenu)
            {
                e.Cancel = true;
                this.Hide();
            }
                
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await viewController.RunAsync();


        }

        private async void ButtonRunScript_Click(object sender, RoutedEventArgs e)
        {

            if (ListBoxJobs.SelectedItem != null)
            {
                if(ListBoxJobs.SelectedItem is ScriptExecutable)
                {
                    ScriptExecutable scriptExecutable = (ScriptExecutable)ListBoxJobs.SelectedItem;
                    scriptExecutable.CurrentlyExecuting = true;
                    await viewController.RunJob(scriptExecutable);
                }
            }
        }

        private async void ButtonInstallApplication_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxJobs.SelectedItem != null)
            {
                if (ListBoxJobs.SelectedItem is ApplicationExecutable)
                {
                    ApplicationExecutable applicationExecutable = (ApplicationExecutable)ListBoxJobs.SelectedItem;
                    applicationExecutable.CurrentlyExecuting = true;
                    await viewController.InstallApplication(applicationExecutable);
                }
            }
        }

        private async void ButtonReinstallApplication_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxJobs.SelectedItem != null)
            {
                if (ListBoxJobs.SelectedItem is ApplicationExecutable)
                {
                    ApplicationExecutable applicationExecutable = (ApplicationExecutable)ListBoxJobs.SelectedItem;
                    applicationExecutable.CurrentlyExecuting = true;
                    await viewController.ReinstallApplication(applicationExecutable);
                }
            }
        }

        private async void ButtonUninstallApplication_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxJobs.SelectedItem != null)
            {
                if (ListBoxJobs.SelectedItem is ApplicationExecutable)
                {
                    ApplicationExecutable applicationExecutable = (ApplicationExecutable)ListBoxJobs.SelectedItem;
                    applicationExecutable.CurrentlyExecuting = true;
                    await viewController.UninstallApplication(applicationExecutable);
                }
            }
            
        }

        private void AutoSuggestBoxSearchExecutables_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            viewController.SearchText = sender.Text;
            viewController.ExecutableCollection.Refresh();
        }


        private void ListBoxJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = (ListBox)sender;

            var item = listbox.SelectedItem;
            if (item != null)
            {
                InnerGrid.ColumnDefinitions[1].Width = GridLength.Auto;
                makeAllButtonsInvisible();

                if (item is IRunnable)
                    RunButton.Visibility = Visibility.Visible;
                if (item is IInstalable)
                    InstallButton.Visibility = Visibility.Visible;
                if (item is IReinstallable)
                    ReinstallButton.Visibility = Visibility.Visible;
                if (item is IUninstallable)
                    UninstallButton.Visibility = Visibility.Visible;
            }
            else
            {
                InnerGrid.ColumnDefinitions[1].Width = new GridLength(0);
            }
        }
        private void makeAllButtonsInvisible()
        {
            RunButton.Visibility = Visibility.Collapsed;
            InstallButton.Visibility = Visibility.Collapsed;
            ReinstallButton.Visibility = Visibility.Collapsed;
            UninstallButton.Visibility = Visibility.Collapsed;
        }
    }
}
