using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BeardedManStudios.Forge.Networking;

namespace BeardedManStudios.WebServer.Controllers
{
    public class ForgeWebServerAPIControllerException : BaseNetworkException
    {
        public ForgeWebServerAPIControllerException(string message) : base(message)
        {

        }
    }
    public abstract class ApiController
    {
        protected HttpListenerContext context;
        protected virtual string GetResponse()
        {
            return this.GetType().Name + "doesn't implement Get method";
        }

        protected virtual string PostResponse()
        {
            return this.GetType().Name + "doesn't implement Post method";
        }


        public string RunAPI(HttpListenerContext context)
        {
            this.context = context;

            if(context.Request.HttpMethod == "GET")
                return GetResponse();
            else if (context.Request.HttpMethod == "POST")
                return PostResponse();

            throw new ForgeWebServerAPIControllerException(GetType().Name+" does not support HTTP method "+context.Request.HttpMethod);
        }
    }
}
