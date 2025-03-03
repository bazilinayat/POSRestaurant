using SQLite;
using System.Reflection;

namespace POSRestaurant.Service
{
    /// <summary>
    /// A migration manager for merging the code logic with the working database
    /// Will not be able to delete tables or columns...
    /// Note tested for alter yet
    /// </summary>
    public class MigrationManager
    {
        /// <summary>
        /// The connection object used to fire queries
        /// </summary>
        private readonly SQLiteAsyncConnection _database;

        /// <summary>
        /// The database path where it is kept
        /// </summary>
        private readonly string _databasePath;

        /// <summary>
        /// The different entity types, a list of tables
        /// </summary>
        private readonly List<Type> _entityTypes;

        /// <summary>
        /// Constructor for the migration manager
        /// </summary>
        /// <param name="databasePath">The database path provided</param>
        /// <param name="entityTypes">The list of tables to create</param>
        public MigrationManager(string databasePath, List<Type> entityTypes)
        {
            _databasePath = databasePath;
            _database = new SQLiteAsyncConnection(databasePath);
            _entityTypes = entityTypes;
        }

        /// <summary>
        /// To actually start the process of migrating database
        /// </summary>
        /// <returns>Returns a Task</returns>
        public async Task MigrateAsync()
        {
            // Run migrations for each entity type
            foreach (var entityType in _entityTypes)
            {
                await MigrateTableAsync(entityType);
            }
        }

        /// <summary>
        /// To migrate each database
        /// </summary>
        /// <param name="entityType">Table to create</param>
        /// <returns>Returns a Task</returns>
        private async Task MigrateTableAsync(Type entityType)
        {
            string tableName = GetTableName(entityType);

            // Check if the table exists
            bool tableExists = await TableExistsAsync(tableName);

            if (!tableExists)
            {
                // If the table doesn't exist, just create it
                await _database.CreateTableAsync(entityType);
                Console.WriteLine($"Created new table: {tableName}");
                return;
            }

            // Get existing columns in the database
            var existingColumns = await GetExistingColumnsAsync(tableName);

            // Get columns defined in the entity class
            var entityColumns = GetEntityColumns(entityType);

            // Find columns that exist in the entity but not in the database
            var columnsToAdd = entityColumns
                .Where(ec => !existingColumns.Any(dc =>
                    string.Equals(dc.Name, ec.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (columnsToAdd.Count > 0)
            {
                // Add new columns
                await AddColumnsAsync(tableName, columnsToAdd);
            }
            // Note: SQLite doesn't support dropping columns easily
            // If you need to remove columns, you would need to:
            // 1. Create a new table with the desired schema
            // 2. Copy data from the old table
            // 3. Drop the old table
            // 4. Rename the new table to the old name
        }

        /// <summary>
        /// To check if the table exists
        /// </summary>
        /// <param name="tableName">Table name to check</param>
        /// <returns>Returs a bool</returns>
        private async Task<bool> TableExistsAsync(string tableName)
        {
            // Use parameters instead of string interpolation for better SQL injection protection
            var query = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
            var result = await _database.QueryScalarsAsync<string>(query, tableName);
            return result.Any();
        }

        /// <summary>
        /// Get the list of existing columns
        /// </summary>
        /// <param name="tableName">Table name to check</param>
        /// <returns>Returns a list of column info</returns>
        private async Task<List<ColumnInfo>> GetExistingColumnsAsync(string tableName)
        {
            var columns = new List<ColumnInfo>();

            // Properly escape the table name with square brackets to handle special characters
            // and avoid SQL syntax errors
            var query = $"PRAGMA table_info([{tableName}])";
            var tableInfo = await _database.QueryAsync<TableInfo>(query);

            foreach (var column in tableInfo)
            {
                columns.Add(new ColumnInfo
                {
                    Name = column.Name,
                    Type = column.Type,
                    NotNull = column.NotNull > 0,
                    IsPrimaryKey = column.Pk > 0
                });
            }

            return columns;
        }

        /// <summary>
        /// To get the current entity columns
        /// </summary>
        /// <param name="entityType">Entity type to check</param>
        /// <returns>Returns a list of column info for the given entity</returns>
        private List<ColumnInfo> GetEntityColumns(Type entityType)
        {
            var columns = new List<ColumnInfo>();

            // Get properties with Column attribute or public properties
            var properties = entityType.GetProperties()
                .Where(p => p.GetCustomAttribute<ColumnAttribute>() != null ||
                           (p.CanRead && p.CanWrite && p.GetMethod.IsPublic && p.SetMethod.IsPublic))
                .ToList();

            foreach (var property in properties)
            {
                // Skip ignored properties
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
                var primaryKeyAttr = property.GetCustomAttribute<PrimaryKeyAttribute>();
                var maxLengthAttr = property.GetCustomAttribute<MaxLengthAttribute>();

                string columnName = columnAttr?.Name ?? property.Name;
                bool isPrimaryKey = primaryKeyAttr != null;
                bool isNotNull = property.PropertyType.IsValueType &&
                                 Nullable.GetUnderlyingType(property.PropertyType) == null;

                // Determine SQLite type
                string sqliteType = GetSqliteType(property.PropertyType, maxLengthAttr);

                columns.Add(new ColumnInfo
                {
                    Name = columnName,
                    Type = sqliteType,
                    NotNull = isNotNull,
                    IsPrimaryKey = isPrimaryKey,
                    Property = property
                });
            }

            return columns;
        }

        /// <summary>
        /// To get the sqlite equivalent type from the property of the entity
        /// </summary>
        /// <param name="propertyType">Property type from entity</param>
        /// <param name="maxLengthAttr">If there is any max length attribute, information for that</param>
        /// <returns>Returns the data type string</returns>
        private string GetSqliteType(Type propertyType, MaxLengthAttribute maxLengthAttr)
        {
            // Nullable types need special handling
            if (Nullable.GetUnderlyingType(propertyType) != null)
                propertyType = Nullable.GetUnderlyingType(propertyType);

            if (propertyType == typeof(int) || propertyType == typeof(long) ||
                propertyType == typeof(bool) || propertyType.IsEnum)
                return "INTEGER";
            else if (propertyType == typeof(float) || propertyType == typeof(double) ||
                     propertyType == typeof(decimal))
                return "REAL";
            else if (propertyType == typeof(string))
                return maxLengthAttr != null ? $"TEXT({maxLengthAttr.Value})" : "TEXT";
            else if (propertyType == typeof(DateTime))
                return "TEXT";
            else if (propertyType == typeof(byte[]))
                return "BLOB";
            else
                return "TEXT"; // Default for complex types (stored as JSON or similar)
        }

        /// <summary>
        /// To add the column in given table
        /// </summary>
        /// <param name="tableName">Table name to add the column</param>
        /// <param name="columnsToAdd">List of columns to add</param>
        /// <returns>Returns a Task</returns>
        private async Task AddColumnsAsync(string tableName, List<ColumnInfo> columnsToAdd)
        {
            foreach (var column in columnsToAdd)
            {
                string nullConstraint = column.NotNull ? "NOT NULL" : "";
                string defaultValue = GetDefaultValueForType(column.Property.PropertyType);

                // Properly escape the table name
                string query = $"ALTER TABLE [{tableName}] ADD COLUMN {column.Name} {column.Type} {nullConstraint} {defaultValue}";
                await _database.ExecuteAsync(query);

                Console.WriteLine($"Added column {column.Name} to table {tableName}");
            }
        }

        /// <summary>
        /// To get the default value for the datatype
        /// </summary>
        /// <param name="type">Type of the field</param>
        /// <returns>Returns a default value string</returns>
        private string GetDefaultValueForType(Type type)
        {
            // Handle nullable types
            bool isNullable = Nullable.GetUnderlyingType(type) != null;
            if (isNullable)
                return ""; // No default value needed for nullable types

            // Get the underlying type if it's nullable
            Type actualType = isNullable ? Nullable.GetUnderlyingType(type) : type;

            // Provide appropriate default values for non-nullable types
            if (actualType == typeof(int) || actualType == typeof(long) || actualType.IsEnum)
                return "DEFAULT 0";
            else if (actualType == typeof(bool))
                return "DEFAULT 0"; // 0 for false
            else if (actualType == typeof(float) || actualType == typeof(double) || actualType == typeof(decimal))
                return "DEFAULT 0.0";
            else if (actualType == typeof(string))
                return "DEFAULT ''";
            else if (actualType == typeof(DateTime))
                return "DEFAULT '0001-01-01T00:00:00'"; // MinValue for DateTime
            else
                return ""; // No default for complex types
        }

        /// <summary>
        /// To get the table name from entity
        /// </summary>
        /// <param name="entityType">The entity to check</param>
        /// <returns>Returns a table name string</returns>
        private string GetTableName(Type entityType)
        {
            var tableAttr = entityType.GetCustomAttribute<TableAttribute>();
            return tableAttr?.Name ?? entityType.Name;
        }

        private class TableInfo
        {
            public int Cid { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public int NotNull { get; set; }
            public string DefaultValue { get; set; }
            public int Pk { get; set; }
        }

        public class ColumnInfo
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool NotNull { get; set; }
            public bool IsPrimaryKey { get; set; }
            public PropertyInfo Property { get; set; }
        }
    }
}
