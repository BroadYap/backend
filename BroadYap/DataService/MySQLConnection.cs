using MySqlConnector;
using System.Data;
using Microsoft.Extensions.Configuration;
public class MySQLConnection
{
    private readonly string _connectionString = "Server=localhost;Database=mydb;User=root;Password=rootroot;";

    public MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}