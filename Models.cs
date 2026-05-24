// =================================================================
//  VelvetBeans Cafe Management System
//  File: Models/Models.cs
//  Purpose: Plain C# classes that mirror each database table.
//           These are used to carry data between the database
//           layer and the UI forms — like containers for rows.
// =================================================================

using System.Collections.Generic;

namespace VelvetBeans.Models
{
    // -----------------------------------------------------------------
    //  User — represents one row in the Users table.
    //  A User is a staff member who can log into the system.
    //  Role is either "Admin" (full access) or "Cashier" (limited).
    // -----------------------------------------------------------------
    public class User
    {
        public int    UserId    { get; set; }  // Primary key, auto-set by database
        public string Username  { get; set; } = string.Empty;
        public string Password  { get; set; } = string.Empty;
        public string Role      { get; set; } = "Cashier";  // "Admin" or "Cashier"
        public string CreatedAt { get; set; } = string.Empty;  // datetime string from SQLite
    }

    // -----------------------------------------------------------------
    //  Order — represents one row in the Orders table.
    //  An order groups together all the items a customer ordered.
    //  Status flow: Pending → Preparing → Completed (or Cancelled)
    // -----------------------------------------------------------------
    public class Order
    {
        public int     OrderId     { get; set; }
        public int     UserId      { get; set; }  // Which cashier created this order
        public string  CashierName { get; set; } = string.Empty;  // Joined from Users table
        public int     TableNumber { get; set; }  // 0 means Takeaway
        public string  Status      { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }  // Sum of all item prices × quantities
        public decimal Discount    { get; set; }  // Amount knocked off (manual discount)
        public string  CreatedAt   { get; set; } = string.Empty;

        // List of individual lines in this order (e.g. 2× Latte, 1× Croissant)
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        // Computed property — no database column needed
        // This is TotalAmount minus any discount applied
        public decimal GrandTotal => TotalAmount - Discount;
    }

    // -----------------------------------------------------------------
    //  OrderItem — represents one row in the OrderItems table.
    //  Each OrderItem is one product line within an order.
    //  E.g. "3 × Cappuccino @ £3.50 each = £10.50"
    // -----------------------------------------------------------------
    public class OrderItem
    {
        public int     ItemId    { get; set; }  // Which menu item this refers to
        public string  ItemName  { get; set; } = string.Empty;  // Joined from MenuItems
        public int     Quantity  { get; set; }  // How many of this item
        public decimal UnitPrice { get; set; }  // Price at time of order (snapshot)

        // Computed: Quantity × UnitPrice — no database column needed
        public decimal Subtotal => Quantity * UnitPrice;
    }

    // -----------------------------------------------------------------
    //  Session — NOT a database table. This is an in-memory holder
    //  for the currently logged-in user. It's static so any form in
    //  the app can read it without needing a reference passed in.
    //  It gets cleared (set to null) when the user logs out.
    // -----------------------------------------------------------------
    public static class Session
    {
        // The currently logged-in user, or null if nobody is logged in
        public static User? CurrentUser { get; set; }

        // Shortcut: true if the logged-in user has the Admin role
        public static bool IsAdmin => CurrentUser?.Role == "Admin";

        // Call this on logout — resets the session back to "nobody logged in"
        public static void Clear() => CurrentUser = null;
    }
}
