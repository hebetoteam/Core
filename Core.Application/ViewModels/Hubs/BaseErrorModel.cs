using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Hubs
{
    public class BaseErrorModel
    {
        [JsonProperty("msg")]
        public string Error { get; set; }
    }
}
