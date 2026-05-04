using System.Data.SqlClient;
using System.Windows;
using CakeShop1.Data;
using CakeShop1.Helpers;

namespace CakeShop1.Views
{
    public partial class ChangeStatusWindow : Window
    {
        private int orderId;
        Database db = new Database();

        public ChangeStatusWindow(int id)
        {
            InitializeComponent();
            orderId = id;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Select status!");
                return;
            }

            string status = (cbStatus.SelectedItem as System.Windows.Controls.ComboBoxItem).Content.ToString();

            string query = "UPDATE [Order] SET Status=@status WHERE OrderID=@id";

            SqlParameter[] param =
            {
                new SqlParameter("@status", status),
                new SqlParameter("@id", orderId)
            };

            db.ExecuteCommand(query, param);
            Logger.Log("Order " + orderId + " status changed to " + status);

            this.Close();
        }
    }
}