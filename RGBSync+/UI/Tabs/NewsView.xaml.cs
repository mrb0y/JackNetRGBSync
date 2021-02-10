﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Win32;
using RGBSyncStudio.Helper;

namespace RGBSyncStudio.UI.Tabs
{
    /// <summary>
    /// Interaction logic for NewsView.xaml
    /// </summary>
    public partial class NewsView : UserControl
    {
        NewsViewModel vm => this.DataContext as NewsViewModel;
        public void OpenUrl(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            e.Parameter.ToString().NavigateToUrlInDefaultBrowser();
        }
        public NewsView()
        {
            InitializeComponent();
        }

        private void CloseModal(object sender, RoutedEventArgs e)
        {
            vm.SelectedNewsItem = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vm.SelectedNewsItem = ((Button)sender).DataContext as NewsViewModel.NewsItemViewModel;
        }

    }
}
