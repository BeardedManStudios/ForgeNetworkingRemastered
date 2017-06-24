using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeardedManStudios.WebServer;
using BeardedManStudios.WebServer.Controllers;
using Newtonsoft.Json.Linq;

namespace BeardedManStudios.WebServer.Controllers
{
    class HomeController : ApiController
    {
        protected override string GetResponse()
        {

            bool hasNetworker = ForgeWebServer.instance.forge_networker != null;
            string noNetworkerMessage = "The Forge Web Server is not connected to a NetWorker!";
            JObject response_object = new JObject();
            response_object.Add("playerCount", (hasNetworker) ?ForgeWebServer.instance.forge_networker.Players.Count.ToString():noNetworkerMessage);
            return response_object.ToString();
        }
    }
}
