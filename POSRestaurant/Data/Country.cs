using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Class to manage the country details in our system
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Primary key for the country to represent in our system
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Name of the country
        /// </summary>
        public string Name { get; set; }
    }
}
