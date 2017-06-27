using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows;

namespace Forwarder_Server.Sources
{
    class DatabaseWork
    {
        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\ForwarderDatabase.mdf;Integrated Security=True";

        public static void Test()
        {
            SqlConnection сonnection = new SqlConnection(connectionString);
            try
            {
                сonnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM[Users]", сonnection);

                DataTable dataTable = new DataTable();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dataTable);
                da.Dispose();

                Functions.AddJournalEntry(": __DATABASE__ Проверка подключения прошла успешно.");
            }
            catch (Exception exp)
            {
                Functions.AddJournalEntry(": __ERROR DATABASE__ Ошибка при подключении к базе данных: " + exp.Message);
            }
            finally
            {
                сonnection.Close();
            }
        }

        public static DataTable RunProcedure(String procedureName)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(procedureName, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader reader = command.ExecuteReader();

                DataTable dataTable = new DataTable();

                if (reader.HasRows)
                {
                    dataTable.Load(reader);
                }
                reader.Close();

                return dataTable;
            }
            catch (Exception exp)
            {
                Functions.AddJournalEntry(": __ERROR DATABASE__ Ошибка при подключении к базе данных: " + exp.Message);
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public static DataTable ExecuteQuery(String inquiry)
        {
            SqlConnection сonnection = new SqlConnection(connectionString);
            try
            {
                сonnection.Open();

                SqlCommand cmd = new SqlCommand(inquiry, сonnection);

                DataTable dataTable = new DataTable();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dataTable);
                da.Dispose();

                return dataTable;
            }
            catch (Exception exp)
            {
                Functions.AddJournalEntry(": __ERROR DATABASE__ Ошибка при подключении к базе данных: " + exp.Message);
                return null;
            }
            finally
            {
                сonnection.Close();
            }
        }

        public static void ExecuteUpdate(String inquiry)
        {

            SqlConnection сonnection = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(inquiry, сonnection);
                сonnection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                Functions.AddJournalEntry(": __ERROR DATABASE__ Ошибка при подключении к базе данных: " + exp.Message);
            }
            finally
            {  
                сonnection.Close();
            }

        }
    }
}
