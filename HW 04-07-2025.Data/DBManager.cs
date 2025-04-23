using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_04_07_2025.Data
{

    public class DBManager
    {
        private readonly string _connectionString;

        public DBManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Simcha> GetSimachot()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.*, Count(c.ContributorId) AS 'Total Contributors', SUM(c.Amount) As 'Total' FROM Simchas s 
Left Join CONTRIbutions c on s.Id = c.SimchaId
GROUP BY s.Id, s.Name, s.Date";

            connection.Open();
            List<Simcha> sim = new();
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    sim.Add(new Simcha
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Date = (DateTime)reader["Date"],
                        ContribCount = (int)reader["ContribCount"],
                        Total = reader.GetOrDefault<decimal>("Total")
                    });
                }
                return sim;
            }
        }

        public int GetTotal()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Count (*) FROM Contributors";
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void AddASimcha(Simcha s)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Simchas(Name, Date)
VALUES (@name, @date); SELECT SCOPE_IDENTITY();";

            connection.Open();
            cmd.Parameters.AddWithValue("@name", s.Name);
            cmd.Parameters.AddWithValue("@date", s.Date);

            s.Id = (int)(decimal)cmd.ExecuteScalar();
        }

        public List<Contributor> GetSimchaContributors(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECt c.id, c.firstName, c.Lastname, c.AlwaysInclude, Sum(contrib.Amount) AS 'Contributions' From Contributors c
LEFT Join Contributions Contrib on c.id = Contrib.ContributorsId AND Contrib.SimchaID = @id
GROUP BY c.id, c.FirstName, c.LAstName,c.AlwaysInclude";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            List<Contributor> con = new();

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                con.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    First = (string)reader["First"],
                    Last = (string)reader["Last"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Contributions = reader.GetOrDefault<decimal>("Contribs"),
                });
            }

            foreach (Contributor c in con)
            {
                c.Balance = GetBalance(c.Id);
            }
            return con;
        }

        public decimal GetBalance(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT (SELECT SUM(Amount) From Deposits) WHERE ContributorId = @id) AS 'Deposits',
(SELECT SUM(Amount) FROM Contributions) WHERE ContributorId = @id) AS 'Contributions'";
            cmd.Parameters.AddWithValue(@"id", id);
            connection.Open();

            var reader = cmd.ExecuteReader();
            reader.Read();
            var deposits = reader.GetOrDefault<decimal>("Deposits");
            var contribs = reader.GetOrDefault<decimal>("Contributions");
            return deposits - contribs;
        }

        public string GetNameById(int id, string name)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            if (name == "Simchas")
            {
                cmd.CommandText = "SELECT Name FRoM Simchas WHERE id = @id";
            }
            else if (name == "Contributors")
            {
                cmd.CommandText = "SELECT FirstName + ' ' LastNAme AS 'Name'FRoM Contributors WHERE id = @id";

            }

            else
            {
                return null;
            }
            cmd.Parameters.AddWithValue(@"id", id);
            connection.Open();
            return (string)cmd.ExecuteScalar();
        }

        public void UpdateContributor(Contributor con)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Contributors SEt FirstName = @first, LastName = @last, CellNumber = @cell, Date= @date, AlwaysInclude= @Include WHERE Id =@id";
            cmd.Parameters.AddWithValue(@"first", con.First);
            cmd.Parameters.AddWithValue(@"last", con.Last);
            cmd.Parameters.AddWithValue(@"cell", con.Cell);
            cmd.Parameters.AddWithValue(@"date", con.Date);
            cmd.Parameters.AddWithValue(@"include", con.AlwaysInclude);
            cmd.Parameters.AddWithValue(@"id", con.Id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Contributions WHERE SimchaId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }


        public void UpdateSimcha(int simId, List<ContribIncusion> con)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors (SimchaId, ContributorId,Amount)
VALUES (@simId, @contribId, @amount)";
            connection.Open();
            foreach (var c in con)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue(@"simId", simId);
                cmd.Parameters.AddWithValue(@"contribId", c.ContributorId);
                cmd.Parameters.AddWithValue(@"amount", c.Amount);
                cmd.ExecuteNonQuery();
            }
        }

        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors";
            connection.Open();
            List<Contributor> con = new();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                con.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    First = (string)reader["First"],
                    Last = (string)reader["Last"],
                    Cell = (string)reader["Cell"],
                    Date = (DateTime)reader["Date"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                });
            }
            foreach (Contributor c in con)
            {
                c.Balance = GetBalance(c.Id);
            }
            return con; 
        }

        public decimal GetTotalCash()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT (SELECT SUM(Amount) From Deposits)  AS 'Deposits',
(SELECT SUM(Amount) FROM Contributions) AS 'Contributions'";
            connection.Open();
            var reader = cmd.ExecuteReader();

            reader.Read();

            var deposits = reader.GetOrDefault<decimal>("Deposits");
            var contribs = reader.GetOrDefault<decimal>("Contributions");
            return deposits - contribs;
        }

        public void AddContributor(Contributor con)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors (FirstNAme, LAstName, CellNUmber, Date, AlwaysInclude)
VALUES(@first @last,@cell, @date, include);SELECT SCOPE_IDENTITY();";
            connection.Open();

            cmd.Parameters.AddWithValue(@"first", con.First);
            cmd.Parameters.AddWithValue(@"last", con.Last);
            cmd.Parameters.AddWithValue(@"cell", con.Cell);
            cmd.Parameters.AddWithValue(@"date", con.Date);
            cmd.Parameters.AddWithValue(@"include", con.AlwaysInclude);

            con.Id = (int)(decimal)cmd.ExecuteScalar();
        }

        public void AddDeposit(Deposit d)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Deposits (ContributorId, Amount,Date
Values(@contribId,@amount,@date)";
            connection.Open();
            cmd.Parameters.AddWithValue("@contribId", d.Id);
            cmd.Parameters.AddWithValue("@amount", d.Amount);
            cmd.Parameters.AddWithValue("date", d.Date);
            cmd.ExecuteNonQuery(); 
        }

        public List<Transactions> GetDepositHistory(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT Date, Amount From Deposits WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();

            List<Transactions> t = new();
            while(reader.Read())
            {
                t.Add(new Transactions
                { 
                    Action = "Deposit",
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"]
                }); 
            }
            return t;
        }

        public List<Transactions> GetContribHistory(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.Name, s.Date, c.Amount FROM Contributions c
JOIN Simchas s On s.Id = c.simchaId WHERE c.ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();

            var reader  =cmd.ExecuteReader();
            List<Transactions> con = new();
            while(reader.Read())
            {
                con.Add(new Transactions
                {
                    Action = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"]
                }); 
            }
            return con;
        }
    }
}

