using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boticario.Api.Models
{
    public class ServiceCashbackModel
    {
        public int StatusCode { get; set; }
        public BodyCashback Body { get; set; }
    }

    public class BodyCashback
    {
        public int Credit { get; set; }
        public string Message { get; set; }
    }
}
