using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Data
{
    /// <summary>
    /// To represent the tax details in India table in DB
    /// </summary>
    public class TaxDetailIndia
    {
        /// <summary>
        /// Id of the tax detail
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Id of the country where this tax law applies
        /// We are keeping in India only... for now
        /// </summary>
        public int CountryId { get; set; }
        /// <summary>
        /// In India we have CGST on items
        /// Value in percentage
        /// </summary>
        public decimal CGST { get; set; }
        /// <summary>
        /// In India we have SGST on items
        /// Value in percentage
        /// </summary>
        public decimal SGST { get; set; }
        /// <summary>
        /// In India we have VAT on alchoholic drinks
        /// Value in percentage
        /// </summary>
        public decimal VAT { get; set; }

    }
}
