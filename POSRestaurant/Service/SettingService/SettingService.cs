using POSRestaurant.Service.LoggerService;
using System.Text.Json;

namespace POSRestaurant.Service.SettingService
{
    /// <summary>
    /// To Keep track of the settings, taken from some config
    /// </summary>
    public class SettingService
    {
        /// <summary>
        /// To see if settings is initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public readonly SettingsModel Settings;

        /// <summary>
        /// Constructor to set settings object
        /// </summary>
        public SettingService(LogService logger)
        {
            if (_isInitialized) return;

            _isInitialized = true;

            try
            {
                using (StreamReader reader = new StreamReader("Settings.json"))
                {
                    string jsontext = reader.ReadToEnd();

                    Settings = JsonSerializer.Deserialize<SettingsModel>(jsontext);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("SettingService-Error in Loading Settings ", ex);
                Settings = null;
            }
        }
    }
}
