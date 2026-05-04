using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using CakeShop1.Data;
using CakeShop1.Helpers;

namespace CakeShop1.Views
{
    public partial class OrderWindow : Window
    {
        Database db = new Database();

        // 🛒 Cart lista
        List<CartItem> cart = new List<CartItem>();

        decimal selectedPrice = 0;

        public OrderWindow()
        {
            InitializeComponent();
            LoadProducts();
        }

        // 🔹 Model za korpu
        class CartItem
        {
            public int ProductID { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Total => Quantity * Price;
        }

        // 🔹 Load products
        private void LoadProducts()
        {
            string query = "SELECT ProductID, Name, Price FROM Product";
            DataTable dt = db.ExecuteSelect(query);

            cbProduct.ItemsSource = dt.DefaultView;
            cbProduct.DisplayMemberPath = "Name";
            cbProduct.SelectedValuePath = "ProductID";
        }

        // 🔹 Kad biraš proizvod
        private void cbProduct_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbProduct.SelectedItem is DataRowView row)
            {
                selectedPrice = Convert.ToDecimal(row["Price"]);
            }
        }

        // 🔹 Add to cart
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbProduct.SelectedItem is DataRowView row)
                {
                    int quantity = int.Parse(txtQuantity.Text);

                    CartItem item = new CartItem
                    {
                        ProductID = (int)row["ProductID"],
                        Name = row["Name"].ToString(),
                        Price = (decimal)row["Price"],
                        Quantity = quantity
                    };

                    cart.Add(item);

                    dgCart.ItemsSource = null;
                    dgCart.ItemsSource = cart;

                    UpdateTotal();
                }
            }
            catch
            {
                MessageBox.Show("Invalid input!");
            }
        }

        // 🔹 Update total
        private void UpdateTotal()
        {
            decimal total = cart.Sum(x => x.Total);
            txtTotal.Text = total + " RSD";
        }

        // 🔹 Create order
        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Cart is empty!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Enter delivery address!");
                return;
            }

            try
            {
                decimal total = cart.Sum(x => x.Total);

                using (SqlConnection conn = new SqlConnection(db.ConnectionString))
                {
                    conn.Open();

                    // ======================
                    // 1. INSERT ORDER
                    // ======================
                    string orderQuery = @"
INSERT INTO [Order] (CustomerID, TotalAmount, Status, DeliveryAddress)
VALUES (@customerId, @total, 'Pending', @address);
SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(orderQuery, conn);

                    cmd.Parameters.AddWithValue("@customerId", Session.CustomerID);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text);

                    int orderId = Convert.ToInt32(cmd.ExecuteScalar());
                    Logger.Log("Order created - ID=" + orderId + " CustomerID=" + Session.CustomerID);

                    // ======================
                    // 2. INSERT ITEMS
                    // ======================
                    foreach (var item in cart)
                    {
                        string itemQuery = @"
INSERT INTO OrderItem (OrderID, ProductID, Quantity)
VALUES (@orderId, @productId, @quantity)";

                        SqlCommand itemCmd = new SqlCommand(itemQuery, conn);

                        itemCmd.Parameters.AddWithValue("@orderId", orderId);
                        itemCmd.Parameters.AddWithValue("@productId", item.ProductID);
                        itemCmd.Parameters.AddWithValue("@quantity", item.Quantity);

                        itemCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Order created successfully!");

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message); // 🔥 sada vidiš pravi problem
            }
        }
    }
}