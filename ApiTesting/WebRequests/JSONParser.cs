using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiTesting.WebRequests
{
    public class JSONParser
    {
        public ResponseWithTags GetResponseWithTags(string jSonResponse)
        {
            return JsonConvert.DeserializeObject<ResponseWithTags>(jSonResponse);
        }
    }
}
