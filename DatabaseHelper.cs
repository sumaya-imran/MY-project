// =================================================================
//  VelvetBeans Cafe Management System
//  File: Database/DatabaseHelper.cs
//  Purpose: The single place for ALL database operations.
//           Uses SQLite (via Microsoft.Data.Sqlite NuGet package).
//
//  DB file location: same folder as VelvetBeans.exe
//  Open it with DB Browser for SQLite to inspect/edit data directly.
//  Path: bin\Debug\net6.0-windows\VelvetBeans.db
// =================================================================

using System;
using System.Data;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;

namespace VelvetBeans.Database
{
    public static class DatabaseHelper
    {
        // -----------------------------------------------------------------
        //  Connection string — points to VelvetBeans.db in the same
        //  folder as the running .exe. AppDomain.CurrentDomain.BaseDirectory
        //  gives us that folder at runtime.
        // -----------------------------------------------------------------
        private static readonly string DbPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VelvetBeans.db");

        public static string ConnectionString => "Data Source=" + DbPath;

        // =================================================================
        //  InitialiseDatabase
        //  Called ONCE at application startup (from Program.cs).
        //  Creates the .db file and all tables if they don't exist.
        //  Also inserts default accounts and sample menu data.
        //  Safe to call every run — uses "IF NOT EXISTS" and "INSERT OR IGNORE".
        // =================================================================
        public static void InitialiseDatabase()
        {
            using (var con = OpenConnection())
            {
                // ----------------------------------------------------------
                //  TABLE: Users
                //  Stores staff accounts. Role = 'Admin' or 'Cashier'.
                // ----------------------------------------------------------
                Run(con, null,
                    "CREATE TABLE IF NOT EXISTS Users (" +
                    "  UserId    INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "  Username  TEXT NOT NULL UNIQUE," +
                    "  Password  TEXT NOT NULL," +
                    "  Role      TEXT NOT NULL DEFAULT 'Cashier'," +
                    "  CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))" +
                    ");");

                // ----------------------------------------------------------
                //  TABLE: Categories
                //  Groups menu items (e.g. Hot Drinks, Food, Desserts).
                //  Items belong to exactly one category.
                // ----------------------------------------------------------
                Run(con, null,
                    "CREATE TABLE IF NOT EXISTS Categories (" +
                    "  CategoryId  INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "  Name        TEXT NOT NULL UNIQUE," +
                    "  Description TEXT" +
                    ");");

                // ----------------------------------------------------------
                //  TABLE: MenuItems
                //  Each row is one product on the cafe menu.
                //  IsAvailable: 1 = shown in POS, 0 = hidden (out of stock).
                // ----------------------------------------------------------
                Run(con, null,
                    "CREATE TABLE IF NOT EXISTS MenuItems (" +
                    "  ItemId      INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "  CategoryId  INTEGER NOT NULL," +
                    "  Name        TEXT NOT NULL," +
                    "  Description TEXT," +
                    "  Price       REAL NOT NULL," +
                    "  IsAvailable INTEGER NOT NULL DEFAULT 1," +
                    "  FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)" +
                    ");");

                // ----------------------------------------------------------
                //  TABLE: Orders
                //  One row per customer order. Links to the cashier (UserId).
                //  Status: Pending → Preparing → Completed | Cancelled
                // ----------------------------------------------------------
                Run(con, null,
                    "CREATE TABLE IF NOT EXISTS Orders (" +
                    "  OrderId     INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "  UserId      INTEGER NOT NULL," +
                    "  TableNumber INTEGER NOT NULL DEFAULT 0," +
                    "  Status      TEXT NOT NULL DEFAULT 'Pending'," +
                    "  TotalAmount REAL NOT NULL DEFAULT 0," +
                    "  Discount    REAL NOT NULL DEFAULT 0," +
                    "  CreatedAt   TEXT NOT NULL DEFAULT (datetime('now'))," +
                    "  CompletedAt TEXT," +
                    "  FOREIGN KEY (UserId) REFERENCES Users(UserId)" +
                    ");");

                // ----------------------------------------------------------
                //  TABLE: OrderItems
                //  Each row is one product line within an Order.
                //  E.g. OrderId=5, ItemId=3, Quantity=2, UnitPrice=3.50
                // ----------------------------------------------------------
                Run(con, null,
                    "CREATE TABLE IF NOT EXISTS OrderItems (" +
                    "  OrderItemId INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "  OrderId     INTEGER NOT NULL," +
                    "  ItemId      INTEGER NOT NULL," +
                    "  Quantity    INTEGER NOT NULL," +
                    "  UnitPrice   REAL NOT NULL," +
                    "  FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)," +
                    "  FOREIGN KEY (ItemId)  REFERENCES MenuItems(ItemId)" +
                    ");");

                // ------ Seed: default staff accounts ----------------------
                // INSERT OR IGNORE means it skips if the username already exists
                Run(con, null, "INSERT OR IGNORE INTO Users (Username,Password,Role) VALUES ('admin',  'admin123',   'Admin');");
                Run(con, null, "INSERT OR IGNORE INTO Users (Username,Password,Role) VALUES ('cashier','cashier123', 'Cashier');");

                // ------ Seed: default categories --------------------------
                Run(con, null, "INSERT OR IGNORE INTO Categories (Name,Description) VALUES ('Hot Drinks',  'Hot coffee and tea');");
                Run(con, null, "INSERT OR IGNORE INTO Categories (Name,Description) VALUES ('Cold Drinks', 'Iced and chilled drinks');");
                Run(con, null, "INSERT OR IGNORE INTO Categories (Name,Description) VALUES ('Food',        'Snacks and light meals');");
                Run(con, null, "INSERT OR IGNORE INTO Categories (Name,Description) VALUES ('Desserts',    'Sweet treats and cakes');");

                // ------ Seed: sample menu items ---------------------------
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (1,'Espresso',     'Single rich espresso shot',    2.50);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (1,'Cappuccino',   'Espresso with milk foam',      3.50);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (1,'Latte',        'Espresso with steamed milk',   3.75);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (1,'Americano',    'Espresso with hot water',      3.00);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (1,'Flat White',   'Ristretto with micro-foam',    3.50);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (1,'Hot Chocolate','Rich Belgian hot chocolate',   3.25);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (2,'Iced Latte',   'Chilled latte over ice',       4.00);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (2,'Cold Brew',    '12-hour steeped cold coffee',  4.50);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (2,'Iced Mocha',   'Chocolate espresso over ice',  4.75);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (2,'Fruit Smoothie','Seasonal fresh fruit blend',  4.00);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (3,'Croissant',    'Butter croissant, baked fresh',2.50);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (3,'Club Sandwich','Triple-decker chicken club',   6.50);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (3,'Avocado Toast','Sourdough with smashed avo',   5.50);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (3,'Panini',       'Toasted mozzarella panini',    5.00);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (4,'Choc Cake',    'Rich dark chocolate slice',    4.00);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (4,'Cheesecake',   'New York plain cheesecake',    4.25);");
                Run(con, null, "INSERT OR IGNORE INTO MenuItems (CategoryId,Name,Description,Price) VALUES (4,'Brownie',      'Warm fudge brownie',           3.00);");
            }
        }

        // =================================================================
        //  OpenConnection
        //  Creates and opens a new SQLite connection. Remember to wrap
        //  it in a using() block so it closes automatically.
        // =================================================================
        public static SqliteConnection OpenConnection()
        {
            var con = new SqliteConnection(ConnectionString);
            con.Open();
            return con;
        }

        // =================================================================
        //  Run — Execute a non-query (INSERT / UPDATE / DELETE / CREATE)
        //  on an EXISTING open connection.
        //
        //  WHY tx parameter? Microsoft.Data.Sqlite requires that any
        //  command running inside a transaction must have its .Transaction
        //  property set to that transaction. If you forget this, SQLite
        //  throws "SqliteException: cannot start a transaction within a
        //  transaction". Pass null when NOT inside a transaction.
        //
        //  WHY SqliteParameter[]? Using parameters (@name) instead of
        //  string concatenation prevents SQL Injection attacks and also
        //  handles special characters in text correctly.
        // =================================================================
        public static void Run(
            SqliteConnection   con,   // The open connection to use
            SqliteTransaction? tx,    // Active transaction, or null
            string             sql,   // The SQL statement to execute
            SqliteParameter[]? p = null) // Optional parameters (@name = value)
        {
            using (var cmd = new SqliteCommand(sql, con))
            {
                if (tx != null) cmd.Transaction = tx;  // Enrol in transaction
                if (p  != null) cmd.Parameters.AddRange(p);
                cmd.ExecuteNonQuery();
            }
        }

        // =================================================================
        //  Query — Runs a SELECT and returns results as a DataTable.
        //  Opens its own connection internally.
        //
        //  WHY NOT dt.Load(reader)? The standard ADO.NET Load() method
        //  re-enables primary-key uniqueness constraints on the DataTable.
        //  When a JOIN returns two columns with the same name (e.g.
        //  m.Name and c.Name both called "Name"), it throws:
        //    System.Data.ConstraintException: Failed to enable constraints
        //  We avoid this by reading rows manually row by row.
        //
        //  ALL VALUES ARE RETURNED AS STRINGS. Use ParseInt() and
        //  ParseDecimal() defined below to convert them safely.
        // =================================================================
        public static DataTable Query(string sql, SqliteParameter[]? p = null)
        {
            using (var con = OpenConnection())
            using (var cmd = new SqliteCommand(sql, con))
            {
                if (p != null) cmd.Parameters.AddRange(p);

                var dt = new DataTable();

                using (var reader = cmd.ExecuteReader())
                {
                    // Step 1: Build the column list from the query's output schema.
                    // If two columns have the same name (from a JOIN), append _2, _3 etc.
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i);
                        string safe = name;
                        int    n    = 2;
                        while (dt.Columns.Contains(safe))
                            safe = name + "_" + n++;       // e.g. "Name_2"
                        dt.Columns.Add(safe, typeof(string));
                    }

                    // Step 2: Read every row and add to the DataTable.
                    // DBNull (SQL NULL) becomes empty string for safety.
                    while (reader.Read())
                    {
                        var row = dt.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                            row[i] = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString()!;
                        dt.Rows.Add(row);
                    }
                }

                return dt;
            }
        }

        // =================================================================
        //  Scalar — Returns a single value (first column, first row).
        //  Useful for COUNT(*), SUM(...), MAX(...) queries.
        //  Returns null if the query returned no rows.
        // =================================================================
        public static object? Scalar(string sql, SqliteParameter[]? p = null)
        {
            using (var con = OpenConnection())
            using (var cmd = new SqliteCommand(sql, con))
            {
                if (p != null) cmd.Parameters.AddRange(p);
                return cmd.ExecuteScalar();
            }
        }

        // =================================================================
        //  Execute — Runs a non-query on its OWN connection.
        //  Use for standalone INSERT / UPDATE / DELETE that don't
        //  need to be part of a multi-step transaction.
        //  Returns the number of rows affected.
        // =================================================================
        public static int Execute(string sql, SqliteParameter[]? p = null)
        {
            using (var con = OpenConnection())
            using (var cmd = new SqliteCommand(sql, con))
            {
                if (p != null) cmd.Parameters.AddRange(p);
                return cmd.ExecuteNonQuery();
            }
        }

        // =================================================================
        //  LastId — Returns the rowid of the last INSERT on this connection.
        //  MUST be called on the SAME connection as the INSERT, before
        //  it is closed. Used when placing orders to get the new OrderId.
        // =================================================================
        public static long LastId(SqliteConnection con, SqliteTransaction? tx = null)
        {
            using (var cmd = new SqliteCommand("SELECT last_insert_rowid();", con))
            {
                if (tx != null) cmd.Transaction = tx;
                return (long)(cmd.ExecuteScalar() ?? 0L);
            }
        }

        // =================================================================
        //  ParseDecimal — Safely converts a string value to decimal.
        //
        //  WHY InvariantCulture? SQLite always stores numbers with a dot
        //  as the decimal separator (e.g. "3.50"). However, Windows
        //  systems in countries like Germany or France use a comma
        //  (e.g. "3,50"). Without InvariantCulture, decimal.TryParse
        //  would silently return 0 for "3.50" on those systems.
        //  InvariantCulture always uses a dot — matches SQLite.
        // =================================================================
        public static decimal ParseDecimal(object? value)
        {
            if (value == null) return 0m;
            string s = value.ToString()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(s)) return 0m;
            // NumberStyles.Any allows leading/trailing spaces, sign symbols, decimals
            decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result);
            return result;
        }

        // =================================================================
        //  ParseInt — Safely converts a string value to int.
        //  Returns 0 if parsing fails (never throws).
        // =================================================================
        public static int ParseInt(object? value)
        {
            if (value == null) return 0;
            int.TryParse(value.ToString()?.Trim(), out int result);
            return result;
        }
    }
}
