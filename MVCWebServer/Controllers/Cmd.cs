using BeardedManStudios.Forge.Networking;

namespace BeardedManStudios.Forge.MVCWebServer.Controllers
{
    public class Cmd : PageController
    {
        public Cmd(NetWorker socket, ForgeWebServer webserver)
            : base(socket, webserver)
        {

        }

        public override string Render()
        {
            return PresentHTML("command");
        }
    }
}