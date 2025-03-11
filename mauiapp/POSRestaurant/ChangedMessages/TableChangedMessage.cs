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
    /// A pub-sub message class for any changes in Table objects to be notified everywhere throughout the application
    /// </summary>
    public class TableChangedMessage : ValueChangedMessage<TableModel>
    {
        /// <summary>
        /// public constructor of the class to init the base as well
        /// </summary>
        /// <param name="value">TableModel the changed value</param>
        public TableChangedMessage(TableModel value) : base(value) { }

        /// <summary>
        /// To get TableChangedMessage from TableModel class
        /// </summary>
        /// <param name="value">TableModel the changed value</param>
        /// <returns>new TableChangedMessage</returns>
        public static TableChangedMessage From(TableModel value) => new(value);
    }
}
