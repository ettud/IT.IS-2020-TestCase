using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogBasePresenter.Models;
using LogBasePresenter.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using WebServer.Models.ReportsService;

namespace WebServer.Services
{
    public class ReportsService
    {
        private readonly LogBasePresenter.LogBasePresenter _logBasePresenter;
        public ReportsService(LogBasePresenter.LogBasePresenter logBasePresenter)
        {
            _logBasePresenter = logBasePresenter;
        }

        public IEnumerable<GoodsCategoryResponseModel> GoodsCategory(string name)
        {
            return _logBasePresenter
                .GoodsCategoryStatisticsDuringDay(name)
                .Statistics.OrderBy(e => e.Key)
                .Select(p => new GoodsCategoryResponseModel(p.Key.TimeOfDay, p.Value));
        }

        public double AverageQueryPerHour()
        {
            return _logBasePresenter.AverageQueryPerHour();
        }

        public int NumberOfCartsLeftUnpaid(DateTime from, DateTime to)
        {
            return _logBasePresenter.NumberOfCartsLeftUnpaid(from, to);
        }

        public int NumberOfRegularUsers(DateTime from, DateTime to)
        {
            return _logBasePresenter.NumberOfRegularUsers(from, to);
        }

        public Dictionary<string, int> CountryPopularity()
        {
            return _logBasePresenter.CountryPopularity();
        }

        public Dictionary<string, int> CountryPopularity(string goodsCategoryName)
        {
            return _logBasePresenter.CountryPopularityByGoodsCategoryName(goodsCategoryName);
        }

        public GoodsCategoriesCombinationsStatistics CategoriesCombinations()
        {
            return _logBasePresenter.CategoriesCombinations();
        }


        public List<string> GoodsCategories()
        {
            return _logBasePresenter.GoodsCategoryNames();
        }
    }
}
