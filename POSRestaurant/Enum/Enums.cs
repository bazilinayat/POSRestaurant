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

    /// <summary>
    /// List of Order Types
    /// </summary>
    public enum OrderTypes
    {
        /// <summary>
        /// Choose when order type is dinein
        /// </summary>
        DineIn = 1,
        /// <summary>
        /// Choose when order type is pickup
        /// </summary>
        Pickup = 2,
        /// <summary>
        /// Choose when order type is online
        /// </summary>
        Online = 3
    }

    /// <summary>
    /// Different staff roles in our organization
    /// </summary>
    public enum StaffRole
    {
        /// <summary>
        /// Owner of the organization
        /// </summary>
        Owner = 1,
        /// <summary>
        /// Co-owner of the organization
        /// </summary>
        CoOwner = 2,
        /// <summary>
        /// Manager of the org
        /// </summary>
        Manager = 3,
        /// <summary>
        /// Waiter working in org
        /// </summary>
        Waiter = 4,
        /// <summary>
        /// Cleaner working in org
        /// </summary>
        Cleaner = 5,
        /// <summary>
        /// Cook working in org
        /// </summary>
        Cook = 6
    }
}
