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
