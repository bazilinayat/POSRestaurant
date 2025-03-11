using POSRestaurant.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Class to use in item report
    /// </summary>
    public class ItemReportModel
    {
        /// <summary>
        /// Name of the category, item belongs to
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// KOT items under this category
        /// </summary>
        public List<KOTItem> KOTItems { get; set; }
    }
}
