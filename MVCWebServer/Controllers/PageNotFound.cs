using BeardedManStudios.Forge.Networking;

namespace BeardedManStudios.Forge.MVCWebServer.Controllers
{
    public class PageNotFound : PageController
    {
        public PageNotFound(NetWorker socket, ForgeWebServer webserver)
            : base(socket, webserver)
        {

        }

        public override string Render()
        {
            return PresentHTML("404");
        }
    }
}