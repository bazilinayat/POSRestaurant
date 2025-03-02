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
    /// A pub-sub message class for any changes in user role objects to be notified everywhere throughout the application
    /// </summary>
    public class UserRoleChangedMessage : ValueChangedMessage<UserRoleModel>
    {
        /// <summary>
        /// public constructor of the class to init the base as well
        /// </summary>
        /// <param name="value">UserRoleModel the changed value</param>
        public UserRoleChangedMessage(UserRoleModel value) : base(value) { }

        /// <summary>
        /// To get RoleChangedMessage from UserRoleModel class
        /// </summary>
        /// <param name="value">UserRoleModel the changed value</param>
        /// <returns>new RoleChangedMessage</returns>
        public static UserRoleChangedMessage From(UserRoleModel value) => new(value);
    }
}
