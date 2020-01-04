using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogBasePresenter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using IpParser;
using LogBasePresenter.Models;
using System.Net;
using System.Threading;

namespace DatabaseSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            string databaseName = new NpgsqlConnection(configuration.GetSection("ConnectionStrings:DefaultConnection").Value).Database;
            using(var ddlStrem = new StreamReader(configuration.GetSection("Ddl").Value))
            {
                CreateDatabase(configuration.GetSection("ConnectionStrings:AdminConnection").Value, 
                    databaseName,
                    ddlStrem);
            }
            string defaultConnectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            {
                using var countriesStream = new StreamReader(File.OpenRead(configuration.GetSection("IpFiles:CountriesFile").Value));
                using var subnetsStream = new StreamReader(File.OpenRead(configuration.GetSection("IpFiles:NetworksFile").Value));
                LoadSubnets(defaultConnectionString, countriesStream, subnetsStream);
            }
            LoadLogs(defaultConnectionString, new StreamReader(configuration.GetSection("LogsFile").Value));
            Console.WriteLine("Database is ready!");
        }

        static void CreateDatabase(string connectionString, string databaseName, StreamReader scriptSource)
        {
            using var connection = new NpgsqlConnection(connectionString);
            for (int i = 0; true; i++)
            {
                try
                {
                    connection.Open();
                    break;
                }
                catch (PostgresException ex) when (i < 10)
                {
                    Console.WriteLine("Can't connect to database now.");
                    Thread.Sleep(1000);
                    continue;
                }
            }
            using (var dropDatabase = new NpgsqlCommand(
                    $"DROP DATABASE IF EXISTS \"{databaseName}\";",
                    connection))
            {
                if (dropDatabase.ExecuteNonQuery() != -1)
                {
                    Console.WriteLine("Old database successfully dropped.");
                }
            }
            using (var createDatabase = new NpgsqlCommand(
                $"CREATE DATABASE \"{databaseName}\";",
                connection))
            {
                createDatabase.ExecuteNonQuery();
            }
            Console.WriteLine("Database successfully created.");
            connection.ChangeDatabase(databaseName);
            var ddlScript = scriptSource.ReadToEnd();
            using (var ddl = new NpgsqlCommand(ddlScript, connection))
            {
                ddl.ExecuteNonQuery();
            }
            Console.WriteLine("Tables successfully created.");
            connection.Close();
            Console.WriteLine("Database is ready.");
        }

        static void LoadSubnets(string connectionString, StreamReader countryStream, StreamReader subnetsStream)
        {
            var parser = new GeoLite2Parser();
            parser.ParseCountries(countryStream);
            Console.WriteLine("Countries successfully parsed.");
            var logBasePresenter = new LogBasePresenter.LogBasePresenter(connectionString);
            logBasePresenter.AddCountries(parser.Countries.Select(c =>
                new LogBasePresenter.DatabaseModels.Country
                {
                    Id = c.Id,
                    Name = c.Name,
                }));
            Console.WriteLine("Countries successfully loaded.");

            parser.ParseIps(subnetsStream);
            Console.WriteLine("Ips successfully parsed.");
            logBasePresenter.AddSubnets(parser.Subnets.Select(s =>
                new LogBasePresenter.DatabaseModels.Subnet
                {
                    CountryId = s.CountryId,
                }.SetSubnetCidr(s.ZeroIp, s.Length)));
            Console.WriteLine("Ips successfully loaded.");

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using (var ddl = new NpgsqlCommand("DELETE FROM \"Country\" WHERE \"Name\"='';", connection))
            {
                ddl.ExecuteNonQuery();
            }
            connection.Close();
        }
        static void LoadLogs(string connectionString, StreamReader logsStream)
        {
            var logs = new LogParser.LogParser().ParseLogs(logsStream);
            Console.WriteLine("Logs successfully parsed.");
            var logBasePresenter = new LogBasePresenter.LogBasePresenter(connectionString);
            Task.Run(()=>logBasePresenter.AddLogRecords(logs.Select(lr => {
                var newRecord = new LogBasePresenter.DatabaseModels.LogRecord { RecordTime = lr.RecordTime };
                newRecord.Id = lr.Id;
                {
                    newRecord.QueryDescription = AQuery.FromUrl(lr.Url);
                }
                return newRecord.SetIp(IPAddress.Parse(lr.Ip));
            }))).Wait();
            Console.WriteLine("Logs successfully loaded.");
        }
    }
}
