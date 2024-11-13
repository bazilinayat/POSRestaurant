using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;
using SQLite;

namespace POSRestaurant.Models
{
    public partial class TableModel : ObservableObject
    {
        /// <summary>
        /// Table Id
        /// Primary key, autoincrement
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        /// <summary>
        /// Table number, Id could be different, we can't say
        /// </summary>
        public int TableNo { get; set; }
        /// <summary>
        /// RunningOrderId associated with the table
        /// </summary>
        public long RunningOrderId { get; set; }
        /// <summary>
        /// To represent status of the table
        /// </summary>
        public TableOrderStatus Status { get; set; }
        /// <summary>
        /// Will keep track for how long the table was running
        /// </summary>
        public TimeOnly RunningTime { get; set; }
        /// <summary>
        /// To track the selected order
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;
        /// <summary>
        /// To make object of TableModel
        /// </summary>
        /// <param name="entity">Table Object</param>
        /// <returns>Returns a TableModel object</returns>
        public static TableModel FromEntity(Table entity) =>
            new()
            {
                Id = entity.Id,
                TableNo = entity.TableNo,
            };
    }
}
