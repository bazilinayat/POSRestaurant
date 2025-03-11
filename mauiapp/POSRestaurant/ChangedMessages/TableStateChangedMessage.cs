using CommunityToolkit.Mvvm.Messaging.Messages;
using POSRestaurant.Models;

namespace POSRestaurant.ChangedMessages
{
    /// <summary>
    /// A pub-sub message class for any changes in Table objects to be notified everywhere throughout the application
    /// </summary>
    public class TableStateChangedMessage : ValueChangedMessage<TableModel>
    {
        /// <summary>
        /// public constructor of the class to init the base as well
        /// </summary>
        /// <param name="value">TableModel the changed value</param>
        public TableStateChangedMessage(TableModel value) : base(value) { }

        /// <summary>
        /// To get TableStateChangedMessage from TableModel class
        /// </summary>
        /// <param name="value">TableModel the changed value</param>
        /// <returns>new TableStateChangedMessage</returns>
        public static TableStateChangedMessage From(TableModel value) => new(value);
    }
}
