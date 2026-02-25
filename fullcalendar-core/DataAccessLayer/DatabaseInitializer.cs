using Microsoft.Data.Sqlite;
using System;

namespace fullcalendarcore.DataAccessLayer
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Create table if not exists
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Events (
                        event_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        title TEXT NOT NULL,
                        description TEXT,
                        event_start TEXT NOT NULL,
                        event_end TEXT,
                        all_day INTEGER NOT NULL DEFAULT 0,
                        user_id TEXT,
                        user_name TEXT,
                        event_type INTEGER NOT NULL DEFAULT 0,
                        is_recurring INTEGER NOT NULL DEFAULT 0,
                        recurrence_pattern INTEGER NOT NULL DEFAULT 0,
                        recurrence_interval INTEGER NOT NULL DEFAULT 1,
                        recurrence_end_date TEXT,
                        parent_event_id INTEGER
                    )";

                createTableCommand.ExecuteNonQuery();

                // Create EventShares table if not exists
                var createSharesTableCommand = connection.CreateCommand();
                createSharesTableCommand.CommandText = @"
                    CREATE TABLE IF NOT EXISTS EventShares (
                        share_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        event_id INTEGER NOT NULL,
                        owner_user_id TEXT NOT NULL,
                        shared_with_user_id TEXT NOT NULL,
                        shared_with_user_name TEXT,
                        can_edit INTEGER NOT NULL DEFAULT 0,
                        shared_date TEXT NOT NULL,
                        FOREIGN KEY (event_id) REFERENCES Events(event_id) ON DELETE CASCADE
                    )";

                createSharesTableCommand.ExecuteNonQuery();

                // Add user_id column if it doesn't exist (for existing databases)
                AddColumnIfNotExists(connection, "Events", "user_id", "TEXT");

                // Add user_name column if it doesn't exist (for existing databases)
                AddColumnIfNotExists(connection, "Events", "user_name", "TEXT");

                // Add event_type column if it doesn't exist
                AddColumnIfNotExists(connection, "Events", "event_type", "INTEGER NOT NULL DEFAULT 0");

                // Add recurrence columns if they don't exist
                AddColumnIfNotExists(connection, "Events", "is_recurring", "INTEGER NOT NULL DEFAULT 0");
                AddColumnIfNotExists(connection, "Events", "recurrence_pattern", "INTEGER NOT NULL DEFAULT 0");
                AddColumnIfNotExists(connection, "Events", "recurrence_interval", "INTEGER NOT NULL DEFAULT 1");
                AddColumnIfNotExists(connection, "Events", "recurrence_end_date", "TEXT");
                AddColumnIfNotExists(connection, "Events", "parent_event_id", "INTEGER");

                // Normalize existing date formats to ISO 8601
                NormalizeDateFormats(connection);
            }
        }

        private void AddColumnIfNotExists(SqliteConnection connection, string tableName, string columnName, string columnType)
        {
            var checkColumnCmd = connection.CreateCommand();
            checkColumnCmd.CommandText = $"PRAGMA table_info({tableName})";

            bool columnExists = false;
            using (var reader = checkColumnCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetString(1).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        columnExists = true;
                        break;
                    }
                }
            }

            if (!columnExists)
            {
                var addColumnCmd = connection.CreateCommand();
                addColumnCmd.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType}";
                addColumnCmd.ExecuteNonQuery();
            }
        }

        private void NormalizeDateFormats(SqliteConnection connection)
        {
            // Get all events
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT event_id, event_start, event_end FROM Events";

            using (var reader = selectCmd.ExecuteReader())
            {
                var updates = new System.Collections.Generic.List<(int id, string start, string end)>();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string start = reader.IsDBNull(1) ? null : reader.GetString(1);
                    string end = reader.IsDBNull(2) ? null : reader.GetString(2);

                    // Try to parse and normalize dates
                    string normalizedStart = NormalizeDate(start);
                    string normalizedEnd = NormalizeDate(end);

                    if (normalizedStart != start || normalizedEnd != end)
                    {
                        updates.Add((id, normalizedStart, normalizedEnd));
                    }
                }

                reader.Close();

                // Update normalized dates
                foreach (var update in updates)
                {
                    var updateCmd = connection.CreateCommand();
                    updateCmd.CommandText = @"
                        UPDATE Events 
                        SET event_start = @start, event_end = @end 
                        WHERE event_id = @id";
                    updateCmd.Parameters.AddWithValue("@id", update.id);
                    updateCmd.Parameters.AddWithValue("@start", update.start);
                    updateCmd.Parameters.AddWithValue("@end", update.end ?? (object)DBNull.Value);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }

        private string NormalizeDate(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return dateStr;

            // Remove comma if present (Dutch format: "24-02-2026, 00:00")
            dateStr = dateStr.Replace(",", "");

            // Try to parse ISO format first
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd HH:mm:ss", 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out DateTime isoDate))
            {
                return isoDate.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Try Dutch format: "24-02-2026 00:00"
            if (DateTime.TryParseExact(dateStr, "dd-MM-yyyy HH:mm", 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out DateTime nlDate))
            {
                return nlDate.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Try US format: "02/24/2026 12:00 AM"
            if (DateTime.TryParse(dateStr, System.Globalization.CultureInfo.GetCultureInfo("en-US"), 
                System.Globalization.DateTimeStyles.None, out DateTime usDate))
            {
                return usDate.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Last resort: try any culture
            if (DateTime.TryParse(dateStr, out DateTime anyDate))
            {
                return anyDate.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return dateStr; // Return as-is if all parsing fails
        }
    }
}
