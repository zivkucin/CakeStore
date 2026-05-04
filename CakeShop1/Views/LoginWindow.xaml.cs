using System.Data;
using System.Data.SqlClient;
using System.Windows;
using CakeShop1.Data;
using System.Threading.Tasks;
using System;
using System.IO;
using CakeShop1.Helpers;

namespace CakeShop1.Views
{
    public partial class LoginWindow : Window
    {
        Database db = new Database();
       
        public LoginWindow()
        {
            InitializeComponent();
        }

        // 🔐 LOGIN
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Enter email and password!");
                return;
            }

            // ======================
            // CUSTOMER LOGIN
            // ======================
            string queryCustomer = @"SELECT * FROM Customer 
                            WHERE Email=@email AND Password=@pass";

            SqlParameter[] paramCustomer =
            {
        new SqlParameter("@email", txtEmail.Text),
        new SqlParameter("@pass", txtPassword.Password)
    };

            DataTable dtCustomer = db.ExecuteSelect(queryCustomer, paramCustomer);

            if (dtCustomer.Rows.Count > 0)
            {
               
                Session.IsAdmin = false;
                Session.CustomerID = Convert.ToInt32(dtCustomer.Rows[0]["CustomerID"]);

                Logger.Log("Login success - CustomerID=" + Session.CustomerID);


                MessageBox.Show("Login successful (Customer)!");

                await Task.Delay(500); // smanji na 0.5s (2s je previše)

                MainWindow mw = new MainWindow();
                mw.Show();

                this.Close();
                return;
            }

            // ======================
            // ADMIN LOGIN
            // ======================
            string queryEmployee = @"SELECT * FROM Employee 
                            WHERE Username=@user AND Password=@pass";

            SqlParameter[] paramEmployee =
            {
        new SqlParameter("@user", txtEmail.Text),
        new SqlParameter("@pass", txtPassword.Password)
    };

            DataTable dtEmployee = db.ExecuteSelect(queryEmployee, paramEmployee);

            if (dtEmployee.Rows.Count > 0)
            {
                // 🔥 KLJUČNO
                Session.IsAdmin = true;

                MessageBox.Show("Login successful (Admin)!");
                Logger.Log("Admin login");

                MainWindow mw = new MainWindow();
                mw.Show();

                this.Close();
                return;
            }

            MessageBox.Show("Invalid credentials!");
        }

        // 📝 REGISTER
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow rw = new RegisterWindow();
            rw.ShowDialog();
        }
    }
}