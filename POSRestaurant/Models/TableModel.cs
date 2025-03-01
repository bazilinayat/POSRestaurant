using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.Data;

namespace POSRestaurant.Models
{
    public partial class TableModel : ObservableObject
    {
        /// <summary>
        /// Table Id
        /// Primary key, autoincrement
        /// </summary>
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
        public TableOrderStatus Status { get; set; } = TableOrderStatus.NoOrder;
        /// <summary>
        /// To set the colour of the table border as per the TableOrderStatus
        /// </summary>
        public Color BorderColour { get; set; }
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
        public TimeOnly RunningTime { get; set; }
        /// <summary>
        /// Will keep track for how long the table was running
        /// Starting of the table
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Will keep track for how long the table was running
        /// Ending of the table
        /// </summary>
        public DateTime EndTime { get; set; }
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
        public StaffModel Waiter { get; set; }
        /// <summary>
        /// Watier id
        /// </summary>
        public int WaiterId { get; set; }
        /// <summary>
        /// Id of the staff cashier who handled the order 
        /// </summary>
        public StaffModel Cashier { get; set; }
        /// <summary>
        /// Cashier id
        /// </summary>
        public int CashierId { get; set; }
        /// <summary>
        /// Order total, to be displayed on UI
        /// </summary>
        public decimal OrderTotal { get; set; }
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

        /// <summary>
        /// To make object of TableState
        /// </summary>
        /// <param name="entity">TableModel Object</param>
        /// <returns>Returns a TableState object</returns>
        public static TableState FromEntity(TableModel entity) =>
            new()
            {
                Id = entity.Id,
                TableNo = entity.TableNo,
                RunningOrderId = entity.RunningOrderId,
                Status = entity.Status,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
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
