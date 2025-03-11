namespace POSRestaurant.Controls;

/// <summary>
/// Class to display current date and time
/// </summary>
public partial class CurrentDateTimeControl : ContentView, IDisposable
{
	/// <summary>
	/// To tick the date and time values
	/// </summary>
	private readonly PeriodicTimer _timer;

	/// <summary>
	/// Constructor, we will set the date and time
	/// </summary>
	public CurrentDateTimeControl()
	{
		InitializeComponent();

		dayTimeLabel.Text = DateTime.Now.ToString("dddd, hh:mm:ss tt");
		dateLabel.Text = DateTime.Now.ToString("MMM dd, yyyy");

		_timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
		UpdateTimer();
	}

	/// <summary>
	/// Imlementing IDisposable interface
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
    public void Dispose() => _timer?.Dispose();

    /// <summary>
    /// Method to start ticking and set the values
    /// </summary>
    private async void UpdateTimer()
	{
		while(await _timer.WaitForNextTickAsync())
		{
            dayTimeLabel.Text = DateTime.Now.ToString("dddd, hh:mm:ss tt");
            dateLabel.Text = DateTime.Now.ToString("MMM dd, yyyy");
        }
	}
}