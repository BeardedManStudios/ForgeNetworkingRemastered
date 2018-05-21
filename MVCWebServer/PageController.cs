using BeardedManStudios.Forge.Networking;
using System;
using System.Collections.Generic;

namespace BeardedManStudios.Forge.MVCWebServer
{
    public abstract class PageController
    {
        public Dictionary<string, Func<string>> variables = new Dictionary<string, Func<string>>();
        protected NetWorker Networker { get; private set; }
        protected ForgeWebServer Webserver { get; private set; }

        public string GetVariable(string key)
        {
            if (!variables.ContainsKey(key))
                throw new ForgeWebServerException("The requested variable " + key + " was not found in the controller");

            return variables[key]();
        }

        public PageController(NetWorker socket, ForgeWebServer webserver)
        {
            Networker = socket;
            Webserver = webserver;
            variables.Add("playerCount", () => { return Networker.Players.Count.ToString(); });
        }

        abstract public string Render();

        protected string PresentHTML(string fileName)
        {
            return ForgeWebServer.RenderHTML(this, fileName);
        }
    }
}