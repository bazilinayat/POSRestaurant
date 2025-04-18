﻿using POSRestaurant.Models;
using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Inventory items of the organization
    /// This is a record of items we are buying and how much we are paying for them
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Primary id of the entry done
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// Type of the item added
        /// </summary>
        public long ExpenseTypeId { get; set; }

        /// <summary>
        /// Type of the item added
        /// </summary>
        public string ExpenseTypeName { get; set; }

        /// <summary>
        /// Expense Item to be added
        /// </summary>
        public long ExpenseItemId { get; set; }

        /// <summary>
        /// Expense Item Name to be added
        /// </summary>
        public string ExpenseItemName { get; set; }

        /// <summary>
        /// If the item is weighted or counted
        /// </summary>
        public bool IsWeighted { get; set; }

        /// <summary>
        /// Quantity of the item, if not weighted
        /// Weight of the item, if weighted
        /// </summary>
        public double QuantityOrWeight { get; set; }

        /// <summary>
        /// Totall cost of the item paid
        /// </summary>
        public double TotalPrice { get; set; }

        /// <summary>
        /// Staff member id who paid for the item
        /// </summary>
        public int StaffId { get; set; }

        /// <summary>
        /// Staff member name who paid for the item
        /// </summary>
        public string StaffName { get; set; }

        /// <summary>
        /// To know the payment mode used
        /// </summary>
        public ExpensePaymentModes PaymentMode { get; set; }

        /// <summary>
        /// To know the payment mode used for name
        /// </summary>
        public string PaymentModeName { get; set; }

        /// <summary>
        /// Date of record entry
        /// </summary>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// To make object of Inventory
        /// </summary>
        /// <param name="entity">InventoryReportModel Object</param>
        /// <returns>Returns a Inventory object</returns>
        public static Inventory FromEntity(InventoryReportModel entity) =>
            new()
            {
                Id = entity.Id,
                ExpenseTypeId = entity.ExpenseTypeId,
                ExpenseTypeName = entity.ExpenseTypeName,
                EntryDate = entity.EntryDate,
                ExpenseItemId = entity.ExpenseItemId,
                IsWeighted = entity.IsWeighted,
                QuantityOrWeight = entity.QuantityOrWeight,
                TotalPrice = entity.TotalPrice,
                StaffId = entity.StaffId,
                ExpenseItemName = entity.ExpenseItemName,
                StaffName = entity.StaffName,
                PaymentMode = entity.PaymentMode,
                PaymentModeName = entity.PaymentModeName
            };
    }
}
