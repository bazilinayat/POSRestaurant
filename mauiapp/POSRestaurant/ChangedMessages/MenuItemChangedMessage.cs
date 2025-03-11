using CommunityToolkit.Mvvm.Messaging.Messages;
using POSRestaurant.Models;

namespace POSRestaurant.ChangedMessages
{
    /// <summary>
    /// A pub-sub message class for any changes in ItemOnMenu objects to be notified everywhere throughout the application
    /// </summary>
    public class MenuItemChangedMessage : ValueChangedMessage<ItemOnMenuChangeModel>
    {
        /// <summary>
        /// public constructor of the class to init the base as well
        /// </summary>
        /// <param name="value">ItemOnMenuChangeModel the changed value</param>
        public MenuItemChangedMessage(ItemOnMenuChangeModel value) : base(value) { }

        /// <summary>
        /// To get MenuItemChangedMessage from ItemOnMenuChangeModel class
        /// </summary>
        /// <param name="value">ItemOnMenuChangeModel the changed value</param>
        /// <returns>new MenuItemChangedMessage</returns>
        public static MenuItemChangedMessage From(ItemOnMenuChangeModel value) => new(value);
    }
}
