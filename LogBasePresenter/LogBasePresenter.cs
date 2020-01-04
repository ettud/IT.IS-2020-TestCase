using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LogBasePresenter.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using LogBasePresenter.Models;
using LogBasePresenter.ResponseModels;
using Npgsql;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace LogBasePresenter
{
    public class LogBasePresenter
    {
        private readonly DbContextOptions _options;
        public LogBasePresenter(DbContextOptions options)
        {
            _options = options;
        }
        public LogBasePresenter(string connectionString) : this(new DbContextOptionsBuilder().UseNpgsql(connectionString).Options)
        {
        }

        public void ClearIps()
        {
            using(var context = new LogBaseContext(_options))
            {
                using(var command = context.Database.GetDbConnection().CreateCommand())
                {
                    const string sqlQuery = "DELETE FROM public.\"Subnet\"; " +
                                            "DELETE FROM public.\"Country\";";
                    command.CommandText = sqlQuery;
                    command.CommandType = CommandType.Text;
                    context.Database.OpenConnection();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
            }
        }
        public void AddCountries(IEnumerable<Country> countries)
        {
            using var context = new LogBaseContext(_options);
            context.Country.AddRange(countries);
            context.SaveChanges();
        }
        public void AddSubnets(IEnumerable<Subnet> subnets)
        {
            using var connection = new LogBaseContext(_options).Database.GetDbConnection() as NpgsqlConnection;
            connection.Open();
            using (var writer = connection.BeginBinaryImport("COPY \"Subnet\" (\"Network\", \"Broadcast\", \"CountryId\") FROM STDIN (FORMAT BINARY)"))
            {
                foreach (var subnet in subnets)
                {
                    writer.StartRow();
                    writer.Write(subnet.Network);
                    writer.Write(subnet.Broadcast);
                    writer.Write(subnet.CountryId);
                }
                writer.Complete();
            }
            connection.Close();
        }
        public void ClearLogRecords()
        {
            using(var context = new LogBaseContext(_options))
            {
                using(var command = context.Database.GetDbConnection().CreateCommand())
                {
                    const string sqlQuery = "DELETE FROM public.\"LogRecords\";";
                    command.CommandText = sqlQuery;
                    command.CommandType = CommandType.Text;
                    context.Database.OpenConnection();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void AddLogRecords(IEnumerable<LogRecord> logRecords)
        {
            using var connection = new LogBaseContext(_options).Database.GetDbConnection() as NpgsqlConnection;
            connection.Open();
            using (var writer = connection.BeginBinaryImport("COPY \"LogRecords\" (\"Id\", \"RecordTime\", \"Ip\", \"IpBit\", \"QueryDescription\") FROM STDIN (FORMAT BINARY)"))
            {
                foreach (var logRecord in logRecords)
                {
                    writer.StartRow();
                    writer.Write(logRecord.Id);
                    writer.Write(logRecord.RecordTime);
                    writer.Write(logRecord.Ip);
                    writer.Write(logRecord.IpBit);
                    writer.Write(JsonConvert.SerializeObject(logRecord.QueryDescription));
                }
                writer.Complete();
            }
            connection.Close();
        }

        /*
            Посетители из какой страны совершают больше всего действий на сайте?
         */
        public Dictionary<string, int> CountryPopularity()
        {
            string functionName = GenerateFunctionName();
            using (var context = new LogBaseContext(_options))
            {
                using (var createFunctionCommand = context.Database.GetDbConnection().CreateCommand())
                {
                    string sqlQuery =
                        $"CREATE OR REPLACE FUNCTION {functionName} () " +
                        "RETURNS SETOF integer AS $$ " +

                        "DECLARE " +
                        "temp_subnet record; " +
                        "temp_log record; " +
                        "curs_subnet CURSOR FOR SELECT \"Network\", \"Broadcast\", \"CountryId\" FROM \"Subnet\" ORDER BY \"Network\"; " +
                        
                        "BEGIN " +
                        "OPEN curs_subnet; " +
                        "FETCH curs_subnet INTO temp_subnet; " +
                        "FOR temp_log IN  SELECT \"IpBit\" FROM \"LogRecords\" " +
                        "WHERE \"IpBit\" < CAST('00000001000000000000000000000000' AS BIT(32)) ORDER BY \"Ip\" " +
                        "LOOP RETURN NEXT NULL; END LOOP; " +
                        "FOR temp_log IN  SELECT \"IpBit\" FROM \"LogRecords\" " +
                        "WHERE \"IpBit\" >= CAST('00000001000000000000000000000000' AS BIT(32)) " +
                        "AND \"IpBit\" < CAST('11111111000000000000000000000000' AS BIT(32)) ORDER BY \"Ip\" " +
                        "LOOP " +
                        "LOOP " +
                        "CASE " +
                        "WHEN temp_subnet IS NULL THEN RETURN NEXT NULL; EXIT; " +
                        "ELSE " +
                        "CASE " +
                        "WHEN (temp_log.\"IpBit\" >= temp_subnet.\"Network\" AND temp_log.\"IpBit\" <= temp_subnet.\"Broadcast\") " +
                        "THEN RETURN next temp_subnet.\"CountryId\"; EXIT; " +
                        "ELSE " +
                        "CASE " +
                        "WHEN temp_log.\"IpBit\" < temp_subnet.\"Network\" " +
                        "THEN RETURN NEXT NULL; EXIT; " +
                        "ELSE FETCH curs_subnet INTO temp_subnet; " +
                        "END CASE; " +
                        "END CASE; " +
                        "END CASE; " +
                        "END LOOP; " +
                        "END LOOP; " +
                        "FOR temp_log IN SELECT \"IpBit\" FROM \"LogRecords\" " +
                        "WHERE \"IpBit\" >= CAST('11111111000000000000000000000000' AS BIT(32)) ORDER BY \"Ip\" " +
                        "LOOP " +
                        "RETURN NEXT NULL; " +
                        "END LOOP; " +

                        "END; " +
                        "$$ LANGUAGE plpgsql;";
                    createFunctionCommand.CommandText = sqlQuery;
                    createFunctionCommand.CommandType = CommandType.Text;
                    context.Database.OpenConnection();
                    createFunctionCommand.ExecuteNonQuery();
                }

                try
                {
                    using (var getCommand = context.Database.GetDbConnection().CreateCommand())
                    {
                        string sqlQuery =
                            $"SELECT * FROM " +
                            $"(SELECT \"Name\", COUNT(\"Id\") FROM " +
                            $"(SELECT * FROM " +
                            $"(SELECT * FROM {functionName}()) cid LEFT JOIN \"Country\" c ON c.\"Id\" = cid.\"{functionName}\") countries " +
                            $"GROUP BY countries.\"Name\") co WHERE \"count\" <> 0;";
                        getCommand.CommandText = sqlQuery;
                        getCommand.CommandType = CommandType.Text;
                        context.Database.OpenConnection();
                        using (var reader = getCommand.ExecuteReader())
                        {
                            Dictionary<string, int> countriesLogCount = new Dictionary<string, int>();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string countryName = reader.GetFieldValue<string>(0);
                                    int count = reader.GetFieldValue<int>(1);
                                    countriesLogCount.Add(countryName, count);
                                }
                            }
                            return countriesLogCount;
                        }
                    }
                }
                finally
                {
                    using (var deleteFunctionCommand = context.Database.GetDbConnection().CreateCommand())
                    {
                        string sqlQuery = $"DROP FUNCTION {functionName};";
                        deleteFunctionCommand.CommandText = sqlQuery;
                        deleteFunctionCommand.CommandType = CommandType.Text;
                        context.Database.OpenConnection();
                        deleteFunctionCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        /*
            Посетители из какой страны чаще всего интересуются товарами из определенных категорий?
         */
        public Dictionary<string, int> CountryPopularityByGoodsCategoryName(string goodsCategoryName)
        {
            if (string.IsNullOrEmpty(goodsCategoryName)) throw new ArgumentNullException();
            string functionName = GenerateFunctionName();
            using (var context = new LogBaseContext(_options))
            {
                using (var createFunctionCommand = context.Database.GetDbConnection().CreateCommand())
                {
                    string sqlQuery =
                        $"CREATE OR REPLACE FUNCTION {functionName} (categoryName varchar) " +
                        "RETURNS SETOF integer AS $$ " +

                        "DECLARE " +
                        "temp_subnet record; " +
                        "temp_log record; " +
                        "curs_subnet CURSOR FOR SELECT \"Network\", \"Broadcast\", \"CountryId\" FROM \"Subnet\" ORDER BY \"Network\"; " +

                        "BEGIN " +
                        "OPEN curs_subnet; " +
                        "FETCH curs_subnet INTO temp_subnet; " +
                        "FOR temp_log IN  SELECT \"IpBit\" FROM \"LogRecords\" " +
                        "WHERE \"IpBit\" < CAST('00000001000000000000000000000000' AS BIT(32)) " +
                        "AND \"QueryDescription\"->>'GoodsCategoryName'=categoryName ORDER BY \"Ip\" " +
                        "LOOP RETURN NEXT NULL; END LOOP; " +
                        "FOR temp_log IN  SELECT \"IpBit\" FROM \"LogRecords\" " +
                        "WHERE \"IpBit\" >= CAST('00000001000000000000000000000000' AS BIT(32)) " +
                        "AND \"IpBit\" < CAST('11111111000000000000000000000000' AS BIT(32)) " +
                        "AND \"QueryDescription\"->>'GoodsCategoryName'=categoryName ORDER BY \"Ip\" " +
                        "LOOP " +
                        "LOOP " +
                        "CASE " +
                        "WHEN temp_subnet IS NULL THEN RETURN NEXT NULL; EXIT; " +
                        "ELSE " +
                        "CASE " +
                        "WHEN (temp_log.\"IpBit\" >= temp_subnet.\"Network\" AND temp_log.\"IpBit\" <= temp_subnet.\"Broadcast\") " +
                        "THEN RETURN next temp_subnet.\"CountryId\"; EXIT; " +
                        "ELSE " +
                        "CASE " +
                        "WHEN temp_log.\"IpBit\" < temp_subnet.\"Network\" " +
                        "THEN RETURN NEXT NULL; EXIT; " +
                        "ELSE FETCH curs_subnet INTO temp_subnet; " +
                        "END CASE; " +
                        "END CASE; " +
                        "END CASE; " +
                        "END LOOP; " +
                        "END LOOP; " +
                        "FOR temp_log IN SELECT \"IpBit\" FROM \"LogRecords\" " +
                        "WHERE \"IpBit\" >= CAST('11111111000000000000000000000000' AS BIT(32)) " +
                        "AND \"QueryDescription\"->>'GoodsCategoryName'=categoryName ORDER BY \"Ip\" " +
                        "LOOP " +
                        "RETURN NEXT NULL; " +
                        "END LOOP; " +

                        "END; " +
                        "$$ LANGUAGE plpgsql;";
                    createFunctionCommand.CommandText = sqlQuery;
                    createFunctionCommand.CommandType = CommandType.Text;
                    context.Database.OpenConnection();
                    createFunctionCommand.ExecuteNonQuery();
                }

                try
                {
                    using (var getCommand = context.Database.GetDbConnection().CreateCommand())
                    {
                        string sqlQuery =
                            $"SELECT * FROM " +
                            $"(SELECT \"Name\", COUNT(\"Id\") FROM " +
                            $"(SELECT * FROM " +
                            $"(SELECT * FROM {functionName}(@categoryName)) cid LEFT JOIN \"Country\" c ON c.\"Id\" = cid.\"{functionName}\") countries " +
                            $"GROUP BY countries.\"Name\") co WHERE \"count\" <> 0;";
                        getCommand.CommandText = sqlQuery;
                        getCommand.CommandType = CommandType.Text;
                        getCommand.Parameters.Add(new NpgsqlParameter<string>("categoryName", NpgsqlDbType.Varchar)
                        {
                            Value = goodsCategoryName
                        });
                        context.Database.OpenConnection();
                        using (var reader = getCommand.ExecuteReader())
                        {
                            Dictionary<string, int> countriesLogCount = new Dictionary<string, int>();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string countryName = reader.GetFieldValue<string>(0);
                                    int count = reader.GetFieldValue<int>(1);
                                    countriesLogCount.Add(countryName, count);
                                }
                            }
                            return countriesLogCount;
                        }
                    }
                }
                finally
                {
                    using (var deleteFunctionCommand = context.Database.GetDbConnection().CreateCommand())
                    {
                        string sqlQuery = $"DROP FUNCTION {functionName};";
                        deleteFunctionCommand.CommandText = sqlQuery;
                        deleteFunctionCommand.CommandType = CommandType.Text;
                        context.Database.OpenConnection();
                        deleteFunctionCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        /*
            В какое время суток чаще всего просматривают определенную категорию товаров?
         */
        public GoodsCategoryStatistics GoodsCategoryStatisticsDuringDay(string goodsCategory)
        {
            using (var context = new LogBaseContext(_options))
            {
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    const string sqlQuery =
                        "SELECT \"RecordTime\" " +
                        "FROM public.\"LogRecords\" " +
                        "WHERE (\"QueryDescription\"->>'GoodsCategoryName')=@goodsCategoryName;";
                    command.CommandText = sqlQuery;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new NpgsqlParameter<string>("goodsCategoryName", NpgsqlDbType.Varchar)
                    {
                        Value = goodsCategory
                    });

                    context.Database.OpenConnection();

                    List<DateTime> datetimes = new List<DateTime>();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                datetimes.Add(reader.GetDateTime(0));
                            }
                        }
                    }

                    return new GoodsCategoryStatistics(goodsCategory)
                    {
                        Statistics = CountDistinct(datetimes)
                    };
                }
            }
        }

        /*
            Какая нагрузка (число запросов) на сайт за астрономический час?
         */
        public double AverageQueryPerHour()
        {
            using (var context = new LogBaseContext(_options))
            {
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    const string sqlQuery =
                        "SELECT AVG(count) FROM " +
                        "(SELECT COUNT(\"Id\") FROM public.\"LogRecords\" " +
                        "GROUP BY(date_trunc('hour', \"RecordTime\"))) C;";
                    command.CommandText = sqlQuery;
                    command.CommandType = CommandType.Text;

                    context.Database.OpenConnection();
                    return Convert.ToDouble(command.ExecuteScalar());
                }
            }
        }
        /*
            Товары из какой категории чаще всего покупают совместно с товаром из заданной категории?
            Т. к. нам не хватаем данных для точного ответа, то предложим следующую ИДЕЮ АЛГОРИТМА:
            1) Находим сделанные покупки (запрос pay);
            2) Находим для каждой покупки запросы добавления в корзину (запрос cart);
            3) Т. к. в запросе cart фигурирует goods_id, по которому мы не можем определить продукт, 
                то находим предыдущий запрос продукта сделанный данным пользователем, предполагая, 
                что пользователь открывает страницу продукта, после чего либо закрывает её,
                либо добавляет этот продукт в корзину;
            4) Находим по имени каждого продукта его категорию;
            5) Анализируем пересечение категорий.
            Также сделаем предположение, что в течение сессии ip пользователя не меняется.
         */
        public GoodsCategoriesCombinationsStatistics CategoriesCombinations()
        {
            List<string> goodsCategories = new List<string>();
            //(cartId: (goods categories name)):
            Dictionary<int, List<string>> cartsGoodsCategories = new Dictionary<int, List<string>>();
            using (var context = new LogBaseContext(_options))
            {
                //получаем пользователей, хоть раз сделавших покупку
                List<IPAddress> usersIps = new List<IPAddress>();
                using (var findIps = context.Database.GetDbConnection().CreateCommand())
                {
                    const string sqlQuery =
                        "SELECT \"Ip\" FROM public.\"LogRecords\" " +
                        "WHERE \"QueryDescription\"->>'QueryType'='SuccessPayQuery' GROUP BY \"Ip\";";
                    findIps.CommandText = sqlQuery;
                    findIps.CommandType = CommandType.Text;

                    context.Database.OpenConnection();
                    using (var reader = findIps.ExecuteReader() as NpgsqlDataReader)
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                IPAddress ip = reader.GetFieldValue<IPAddress>(0);
                                usersIps.Add(ip);
                            }
                        }
                    }
                }
                
                foreach (var ip in usersIps)
                {
                    var unparsedUserQueries = new List<Tuple<DateTime, string>>();
                    //находим для данного пользователя:
                    //1) все операции по добавлению в корзину;
                    //2) успешные оплаты;
                    //3) все просмотры товаров данного пользователя;
                    using (var getQueries = context.Database.GetDbConnection().CreateCommand())
                    {
                        const string sqlQuery =
                            "SELECT \"RecordTime\", \"QueryDescription\" FROM public.\"LogRecords\" " +
                            "WHERE \"Ip\"=@ip AND " +
                            "(\"QueryDescription\"->>'QueryType'='CartQuery' OR " +
                            "\"QueryDescription\"->>'QueryType'='GoodsQuery' OR " +
                            "\"QueryDescription\"->>'QueryType'='SuccessPayQuery')" +
                            "ORDER BY \"RecordTime\";";
                        getQueries.CommandText = sqlQuery;
                        getQueries.CommandType = CommandType.Text;
                        getQueries.Parameters.Add(new NpgsqlParameter<IPAddress>("ip", NpgsqlDbType.Inet)
                        {
                            Value = ip
                        });

                        context.Database.OpenConnection();
                        using (var reader = getQueries.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var recordTime = reader.GetFieldValue<DateTime>(0);
                                    string json = reader.GetFieldValue<string>(1);
                                    unparsedUserQueries.Add(new Tuple<DateTime, string>(recordTime, json));
                                }
                            }
                        }
                    }

                    var userQueries = unparsedUserQueries.Select((tuple) => new Tuple<DateTime, AQuery>(tuple.Item1, JsonConvert.DeserializeObject<AQuery>(tuple.Item2)));
                    var payedCartIds = userQueries.Where(q => q.Item2 is SuccessPayQuery).Select(q => (q.Item2 as SuccessPayQuery).TransactionId);
                    if (payedCartIds.Any())
                    {
                        var cartOrGoodsQuery = userQueries.Where(q => q.Item2 is CartQuery || q.Item2 is GoodsQuery)
                            .Select(t => t.Item2).ToList();
                        for (int i = cartOrGoodsQuery.Count - 1; i > 0; i--)
                        {
                            if (cartOrGoodsQuery[i] is CartQuery cartQuery)
                            {
                                var cartId = cartQuery.CartId;
                                if (payedCartIds.Contains(cartQuery.CartId) &&
                                    cartOrGoodsQuery[i - 1] is GoodsQuery goodsQuery)
                                {
                                    i--;
                                    if (cartsGoodsCategories.ContainsKey(cartId))
                                    {
                                        if (!cartsGoodsCategories[cartId].Contains(goodsQuery.GoodsCategoryName))
                                        {
                                            cartsGoodsCategories[cartId].Add(goodsQuery.GoodsCategoryName);
                                        }
                                    }
                                    else
                                    {
                                        cartsGoodsCategories.Add(cartId,
                                            new List<string> {goodsQuery.GoodsCategoryName});
                                    }

                                    if (!goodsCategories.Contains(goodsQuery.GoodsCategoryName))
                                    {
                                        goodsCategories.Add(goodsQuery.GoodsCategoryName);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var result = new GoodsCategoriesCombinationsStatistics();

            for (var i = 0; i < goodsCategories.Count; i++)
            {
                var goodsCategory1 = goodsCategories[i];
                for (var j = 0; j < goodsCategories.Count; j++)
                {
                    var goodsCategory2 = goodsCategories[j];
                    if (i == j) continue;
                    if (result.TryGetValue(goodsCategory1, goodsCategory2, out _)) continue;
                    int cartWithOne = 0;
                    int cartWithCombination = 0;
                    foreach (var cart in cartsGoodsCategories)
                    {
                        if (cart.Value.Contains(goodsCategory1) || cart.Value.Contains(goodsCategory2))
                        {
                            cartWithOne++;
                            if (cart.Value.Contains(goodsCategory1) && cart.Value.Contains(goodsCategory2))
                                cartWithCombination++;
                        }
                    }

                    result.SetCombination(goodsCategory1, goodsCategory2,
                        ((double)cartWithCombination) / cartWithOne);
                }
            }
            return result;
        }

        /*
            Сколько брошенных (не оплаченных) корзин имеется за определенный период?
         */
         public int NumberOfCartsLeftUnpaid(DateTime from, DateTime to)
        {
            using (var context = new LogBaseContext(_options))
            {
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    const string sqlQuery =
                        "SELECT COUNT(*) FROM " +
                        "(SELECT \"QueryDescription\"->>'CartId' CartId " +
                        "FROM public.\"LogRecords\" " +
                        "WHERE \"QueryDescription\"->>'QueryType'='CartQuery' " +
                        "AND @from<=\"RecordTime\" AND \"RecordTime\"<=@to " +
                        "GROUP BY \"QueryDescription\"->>'CartId') Carts " +
                        "LEFT JOIN (SELECT \"QueryDescription\"->>'TransactionId' CartId " +
                        "FROM public.\"LogRecords\" " +
                        "WHERE \"QueryDescription\"->>'QueryType'='SuccessPayQuery' " +
                        "AND @from<=\"RecordTime\" AND \"RecordTime\"<=@to " +
                        "GROUP BY \"QueryDescription\"->>'TransactionId') Pays " +
                        "ON Carts.CartId=Pays.CartId WHERE Pays.CartId IS NULL;";
                    command.CommandText = sqlQuery;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new NpgsqlParameter<DateTime>("from", NpgsqlDbType.Timestamp)
                    {
                        Value = from
                    });
                    command.Parameters.Add(new NpgsqlParameter<DateTime>("to", NpgsqlDbType.Timestamp)
                    {
                        Value = to
                    });

                    context.Database.OpenConnection();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /*
            Какое количество пользователей совершали повторные покупки за определенный период?
         */
        public int NumberOfRegularUsers(DateTime from, DateTime to)
        {
            using (var context = new LogBaseContext(_options))
            {
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    const string sqlQuery =
                        "SELECT COUNT(c) FROM (SELECT COUNT(\"Id\") c FROM " +
                        "public.\"LogRecords\" WHERE " +
                        "\"QueryDescription\"->>'QueryType'='SuccessPayQuery' AND " +
                        "@from<=\"RecordTime\" AND \"RecordTime\"<=@to " +
                        "GROUP BY \"Ip\") buys WHERE c > 1;";
                    command.CommandText = sqlQuery;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new NpgsqlParameter<DateTime>("from", NpgsqlDbType.Timestamp)
                    {
                        Value = from
                    });
                    command.Parameters.Add(new NpgsqlParameter<DateTime>("to", NpgsqlDbType.Timestamp)
                    {
                        Value = to
                    });

                    context.Database.OpenConnection();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }


        //SELECT DISTINCT ("QueryDescription"->>'GoodsCategoryName') as "Name" FROM "LogRecords";
        public List<string> GoodsCategoryNames()
        {
            using (var context = new LogBaseContext(_options))
            {
                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    const string sqlQuery =
                        "SELECT * FROM (SELECT DISTINCT (\"QueryDescription\"->>'GoodsCategoryName') as \"Name\" FROM \"LogRecords\") n WHERE \"Name\" IS NOT NULL;";
                    command.CommandText = sqlQuery;
                    command.CommandType = CommandType.Text;
                    context.Database.OpenConnection();

                    List<string> names = new List<string>();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var str = reader.GetString(0);
                                if(!string.IsNullOrEmpty(str))
                                    names.Add(reader.GetString(0));
                            }
                        }
                    }

                    return names;
                }
            }
        }

        private static Dictionary<T, int> CountDistinct<T>(IEnumerable<T> collection)
        {
            Dictionary<T, int> dictionary = new Dictionary<T, int>();
            foreach (var element in collection)
            {
                if (dictionary.ContainsKey(element))
                {
                    dictionary[element] = dictionary[element] + 1;
                }
                else
                {

                    dictionary.Add(element, 1);
                }
            }
            return dictionary;
        }

        private string GenerateFunctionName()
        {
            var uidB = Guid.NewGuid().ToByteArray();
            var dtB = BitConverter.GetBytes(DateTime.Now.Ticks);
            var array = uidB.Concat(dtB);
            byte min = array.Min();
            array = array.Select(a => (byte)(a - min));
            string result = "";
            const string validSymbols = "abcdefghijklmnopqrstuvwxyz";
            foreach (var a in array)
            {
                for (int t = a; t >= validSymbols.Length; t %= validSymbols.Length)
                {
                    result += validSymbols[t % validSymbols.Length];
                }
            }
            return result;
        }
    }
}
