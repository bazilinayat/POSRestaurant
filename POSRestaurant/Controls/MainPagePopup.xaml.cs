using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Models;
using POSRestaurant.Pages;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Controls;

/// <summary>
/// Class for Mainpooup
/// </summary>
public partial class MainPagePopup : Popup
{
    /// <summary>
    /// Initialize MainPage
    /// </summary>
    /// <param name="homeViewModel">DI for HomeViewModel</param>
	public MainPagePopup(HomeViewModel homeViewModel, TableModel tableModel)
	{
		InitializeComponent();

        Content = new MainPageContent(this, homeViewModel, tableModel);
    }

}