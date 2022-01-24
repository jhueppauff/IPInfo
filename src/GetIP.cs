using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace IPInfo
{
    public static class GetIP
    {
        [Function("GetIP")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("GetIP");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            IPResponse ip = new();
            IEnumerable<string> values;
            if (req.Headers.TryGetValues("X-Forwarded-For", out values))
            {
                ip.IPAddress = values.FirstOrDefault().Split(new char[] { ',' }).FirstOrDefault().Split(new char[] { ':' }).FirstOrDefault();
            }

            // Get PTR
            ip.IPAddressHostName = (await Dns.GetHostEntryAsync(ip.IPAddress)).HostName;


            var response = req.CreateResponse(HttpStatusCode.OK);
            //response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            await response.WriteAsJsonAsync(ip);

            return response;
        }
    }
}
