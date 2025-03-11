using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.Models;

namespace POSRestaurant.Controls;

/// <summary>
/// Control for Menu Item List
/// </summary>
public partial class MenuItemsListControl : ContentView
{
    /// <summary>
	/// Construstor for ControlView
	/// </summary>
	public MenuItemsListControl()
	{
		InitializeComponent();
	}

    /// <summary>
    /// BindableProperty for Categories to be used on UI
    /// </summary>
    public static readonly BindableProperty MenuItemsProperty =
        BindableProperty.Create(nameof(MenuItems), typeof(ItemOnMenu[]), typeof(MenuItemsListControl), Array.Empty<ItemOnMenu>());

    /// <summary>
    /// Public property for Categories
    /// </summary>
    public ItemOnMenu[] MenuItems
    {
        get => (ItemOnMenu[])GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
    }

    /// <summary>
    /// Event to be called when Category gets changed
    /// </summary>
    public event Action<ItemOnMenu> OnMenuItemSelected;

    /// <summary>
    /// Default action icon for each item, dynamic so we can use different for different screen
    /// </summary>
    public string ActionIcon { get; set; } = "shopping_bag_regular_24.png";

    /// <summary>
    /// A public property for settings when we use the control
    /// </summary>
    public bool IsEditCase
    {
        set => ActionIcon = (value ? "right_arrow_regular_24.png" : "shopping_bag_regular_24.png");
    }

    /// <summary>
    /// Command to run when category is changed on UI
    /// </summary>
    /// <param name="category">Selected Category</param>
    [RelayCommand]
    private void SelectMenuItem(ItemOnMenu menuItem) =>
        OnMenuItemSelected?.Invoke(menuItem);
}