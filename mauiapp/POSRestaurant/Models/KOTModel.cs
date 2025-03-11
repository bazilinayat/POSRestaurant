using CommunityToolkit.Mvvm.ComponentModel;
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
    /// KOTModel for the UI
    /// </summary>
    public partial class KOTModel : ObservableObject
    {
        /// <summary>
        /// Order Id
        /// Primary key, auto incremented
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// To keep track of the number of KOTs in the order
        /// </summary>
        public int KOTNumber { get; set; }
        /// <summary>
        /// Order Id, KOT belongs to
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// DateTime of the Order
        /// </summary>
        public DateTime KOTDateTime { get; set; }
        /// <summary>
        /// Total number of items of the order
        /// </summary>
        public int TotalItemCount { get; set; }
        /// <summary>
        /// Total amount of the order
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// Items in the current KOT
        /// </summary>
        public KOTItem[] Items { get; set; }

        /// <summary>
        /// To make object of KOTModel
        /// </summary>
        /// <param name="entity">KOT Object</param>
        /// <returns>Returns a KOTModel object</returns>
        public static KOTModel FromEntity(KOT entity) =>
            new()
            {
                Id = entity.Id,
                KOTNumber = entity.KOTNumber,
                OrderId = entity.OrderId,
                KOTDateTime = entity.KOTDateTime,
                TotalItemCount = entity.TotalItemCount,
                TotalPrice = entity.TotalPrice,
                Items = entity.Items
            };
    }
}
