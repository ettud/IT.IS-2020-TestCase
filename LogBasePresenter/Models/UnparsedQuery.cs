using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LogBasePresenter.Models
{

    public class UnparsedQuery : AQuery
    {
        public UnparsedQuery(string url)
        {
            Debug.WriteLine($"Unknown url: {url}");
        }
    }
}
