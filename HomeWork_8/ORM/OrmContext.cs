using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace ORM
{
    public sealed class OrmContext : IOrmContext
    {
        private readonly string _connectionString;

        public OrmContext(string connectionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
            _connectionString = connectionString;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync<T>(string tableName, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            string safeTableName = EscapeIdentifier(tableName);
            string sql = $"SELECT * FROM {safeTableName}";

            var result = new List<T>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            PropertyInfo[] properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanWrite && property.SetMethod?.IsPublic == true)
                .ToArray();

            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < reader.FieldCount; index++)
            {
                columns[reader.GetName(index)] = index;
            }

            while (await reader.ReadAsync(cancellationToken))
            {
                T entity = new();

                foreach (PropertyInfo property in properties)
                {
                    if (!columns.TryGetValue(property.Name, out int ordinal))
                    {
                        continue;
                    }

                    if (reader.IsDBNull(ordinal))
                    {
                        if (!property.PropertyType.IsValueType ||
                            Nullable.GetUnderlyingType(property.PropertyType) != null)
                        {
                            property.SetValue(entity, null);
                        }
                        continue;
                    }

                    object value = reader.GetValue(ordinal);
                    Type targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    object convertedValue = targetType.IsInstanceOfType(value)
                        ? value
                        : targetType.IsEnum
                            ? Enum.ToObject(targetType, value)
                            : Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);

                    property.SetValue(entity, convertedValue);
                }

                result.Add(entity);
            }

            return result;
        }

        public async Task<int> InsertAsync<T>(string tableName, T entity, CancellationToken cancellationToken = default)
            where T : class
        {
            string safeTableName = EscapeIdentifier(tableName);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanWrite && property.SetMethod?.IsPublic == true)
                .ToArray();

            var propertyNames = new List<string>();
            var parameterNames = new List<string>();
            var parameterValues = new List<object>();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "Id" && property.PropertyType == typeof(int))
                {
                    continue;
                }

                string name = property.Name;
                object? value = property.GetValue(entity);

                propertyNames.Add(name);
                parameterNames.Add($"@{name}");
                parameterValues.Add(value ?? DBNull.Value);
            }

            string columnsString = string.Join(", ", propertyNames);
            string parametersString = string.Join(", ", parameterNames);
            string sql = $"INSERT INTO {safeTableName} ({columnsString}) VALUES ({parametersString})";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);

            for (int i = 0; i < parameterValues.Count; i++)
            {
                command.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
            }

            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            return rowsAffected;
        }

        public async Task<int> UpdateAsync<T>(string tableName, int id, T entity, CancellationToken cancellationToken = default)
            where T : class
        {
            string safeTableName = EscapeIdentifier(tableName);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanWrite && property.SetMethod?.IsPublic == true)
                .ToArray();

            var setClauses = new List<string>();
            var parameterNames = new List<string>();
            var parameterValues = new List<object>();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "Id")
                {
                    continue;
                }

                string name = property.Name;
                object? value = property.GetValue(entity);

                setClauses.Add($"\"{name}\" = @{name}");
                parameterNames.Add($"@{name}");
                parameterValues.Add(value ?? DBNull.Value);
            }

            parameterNames.Add("@UpdateId");
            parameterValues.Add(id);

            string setClause = string.Join(", ", setClauses);
            string sql = $"UPDATE {safeTableName} SET {setClause} WHERE \"Id\" = @UpdateId";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);

            for (int i = 0; i < parameterValues.Count; i++)
            {
                command.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
            }

            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            return rowsAffected;
        }

        public async Task<int> DeleteAsync(string tableName, int id, CancellationToken cancellationToken = default)
        {
            string safeTableName = EscapeIdentifier(tableName);
            string sql = $"DELETE FROM {safeTableName} WHERE \"Id\" = @id";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            return rowsAffected;
        }

        private static string EscapeIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier) ||
                !Regex.IsMatch(identifier, @"^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                throw new ArgumentException("Недопустимое имя SQL-идентификатора.", nameof(identifier));
            }
            return $"\"{identifier}\"";
        }
    }
}