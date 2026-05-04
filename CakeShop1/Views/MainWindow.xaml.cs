using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using CakeShop1.Data;
using CakeShop1.Helpers;

namespace CakeShop1.Views
{
    public partial class MainWindow : Window
    {
        Database db = new Database();

        public MainWindow()
        {
            InitializeComponent();
            LoadOrders();
            //  sakrij dugme ako nije admin
            if (!Session.IsAdmin)
            {
                btnChangeStatus.Visibility = Visibility.Collapsed;
            }

            
        }

        private void LoadOrders()
        {
            DataTable dt;

            // ✅ ADMIN vidi sve
            if (Session.IsAdmin)
            {
                dt = db.ExecuteSelect("SELECT OrderID, TotalAmount, Status FROM [Order]");
            }
            // ✅ CUSTOMER vidi samo svoje
            else
            {
                dt = db.ExecuteSelect(
                    "SELECT OrderID, TotalAmount, Status FROM [Order] WHERE CustomerID=@id",
                    new System.Data.SqlClient.SqlParameter[]
                    {
                        new System.Data.SqlClient.SqlParameter("@id", Session.CustomerID)
                    });
            }

            dataGrid.ItemsSource = dt.DefaultView;

            // ✅ NO DATA logika
            if (dt.Rows.Count == 0)
            {
                txtNoData.Visibility = Visibility.Visible;
                dataGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoData.Visibility = Visibility.Collapsed;
                dataGrid.Visibility = Visibility.Visible;
            }
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().ShowDialog();
            LoadOrders();
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            // ❗ SAMO ADMIN SME
            if (!Session.IsAdmin)
            {
                MessageBox.Show("Only admin can change order status!");
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Select order first!");
                return;
            }

            DataRowView row = (DataRowView)dataGrid.SelectedItem;
            int orderId = (int)row["OrderID"];

            ChangeStatusWindow csw = new ChangeStatusWindow(orderId);
            csw.ShowDialog();

            LoadOrders(); // refresh
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log("User logged out");
            // 🔥 reset session
            Session.IsAdmin = false;
            Session.CustomerID = 0;

            // vrati na login
            LoginWindow lw = new LoginWindow();
            lw.Show();

            this.Close();
        }
    }
}