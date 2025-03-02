using POSRestaurant.Data;
using POSRestaurant.Models;
using SQLite;

namespace POSRestaurant.DBO
{
    /// <summary>
    /// To handle all the user, role, permission related operations
    /// </summary>
    public class UserOperations
    {
        /// <summary>
        /// Readonly connection object to our SQLite db
        /// </summary>
        private readonly SQLiteAsyncConnection _connection;

        /// <summary>
        /// Contructor to assign the connection
        /// </summary>
        /// <param name="connection">Connection object to be used henceforth</param>
        public UserOperations(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Method to get all the permissions from database
        /// </summary>
        /// <returns>Array of Table</returns>
        public async Task<List<Permission>> GetAllPermissionsAsync() =>
            await _connection.Table<Permission>().ToListAsync();

        /// <summary>
        /// Method to get all the roles from database
        /// </summary>
        /// <returns>Array of Table</returns>
        public async Task<UserRole[]> GetAllRolesAsync() =>
            await _connection.Table<UserRole>().ToArrayAsync();

        /// <summary>
        /// Method to get a role with its permissions
        /// </summary>
        /// <param name="roleId">The role ID to retrieve</param>
        /// <returns>UserRoleEditModel with permissions</returns>
        public async Task<UserRoleEditModel> GetRoleByIdAsync(int roleId)
        {
            // Get the role
            var role = await _connection.Table<UserRole>().Where(r => r.Id == roleId).FirstOrDefaultAsync();

            if (role == null)
                return null;

            // Create the edit model
            var roleModel = new UserRoleEditModel
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = new List<PermissionModel>()
            };

            // Get all available permissions
            var allPermissions = await _connection.Table<Permission>().ToListAsync();

            // Get the permissions assigned to this role
            var rolePermissions = await _connection.Table<RolePermission>()
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            // Create permission models with selected state
            foreach (var permission in allPermissions)
            {
                var permissionModel = PermissionModel.FromEntity(permission);
                permissionModel.IsSelected = rolePermissions.Any(rp => rp.PermissionId == permission.Id);
                roleModel.Permissions.Add(permissionModel);
            }

            return roleModel;
        }

        /// <summary>
        /// Method to save the role and its permissions to database
        /// </summary>
        /// <param name="roleModel">The role with permissions to save</param>
        /// <returns>The saved UserRoleEditModel or error message</returns>
        public async Task<UserRoleEditModel> SaveRoleAsync(UserRoleEditModel roleModel)
        {
            // Begin a transaction
            await _connection.RunInTransactionAsync(async (transaction) =>
            {
                UserRole role = new UserRole
                {
                    Id = roleModel.Id,
                    Name = roleModel.Name
                };

                // Insert or update the role
                if (role.Id == 0)
                {
                    // New role
                    await _connection.InsertAsync(role);
                    // Get the new ID
                    roleModel.Id = role.Id;
                }
                else
                {
                    // Update existing role
                    await _connection.UpdateAsync(role);
                }

                // Delete existing role permissions
                await _connection.ExecuteAsync("DELETE FROM RolePermission WHERE RoleId = ?", roleModel.Id);

                // Insert new role permissions for selected permissions
                foreach (var permission in roleModel.Permissions.Where(p => p.IsSelected))
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = roleModel.Id,
                        PermissionId = permission.Id
                    };
                    await _connection.InsertAsync(rolePermission);
                }
            });

            // Return the saved role with permissions
            return await GetRoleByIdAsync(roleModel.Id);
        }

        /// <summary>
        /// Delete a role and its permissions from the database
        /// </summary>
        /// <param name="roleId">ID of the role to delete</param>
        /// <returns>Returns true if successfully deleted</returns>
        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            try
            {
                await _connection.RunInTransactionAsync(async (transaction) =>
                {
                    // Delete the role-permission mappings first
                    await _connection.ExecuteAsync("DELETE FROM RolePermission WHERE RoleId = ?", roleId);

                    // Delete the role
                    await _connection.ExecuteAsync("DELETE FROM UserRole WHERE Id = ?", roleId);
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method to get all the users from database
        /// </summary>
        /// <returns>Array of User</returns>
        public async Task<User[]> GetAllUsersAsync() =>
            await _connection.Table<User>().ToArrayAsync();

        /// <summary>
        /// Method to get a user with its roles
        /// </summary>
        /// <param name="userId">The user ID to retrieve</param>
        /// <returns>UserModel with Roles</returns>
        public async Task<UserModel> GetUserByIdAsync(int userId)
        {
            // Get the role
            var user = await _connection.Table<User>().Where(r => r.Id == userId).FirstOrDefaultAsync();

            if (user == null)
                return null;

            // Create the edit model
            var userModel = new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password
            };

            // Get assigned role
            var assignedRole = await GetUserRoleAsync(user.Id);
            if (assignedRole != null)
            {
                userModel.AssignedRoleId = assignedRole.RoleId;
            }

            return userModel;
        }

        public async Task<AssignedUserRole> GetUserRoleAsync(int userId) =>
            await _connection.Table<AssignedUserRole>().Where(o => o.UserId == userId).FirstOrDefaultAsync();

        /// <summary>
        /// Method to save the user and its roles to database
        /// </summary>
        /// <param name="userModel">The user with roles to save</param>
        /// <returns>The saved UserModel or error message</returns>
        public async Task<UserModel> SaveUserAsync(UserEditModel userModel)
        {
            // Begin a transaction
            await _connection.RunInTransactionAsync(async (transaction) =>
            {
                User user = new User
                {
                    Id = userModel.Id,
                    Username = userModel.Username,
                    Password = userModel.Password
                };

                // Insert or update the role
                if (user.Id == 0)
                {
                    // New role
                    await _connection.InsertAsync(user);
                    // Get the new ID
                    userModel.Id = user.Id;
                }
                else
                {
                    // Update existing role
                    await _connection.UpdateAsync(user);
                }

                // Delete existing assigned roles
                await _connection.ExecuteAsync("DELETE FROM AssignedUserRole WHERE UserId = ?", userModel.Id);

                // Then add new assignment if a role is selected
                if (userModel.AssignedRoleId.HasValue)
                {
                    var assignment = new AssignedUserRole
                    {
                        UserId = userModel.Id,
                        RoleId = userModel.AssignedRoleId.Value
                    };

                    await _connection.InsertAsync(assignment);
                }
            });

            // Return the saved user with roles
            return await GetUserByIdAsync(userModel.Id);
        }

        /// <summary>
        /// Delete a user and its roles from the database
        /// </summary>
        /// <param name="userId">ID of the user to delete</param>
        /// <returns>Returns true if successfully deleted</returns>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                await _connection.RunInTransactionAsync(async (transaction) =>
                {
                    // Delete the assgined roles mappings first
                    await _connection.ExecuteAsync("DELETE FROM AssignedUserRole WHERE UserId = ?", userId);

                    // Delete the role
                    await _connection.ExecuteAsync("DELETE FROM User WHERE Id = ?", userId);
                });

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
