using POSRestaurant.Models;
using SQLite;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Table to store the table state of the application
    /// This will be used if in case application is closed abruptly
    /// </summary>
    public class TableState
    {
        /// <summary>
        /// Table id 
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }
        /// <summary>
        /// Table number
        /// </summary>
        public int TableNo { get; set; }
        /// <summary>
        /// RunningOrderId associated with the table
        /// </summary>
        public long RunningOrderId { get; set; }
        /// <summary>
        /// To represent status of the table
        /// </summary>
        public TableOrderStatus Status { get; set; } = TableOrderStatus.NoOrder;
        /// <summary>
        /// As per the Status, the action to be taken will change
        /// Should display different images for each action
        /// </summary>
        public string ActionButtonImageIcon { get; set; } = "";
        /// <summary>
        /// The action button will enabled in certain conditions only
        /// </summary>
        public bool ActionButtonEnabled { get; set; }
        /// <summary>
        /// Tool tip to know what the action button is meant for
        /// </summary>
        public string ActionButtonToolTipText { get; set; }
        /// <summary>
        /// Will keep track for how long the table was running
        /// </summary>
        public DateTime RunningTime { get; set; }
        /// <summary>
        /// To represent the type of order this is
        /// </summary>
        public OrderTypes OrderType { get; set; }
        /// <summary>
        /// Number of people sitting on the table
        /// </summary>
        public int NumberOfPeople { get; set; }
        /// <summary>
        /// Id of the staff waiter who handled the order and table
        /// </summary>
        public int WaiterId { get; set; }
        /// <summary>
        /// Order total, to be displayed on UI
        /// </summary>
        public decimal OrderTotal { get; set; }
        /// <summary>
        /// To make object of TableModel
        /// </summary>
        /// <param name="entity">TableState Object</param>
        /// <returns>Returns a TableModel object</returns>
        public static TableModel FromEntity(TableState entity) =>
            new()
            {
                Id = entity.Id,
                TableNo = entity.TableNo,
                RunningOrderId = entity.RunningOrderId,
                Status = entity.Status,
                ActionButtonImageIcon = entity.ActionButtonImageIcon,
                ActionButtonEnabled = entity.ActionButtonEnabled,
                ActionButtonToolTipText = entity.ActionButtonToolTipText,
                OrderType = entity.OrderType,
                NumberOfPeople = entity.NumberOfPeople,
                WaiterId = entity.WaiterId,
                OrderTotal = entity.OrderTotal
            };
    }
}
