using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LogBasePresenter.Models;
using Newtonsoft.Json;
using WebServer.Services;

namespace WebServer.Controllers
{
    [Route("api/admin/")]
    public class AdminController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly AdminService _adminService;
        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        [Route("Test")]
        public ActionResult Test()
        {
            return new OkResult();
        }

        [HttpDelete]
        [Route("ClearLogs")]
        public ActionResult ClearLogs()
        {
            _adminService.ClearLogs();
            return new OkResult();
        }
        [HttpPost]
        [Route("UploadLogs")]
        public ActionResult UploadLogs(IFormFile file)
        {
            if (file == null) throw new ArgumentNullException();
            _uploadLogs(file.OpenReadStream());
            return new OkResult();
        }
        [HttpPost]
        [Route("UploadLogsWithUrl")]
        public ActionResult UploadLogs(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException();
            var request = (HttpWebRequest) WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                _uploadLogs(response.GetResponseStream());
                return new OkResult();
            }
            else
            {
                return new BadRequestResult();
            }
        }
        private void _uploadLogs(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException();
            IEnumerable<LogParser.LogRecord> records;
            using (var sr = new StreamReader(stream))
            {
                records = new LogParser.LogParser().ParseLogs(sr);
            }
            _adminService.UploadLogs(records);
        }
    }
}
