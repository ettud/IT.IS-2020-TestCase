using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using LogBasePresenter.Models;

namespace LogBasePresenter
{
    public class LogBasePresenter
    {
        private readonly string _connectionString;
        public LogBasePresenter(string connectionString)
        {
            _connectionString = connectionString;
        }
        /*
            Посетители из какой страны совершают больше всего действий на сайте?
            Посетители из какой страны чаще всего интересуются товарами из определенных категорий?
            В какое время суток чаще всего просматривают определенную категорию товаров?
            Какая нагрузка (число запросов) на сайт за астрономический час?
            Товары из какой категории чаще всего покупают совместно с товаром из заданной категории?
            Сколько брошенных (не оплаченных) корзин имеется за определенный период?
            Какое количество пользователей совершали повторные покупки за определенный период?
         */
        public void ClearLogRecords()
        {
            var schema = new LogBaseContext().Model.FindEntityType(typeof(LogRecord));
            var tableName = schema.GetSchema();
            new LogBaseContext().LogRecords.FromSqlRaw($"DELETE * FROM {tableName}");
        }
        public void AddLogRecords(IEnumerable<LogRecord> logRecords)
        {
            var context = new LogBaseContext();
            context.LogRecords.AddRange(logRecords);
            context.SaveChanges();
        }
    }
}
