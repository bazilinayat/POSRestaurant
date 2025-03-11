using CommunityToolkit.Mvvm.Messaging.Messages;

namespace POSRestaurant.ChangedMessages
{
    /// <summary>
    /// A pub-sub message class for any changes in tax details to be notified everywhere throughout the application
    /// </summary>
    public class TaxChangedMessage : ValueChangedMessage<bool>
    {
        /// <summary>
        /// public constructor of the class to init the base as well
        /// </summary>
        /// <param name="value">bool just to pass something, to change the value</param>
        public TaxChangedMessage(bool value) : base(value) { }

        /// <summary>
        /// To get TaxChangedMessage
        /// </summary>
        /// <param name="value">Bool just to pass something, to change the value</param>
        /// <returns>new TaxChangedMessage</returns>
        public static TaxChangedMessage From(bool value) => new(value);
    }
}
