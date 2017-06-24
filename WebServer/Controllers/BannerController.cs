using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeardedManStudios.WebServer.Controllers
{
    class BannerController : ApiController
    {

        protected override string GetResponse()
        {
            return "{\n\"modules\" : " + " [\n\"" +string.Join("\",\n\"", ForgeWebServer.module_names) + "\"\n]\n}";
        }
    }
}
