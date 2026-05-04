using System;
using System.Data.SqlClient;
using System.Windows;
using CakeShop1.Data;

namespace CakeShop1.Views
{
    public partial class RegisterWindow : Window
    {
        Database db = new Database();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // VALIDACIJA
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("All fields are required!");
                return;
            }

            try
            {
                string query = @"INSERT INTO Customer (FirstName, LastName, Email, Password)
                                 VALUES (@fn, @ln, @em, @pass)";

                SqlParameter[] param =
                {
                    new SqlParameter("@fn", txtFirstName.Text),
                    new SqlParameter("@ln", txtLastName.Text),
                    new SqlParameter("@em", txtEmail.Text),
                    new SqlParameter("@pass", txtPassword.Password)
                };

                db.ExecuteCommand(query, param);

                MessageBox.Show("Registration successful!");

                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("User already exists or error!");
            }
        }
    }
}