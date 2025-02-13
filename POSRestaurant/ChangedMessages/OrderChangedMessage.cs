using CommunityToolkit.Mvvm.Messaging.Messages;
using POSRestaurant.Models;

namespace POSRestaurant.ChangedMessages
{
    /// <summary>
    /// A pub-sub message class for any changes in Order objects to be notified everywhere throughout the application
    /// </summary>
    public class OrderChangedMessage : ValueChangedMessage<bool>
    {
        /// <summary>
        /// public constructor of the class to init the base as well
        /// </summary>
        /// <param name="value">bool just to pass something, to change the value</param>
        public OrderChangedMessage(bool value) : base(value) { }

        /// <summary>
        /// To get OrderChangedMessage
        /// </summary>
        /// <param name="value">Bool just to pass something, to change the value</param>
        /// <returns>new OrderChangedMessage</returns>
        public static OrderChangedMessage From(bool value) => new(value);
    }
}
