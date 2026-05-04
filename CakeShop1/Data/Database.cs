using System.Data;
using System.Data.SqlClient;

namespace CakeShop1.Data
{
    public class Database
    {
        private readonly string connectionString =
            @"Data Source=DESKTOP-DD2O2C7;Initial Catalog=CAKE_SHOP;Integrated Security=True";

        public string ConnectionString => connectionString;

        public DataTable ExecuteSelect(string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);

                return table;
            }
        }

        public DataTable ExecuteSelect(string query, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(parameters);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                return table;
            }
        }

        public void ExecuteCommand(string query, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(parameters);

                cmd.ExecuteNonQuery();
            }
        }
    }
}