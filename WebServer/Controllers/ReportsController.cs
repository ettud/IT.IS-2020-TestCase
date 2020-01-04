using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogBasePresenter.Models;
using Microsoft.AspNetCore.Mvc;
using WebServer.Models.ReportsService;
using WebServer.Services;

namespace WebServer.Controller
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ReportsService _reportsService;
        public ReportsController(ReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [HttpGet]
        [Route("GoodsCategory")]
        public IEnumerable<GoodsCategoryResponseModel> GoodsCategory(string name)
        {
            return _reportsService.GoodsCategory(name);
        }

        [HttpGet]
        [Route("AverageQueryPerHour")]
        public double AverageQueryPerHour()
        {
            return _reportsService.AverageQueryPerHour();
        }
    }
}
