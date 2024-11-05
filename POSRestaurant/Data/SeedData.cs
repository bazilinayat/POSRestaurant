using System.Text.Json;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Class for initial data for application
    /// Menu data should come from menu.json file
    /// </summary>
    public static class SeedData
    {
        public static List<MenuCategory> GetMenuCategories()
        {
            try
            {
                using (StreamReader reader = new StreamReader("./Data/MenuCategories.json"))
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

        public static List<ItemOnMenu> GetMenuItems()
        {
            try
            {
                using (StreamReader reader = new StreamReader("./Data/MenuItems.json"))
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
    }
}
