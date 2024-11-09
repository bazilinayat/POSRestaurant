﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Class to keep the values taken from Settings.json file
    /// Initialized in SettingsViewModel
    /// </summary>
    public class SettingsModel
    {
        /// <summary>
        /// Name of the Customer
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// To store dfault tax percentage
        /// </summary>
        public decimal DefaultTaxPercentage { get; set; }
        /// <summary>
        /// phone number of the developer
        /// </summary>
        public string Phone {  get; set; }
        /// <summary>
        /// Email of the developer
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Website to redirect to, when developer name is clicked
        /// </summary>
        public string WebsiteURL { get; set; }
    }
}