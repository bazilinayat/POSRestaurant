using Ghostscript.NET;
using POSRestaurant.Models;
using System.ComponentModel;
using System.Reflection;

namespace POSRestaurant.Data
{
    /// <summary>
    /// Class to take care of enum description
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Method to get the enum description for the given Enum value
        /// </summary>
        /// <param name="value">Value of enum</param>
        /// <returns>Returns description of the enum value</returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return value.ToString();

            var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// To get the list of all the descriptions from an enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Returns a list of all the descriptions</returns>
        public static List<ValueForPicker> GetAllDescriptions<T>() where T : Enum
        {
            var list = new List<ValueForPicker>();

            var type = typeof(T);
            return Enum.GetValues(type)
                       .Cast<T>()
                       .Select(value =>
                       {
                           var fieldInfo = type.GetField(value.ToString());
                           var attribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();

                           return new ValueForPicker
                           {
                               Key = Convert.ToInt32(value),
                               Value = attribute?.Description ?? value.ToString()
                           };
                       })
                       .ToList();
        }
    }

    /// <summary>
    /// List of expense item types
    /// </summary>
    public enum ExpenseItemTypes
    {
        /// <summary>
        /// To represent all types, for reports only
        /// </summary>
        [Description("All")]
        All = 0,
        /// <summary>
        /// Vegetable products
        /// </summary>
        [Description("Vegetable")]
        Vegetable = 1,
        /// <summary>
        /// Non veg products
        /// </summary>
        [Description("Fruit")]
        Fruit = 2,
        /// <summary>
        /// Dairy products
        /// </summary>
        [Description("Dairy")]
        Dairy = 3,
        /// <summary>
        /// Bakery products
        /// </summary>
        [Description("Bakery")]
        Bakery = 4
    }

    /// <summary>
    /// List of Payment Modes
    /// </summary>
    public enum PaymentModes
    {
        [Description("All")]
        All = 0,
        /// <summary>
        /// Choose when payment method is cash
        /// </summary>
        [Description("Cash")]
        Cash = 1,
        /// <summary>
        /// Choose when payment method is online
        /// </summary>
        [Description("Online")]
        Online = 2,
        /// <summary>
        /// Choose when payment method is card
        /// </summary>
        [Description("Card")]
        Card = 3,
        /// <summary>
        /// Part payment done
        /// Split in different payment options
        /// </summary>
        [Description("Part")]
        Part = 4
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
        /// Used to indicate order is confirmed
        /// No changes will be made in order from system now
        /// </summary>
        Confirmed = 2,
        /// <summary>
        /// Used to indicate order is printed
        /// </summary>
        Printed = 3,
        /// <summary>
        /// Used to indicate order is paid
        /// </summary>
        Paid = 4
    }

    /// <summary>
    /// List of Order Types
    /// </summary>
    public enum OrderTypes
    {
        All = 0,
        /// <summary>
        /// Choose when order type is dinein
        /// </summary>
        [Description("Dine In")]
        DineIn = 1,
        /// <summary>
        /// Choose when order type is pickup
        /// </summary>
        [Description("Pickup")]
        Pickup = 2,
        /// <summary>
        /// Choose when order type is online
        /// </summary>
        [Description("Online")]
        Online = 3
    }

    /// <summary>
    /// Different staff roles in our organization
    /// </summary>
    public enum StaffRole
    {
        /// <summary>
        /// To represent all roles, for reporting only
        /// </summary>
        All = 0,
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
        Cook = 6,
        /// <summary>
        /// Cook working in org
        /// </summary>
        Cashier = 7,
        /// <summary>
        /// Guys who make the delivery
        /// </summary>
        Delivery = 8
    }

    /// <summary>
    /// Different sources from where you can get order for pickup
    /// </summary>
    public enum PickupSource
    {
        /// <summary>
        /// For orders received from swiggy
        /// </summary>
        Swiggy = 1,
        /// <summary>
        /// For orders received from zomato
        /// </summary>
        Zomato = 2,
        /// <summary>
        /// For orders received directly to restaurant
        /// </summary>
        Direct = 3
    }

    /// <summary>
    /// List of modes which can be used to make payments in expenses
    /// </summary>
    public enum ExpensePaymentModes
    {
        /// <summary>
        /// Choose when payment method is cash
        /// </summary>
        [Description("Cash")]
        Cash = 1,
        /// <summary>
        /// Choose when payment method is online
        /// </summary>
        [Description("Online")]
        Online = 2,
        /// <summary>
        /// Choose when payment method is card
        /// </summary>
        [Description("Bank")]
        Bank = 3
    }
}
