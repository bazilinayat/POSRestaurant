using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Models;
using System.Runtime.CompilerServices;

namespace POSRestaurant.Controls;

/// <summary>
/// Control for Categoris List
/// </summary>
public partial class CategoriesListControl : ContentView
{
	/// <summary>
	/// Construstor for ControlView
	/// </summary>
	public CategoriesListControl()
	{
		InitializeComponent();
	}

	/// <summary>
	/// BindableProperty for Categories to be used on UI
	/// </summary>
	public static readonly BindableProperty CategoriesProperty =
		BindableProperty.Create(nameof(Categories), typeof(MenuCategoryModel[]), typeof(CategoriesListControl), Array.Empty<MenuCategoryModel>());

	/// <summary>
	/// Public property for Categories
	/// </summary>
	public MenuCategoryModel[] Categories
	{
		get => (MenuCategoryModel[])GetValue(CategoriesProperty);
		set => SetValue(CategoriesProperty, value);
	}

	/// <summary>
	/// Event to be called when Category gets changed
	/// </summary>
	public event Action<MenuCategoryModel> OnCategorySelected;

	/// <summary>
	/// Command to run when category is changed on UI
	/// </summary>
	/// <param name="category">Selected Category</param>
	[RelayCommand]
	private void SelectCategory(MenuCategoryModel category) =>
		OnCategorySelected?.Invoke(category);

}