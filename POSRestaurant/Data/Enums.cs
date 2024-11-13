namespace POSRestaurant.Data
{
    /// <summary>
    /// List of Payment Modes
    /// </summary>
    public enum PaymentModes
    {
        /// <summary>
        /// Choose when payment method is cash
        /// </summary>
        Cash = 1,
        /// <summary>
        /// Choose when payment method is online
        /// </summary>
        Online = 2,
        /// <summary>
        /// Choose when payment method is card
        /// </summary>
        Card = 3
    }

    /// <summary>
    /// List of Table Order Status
    /// </summary>
    public enum TableOrderStatus
    {
        /// <summary>
        /// Used to indicate there is no order for table now
        /// </summary>
        NoOrder = 0,
        /// <summary>
        /// Used to indicate order is running
        /// </summary>
        Running = 1,
        /// <summary>
        /// Used to indicate order is printed
        /// No changes will be made in system now
        /// </summary>
        Printed = 2,
        /// <summary>
        /// Used to indicate order is paid
        /// </summary>
        Paid = 3
    }
}
