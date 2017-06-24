using BeardedManStudios.SimpleJSON;

namespace BeardedManStudios.WebServer.Controllers
{
    class HomeController : ApiController
    {
        protected override string GetResponse()
        {

            bool hasNetworker = ForgeWebServer.instance.forge_networker != null;
            string noNetworkerMessage = "The Forge Web Server is not connected to a NetWorker!";
            JSONNode responseObject = new JSONClass();
            responseObject.Add("playerCount", new JSONData((hasNetworker) ?ForgeWebServer.instance.forge_networker.Players.Count.ToString():noNetworkerMessage));
            return responseObject.ToString();
        }
    }
}
