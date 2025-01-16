using POSRestaurant.Data;
using SettingLibrary;
using System.Text.Json;

namespace POSRestaurant.DBO
{
    /// <summary>
    /// Class for initial data for application
    /// Menu data should come from menu.json file
    /// </summary>
    public class SeedData
    {
        /// <summary>
        /// DIed SettingService
        /// </summary>
        private readonly SettingService _settingService;

        /// <summary>
        /// Constructor for the SeedingData
        /// </summary>
        /// <param name="settingService">DI for SettingService</param>
        public SeedData(SettingService settingService)
        {
            _settingService = settingService;
        }

        /// <summary>
        /// Read MenuCategories from json file for initial data
        /// </summary>
        /// <returns>Returns a List of MenuCategory</returns>
        public List<MenuCategory> GetMenuCategories()
        {
            try
            {
                using (StreamReader reader = new StreamReader("./MenuCategories.json"))
                {
                    string jsontext = reader.ReadToEnd();

                    return JsonSerializer.Deserialize<List<MenuCategory>>(jsontext);
                }                
            }
            catch (Exception ex)
            {
                return new List<MenuCategory>();
            }
        }

        /// <summary>
        /// Read MenuCategoryItems from json file for initial data
        /// </summary>
        /// <returns>Returns a List of ItemOnMenu</returns>
        public List<ItemOnMenu> GetMenuItems()
        {
            try
            {
                using (StreamReader reader = new StreamReader("./MenuItems.json"))
                {
                    string jsontext = reader.ReadToEnd();

                    return JsonSerializer.Deserialize<List<ItemOnMenu>>(jsontext);
                }
            }
            catch (Exception ex)
            {
                return new List<ItemOnMenu>();
            }
        }

        /// <summary>
        /// Setting the number of table in the database
        /// </summary>
        /// <returns></returns>
        public List<Table> GetTables()
        {
            List<Table> tables = new List<Table>();
            for (int i = 1; i <= _settingService.Settings.NumberOfTables; i++)
            {
                tables.Add(new Table
                {
                    TableNo = i
                });
            }

            return tables;
        }
    }
}
