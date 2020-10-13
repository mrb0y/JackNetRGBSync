﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace RGBSyncPlus.UI.Tabs
{
    /// <summary>
    /// Interaction logic for ProfilesTab.xaml
    /// </summary>
    public partial class ProfilesTab : UserControl
    {

        ProfileTabViewModel vm => (ProfileTabViewModel)DataContext;
        public ProfilesTab()
        {
            InitializeComponent();
        }


        private void DeleteProfile(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ProfileTabViewModel.ProfileItemViewModel dc = button.DataContext as ProfileTabViewModel.ProfileItemViewModel;
            vm.DeleteProfile(dc);
        }

        private void ToggleShowTriggers(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ProfileTabViewModel.ProfileItemViewModel dc = button.DataContext as ProfileTabViewModel.ProfileItemViewModel;
            vm.ShowTriggers = !vm.ShowTriggers;
            vm.CurrentProfile = dc;

            var tempProfile = ApplicationManager.Instance.GetProfileFromName(dc.Name);
            vm.CurrentProfile.ProfileId = tempProfile.Id;
        }

        private void AddNewProfile(object sender, RoutedEventArgs e)
        {
            vm.CreateNewProfileUI();
        }

        private void CreateProfile(object sender, RoutedEventArgs e)
        {
            vm.CreateProfile();
        }

        private void EditProfile(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ProfileTabViewModel.ProfileItemViewModel dc = button.DataContext as ProfileTabViewModel.ProfileItemViewModel;
            vm.EditProfile(dc);
        }

        private void ToggleShowProfile(object sender, RoutedEventArgs e)
        {
            vm.ShowEditProfile = !vm.ShowEditProfile;
        }

        private void CloseShowProfile(object sender, RoutedEventArgs e)
        {
            vm.ShowEditProfile = false;
            vm.SaveProfile();
        }


        private void SetProfile(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ProfileTabViewModel.ProfileItemViewModel dc = button.DataContext as ProfileTabViewModel.ProfileItemViewModel;
            vm.SwitchToProfile(dc);
        }

        private void SetTrigger(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ProfileTriggerManager.ProfileTriggerEntry dc = button.DataContext as ProfileTriggerManager.ProfileTriggerEntry;
            dc.TriggerType = (string)button.Tag;
            Debug.WriteLine(dc);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CloseTriggerList(object sender, RoutedEventArgs e)
        {
            vm.ShowTriggers = false;
        }

        private void ToggleTriggerWhenNotFound(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ProfileTriggerManager.ProfileTriggerEntry dc = button.DataContext as ProfileTriggerManager.ProfileTriggerEntry;
            dc.TriggerWhenNotFound = !dc.TriggerWhenNotFound;
        }

        private void AddNewTrigger(object sender, RoutedEventArgs e)
        {
            vm.CreateNewTrigger();
        }

        private void ToggleExpanded(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ProfileTriggerManager.ProfileTriggerEntry dc = button.DataContext as ProfileTriggerManager.ProfileTriggerEntry;

            dc.Expanded = !dc.Expanded;
        }
    }
}
