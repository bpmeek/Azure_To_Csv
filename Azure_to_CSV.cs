using System;
using System.IO;
using System.Data.SqlClient;
using System.Text;


namespace sqltest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "[azure].database.windows.net";
                builder.UserID = "[UserId]";
                builder.Password = "[Password]";
                builder.InitialCatalog = "[default table]";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery User Data:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    string itemSql = File.ReadAllText(@"SqlQuery.txt", Encoding.UTF8);

                    StringBuilder results = new StringBuilder();
                    string strCSV = "club_nbr, event_date, unique_users, \r\n";
                    string filePath = @"SavePath.csv";
                    using (SqlCommand command = new SqlCommand(itemSql, connection))
                    {
                        command.CommandTimeout = 60;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    object item = reader[i];
                                    strCSV += item.ToString().Replace(",", ";") + ",";
                                }
                                strCSV += "\r\n";
                            }
                        }
                    }
                    File.WriteAllText(filePath, strCSV);
                    connection.Close();
                    Console.WriteLine("Finished");
                    Environment.Exit(0);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }
}
