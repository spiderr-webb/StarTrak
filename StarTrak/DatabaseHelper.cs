using CsvHelper;
using System;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Windows;

public class DatabaseHelper
{
    private string connectionString;

    public DatabaseHelper(string dbPath)
    {
        connectionString = $"Data Source={dbPath};Version=3;";
        //CreateStarsTable();
        //AddStarsToTable();
    }

    private void CreateStarsTable()
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string sql1 = "DROP TABLE IF EXISTS Stars";
            using (var command1 = new SQLiteCommand(sql1, connection))
            {
                command1.ExecuteNonQuery();
            }

            string sql2 = "CREATE TABLE Stars (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, RA REAL NOT NULL, Declination REAL NOT NULL, Magnitude REAL NOT NULL, Constellation TEXT)";
            using (var command2 = new SQLiteCommand(sql2, connection))
            {
                command2.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public void AddStarsToTable()
    {
        MessageBox.Show("-");

        using var reader = new StreamReader("hyg_v42.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<dynamic>();

        foreach (var r in records)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Stars (Name, RA, Declination, Magnitude) VALUES (@name, @ra, @dec, @mag)";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@name", r.name);
                    command.Parameters.AddWithValue("@ra", r.ra);
                    command.Parameters.AddWithValue("@dec", r.dec);
                    command.Parameters.AddWithValue("@mag", r.mag);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        MessageBox.Show("Done");
    }

    public List<Star> GetStars()
    {
        List<Star> stars = new List<Star>();

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM Stars";
            using (var command = new SQLiteCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    stars.Add(new Star(Convert.ToString(reader["Name"]), Convert.ToDouble(reader["RA"]), Convert.ToDouble(reader["Declination"]), Convert.ToDouble(reader["Magnitude"])));
                    //MessageBox.Show($"ID: {reader["ID"]}, Name: {reader["Name"]}, RA: {reader["RA"]}, Dec: {reader["Declination"]}, Mag: {reader["Magnitude"]}");
                }
            }
        }

        return stars;
    }
}
