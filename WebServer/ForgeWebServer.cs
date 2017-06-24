using System;
using System.IO;
using System.Text;
using System.Net;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Threading;
using BeardedManStudios.WebServer.Controllers;
using BeardedManStudios.WebServer;

//using BeardedManStudios.Threading;

namespace BeardedManStudios.WebServer
{
    public class ForgeWebServerException : BaseNetworkException
    {
        public ForgeWebServerException(string message) : base(message)
        {

        }
    }

    public class ForgeWebServer
    {
        //temporary method for accessing local web server instance
        public static ForgeWebServer instance;
        public static string[] module_names { get; private set; }
        private byte[] page_payload;

        private ushort port;
        public NetWorker forge_networker { get; private set; }
        private HttpListener listener;

        public ForgeWebServer(ushort port, NetWorker networker)
        {
            this.port = port;
            instance = this;

            if (!HttpListener.IsSupported)
                throw new ForgeWebServerException("Web servers are not supported on this machine");

            if (port == 80 || port == 443)
                throw new ForgeWebServerException(string.Format("Web servers on port {0} are not supported", port));

            listener = new HttpListener();
            listener.Prefixes.Add("http://*:" + port + "/");

            Console.WriteLine("Forge Web Server - HTTP listener starting...");
        }

        public void Start()
        {
            listener.Start();
            compilePayload();
            for (;;)
            {
                //new Thread(Listen).Start();
                Task.Queue(Listen);
            }
        }

        private void Listen()
        {
            HttpListenerContext context = listener.GetContext();
            string msg = context.Request.HttpMethod + " " + context.Request.Url;

            context.Response.AddHeader("test", "t");

            byte[] response = handleRequest(context);
            
            Stream output = context.Response.OutputStream;
            if(response != null)
                output.Write(response, 0, response.Length);

            context.Response.OutputStream.Close();
        }

        private byte[] handleRequest(HttpListenerContext context)
        {
            string controller_name = (context.Request.Url.ToString().Contains("/api/")) ? context.Request.Url.ToString().Split(new[] { "/api/" }, StringSplitOptions.None)[1].Split(new[] { "/" }, StringSplitOptions.None)[0] : "payload";

            Type controller = Type.GetType(typeof(BannerController).Namespace+"." + controller_name + "Controller, "+typeof(BannerController).Assembly.GetName().Name);

            
            if (controller_name == "payload")
            {
                return page_payload;
            }
            else if (controller == null)
            {
                Console.WriteLine("Attempted to open controller: {0} but class was not found", typeof(BannerController).Namespace + "." + controller_name + "Controller, " + typeof(BannerController).Assembly.GetName().Name);
                return null;
            }
            else if (!controller.IsSubclassOf(typeof(ApiController)))
            {
                Console.WriteLine("Attempted to open controller: {0} but class was not child of APIController", typeof(BannerController).Namespace + "." + controller_name + "Controller, " + typeof(BannerController).Assembly.GetName().Name);
                return null;
            }
            else
            {
                ApiController controller_object = (ApiController) Activator.CreateInstance(controller);
                return Encoding.UTF8.GetBytes(controller_object.RunAPI(context));
            }
        }

        private void compilePayload()
        {
            if (!Directory.Exists("www") || !Directory.Exists("www/modules"))
                throw new ForgeWebServerException("no www or www/modules directory");

            HtmlPayloadCompiler html_compiler = new HtmlPayloadCompiler("www/modules");
            module_names = html_compiler.compiled_file_names;
            page_payload = Encoding.UTF8.GetBytes(html_compiler.getPayload());
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
