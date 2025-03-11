using POSRestaurant.Data;

namespace POSRestaurant.Controls;

public partial class ItemReportControl : ContentView
{
	public ItemReportControl()
	{
		InitializeComponent();
	}

    /// <summary>
    /// BindableProperty for Categories to be used on UI
    /// </summary>
    public static readonly BindableProperty KOTsProperty =
        BindableProperty.Create(nameof(KOTs), typeof(KOTItem[]), typeof(ItemReportControl), Array.Empty<KOTItem>());

    /// <summary>
    /// Public property for Categories
    /// </summary>
    public KOTItem[] KOTs
    {
        get => (KOTItem[])GetValue(KOTsProperty);
        set => SetValue(KOTsProperty, value);
    }

    /// <summary>
    /// BindableProperty for Categories to be used on UI
    /// </summary>
    public static readonly BindableProperty CategoryProperty =
        BindableProperty.Create(nameof(Category), typeof(string), typeof(ItemReportControl), "");

    /// <summary>
    /// Public property for Categories
    /// </summary>
    public string Category
    {
        get => (string)GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }
}