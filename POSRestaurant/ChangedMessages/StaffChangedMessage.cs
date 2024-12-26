using CommunityToolkit.Mvvm.Messaging.Messages;
using POSRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.ChangedMessages
{
    /// <summary>
    /// A pub-sub message class for any changes in Staff objects to be notified everywhere throughout the application
    /// </summary>
    public class StaffChangedMessage : ValueChangedMessage<StaffModel>
    {
        /// <summary>
        /// public constructor of the class to init the base as well
        /// </summary>
        /// <param name="value">StaffModel the changed value</param>
        public StaffChangedMessage(StaffModel value) : base(value) { }

        /// <summary>
        /// To get StaffChangedMessage from StaffModel class
        /// </summary>
        /// <param name="value">StaffModel the changed value</param>
        /// <returns>new StaffChangedMessage</returns>
        public static StaffChangedMessage From(StaffModel value) => new(value);
    }
}
