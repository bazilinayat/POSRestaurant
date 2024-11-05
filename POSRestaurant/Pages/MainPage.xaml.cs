using POSRestaurant.Data;

namespace POSRestaurant.Pages;

/// <summary>
/// MainPage of the Application
/// </summary>
public partial class MainPage : ContentPage
{
    int count = 0;

    /// <summary>
    /// Initialize MainPage
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}
