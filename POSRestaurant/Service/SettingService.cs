using POSRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace POSRestaurant.Utility
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
        public SettingService()
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
                Settings = null;
            }
        }
    }
}
