using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Models;

namespace POSRestaurant.Controls;

/// <summary>
/// Control for editing the items in our menu
/// </summary>
public partial class SaveMenuItemFormControl : ContentView
{
	/// <summary>
	/// Constructor for the editing control
	/// </summary>
	public SaveMenuItemFormControl()
	{
		InitializeComponent();
	}

    ///// <summary>
    ///// BindableProperty for Categories to be used on UI
    ///// </summary>
    //public static readonly BindableProperty CategoriesProperty =
    //    BindableProperty.Create(nameof(Categories), typeof(MenuCategoryModel[]), typeof(CategoriesListControl), Array.Empty<MenuCategoryModel>());
    
    ///// <summary>
    ///// Public property for Categories
    ///// </summary>
    //public MenuCategoryModel[] Categories
    //{
    //    get => (MenuCategoryModel[])GetValue(CategoriesProperty);
    //    set => SetValue(CategoriesProperty, value);
    //}

    /// <summary>
    /// BindableProperty for Item to be used on UI
    /// </summary>
    public static readonly BindableProperty ItemProperty =
        BindableProperty.Create(nameof(Item), typeof(ItemOnMenuModel), typeof(SaveMenuItemFormControl), new ItemOnMenuModel());

    /// <summary>
    /// For the specific item this control is responsible for
    /// </summary>
    public ItemOnMenuModel Item
    {
        get => (ItemOnMenuModel)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    /// <summary>
    /// BindableProperty for Item to be used on UI
    /// </summary>
    public static readonly BindableProperty CanBeDeletedProperty =
        BindableProperty.Create(nameof(CanBeDeleted), typeof(int), typeof(SaveMenuItemFormControl), -1);

    /// <summary>
    /// A public property to pass when we use the control
    /// </summary>
    public int CanBeDeleted
    {
        get
        {
            var val = (int)GetValue(CanBeDeletedProperty);
            return val > 0 ? 1 : 0;
        }
        set
        {
            SetValue(CanBeDeletedProperty, value);
        }
    }

    /// <summary>
    /// Event to work with cancel button click on the control
    /// </summary>
    public event Action? OnCancel;

    /// <summary>
    /// Command to handle the cancel button click
    /// </summary>
    [RelayCommand]
    private void Cancel() => OnCancel?.Invoke();

    /// <summary>
    /// Event to work with save button click on the control
    /// </summary>
    public event Action<ItemOnMenuModel> OnSaveItem;

    /// <summary>
    /// Command called when Save button is clicked
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task SaveMenuItem()
    {
        // Validation

        if (Item.Category == null)
        {
            await ErrorAlertAsync("Select a category for changes");
            return;
        }
        
        if (string.IsNullOrEmpty(Item.Name) || string.IsNullOrWhiteSpace(Item.Description))
        {
            await ErrorAlertAsync("Item name and description are mandatory");
            return;
        }

        if (Item.Price <= 0)
        {
            await ErrorAlertAsync("Item price should be greater than 0");
            return;
        }

        OnSaveItem?.Invoke(Item);

        static async Task ErrorAlertAsync(string message) =>
            await Shell.Current.DisplayAlert("Validation Error", message, "OK");
    }

    /// <summary>
    /// Event to work with delete button click on the control
    /// </summary>
    public event Action<ItemOnMenuModel> OnDeleteItem;

    /// <summary>
    /// Command called when Save button is clicked
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task DeleteMenuItem()
    {
        if (await Shell.Current.DisplayAlert("Delete Item?", $"Do you really want to delete {Item.Name}?", "Yes", "No"))
            OnDeleteItem?.Invoke(Item);
    }
}