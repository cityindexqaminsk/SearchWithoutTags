using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTesting
{
    public class ResponseWithTags
    {
        public List<Dictionary<string, object>> MarketInformation { get; set; }
        public List<Dictionary<string, object>> Tags { get; set; }
    }
}
