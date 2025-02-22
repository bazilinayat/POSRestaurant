﻿using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Controls;
using POSRestaurant.Pages;
using SettingLibrary;

namespace POSRestaurant.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        /// <summary>
        /// ServiceProvider for the DIs
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor for the TablesViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public ShellViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Command to navigate to OrdersPage
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task NavigateToOrdersPage()
        {
            // Navigate to ProfilePage
            var tabv = _serviceProvider.GetRequiredService<OrdersViewModel>();
            await Application.Current.MainPage.Navigation.PushAsync(new OrdersPage(tabv));
        }

        /// <summary>
        /// Command to navigate to ManageMenuItemPage
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task NavigateToSettings()
        {
            // Navigate to SettingsPage
            var settingVM = _serviceProvider.GetRequiredService<SettingsViewModel>();
            await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage(settingVM));
        }

        /// <summary>
        /// Command to open the support popup
        /// </summary>
        /// <returns>Return a Task Object</returns>
        [RelayCommand]
        private async Task SupportCommand()
        {
            var setting = _serviceProvider.GetRequiredService<SettingService>();
            var helpPopup = new HelpPopup(setting);
            await Shell.Current.ShowPopupAsync(helpPopup);
        }
    }
}
