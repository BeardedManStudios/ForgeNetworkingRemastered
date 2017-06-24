using BeardedManStudios.Forge.MVCWebServer.Plugins;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace BeardedManStudios.Forge.MVCWebServer
{
    public class ForgeWebServerException : Exception
    {
        public ForgeWebServerException(string message) : base(message)
        {

        }
    }

    public class ForgeWebServer
    {
        public const string WEB_ROOT = "www";
        public const ushort DEFAULT_PORT = 15942;

        private HttpListener listener = null;

        private static Dictionary<string, string> Pages = new Dictionary<string, string>();
        //public static string HTMLDataPath { get { return WEB_ROOT + "/html"; } }
        //public static string JSDataPath { get { return WEB_ROOT + "/js"; } }
        //public static string CSSDataPath { get { return WEB_ROOT + "/css"; } }
        //public static string ImageDataPath { get { return WEB_ROOT + "/image"; } }

        public static bool Listening { get; set; }

        private NetWorker mainNetworker = null;

        internal struct Plugin
        {
            internal IWebserverPlugin plugin;
            internal HelpInfo help;
        }

        internal Dictionary<string, Plugin> Plugins { get; set; }
        internal Dictionary<string, PageController> Controllers { get; set; }

        public ForgeWebServer(NetWorker mainNetworker, Dictionary<string, string> pages, ushort port = DEFAULT_PORT)
        {
            this.mainNetworker = mainNetworker;

            if (!HttpListener.IsSupported)
                throw new ForgeWebServerException("Web servers are not supported on this machine");

            if (port == 80)
                throw new ForgeWebServerException("Port 80 is not supported for this web server");
            else if (port == 443)
                throw new ForgeWebServerException("Port 443 is not supported for this web server");

            Pages = pages;

            //if (!Directory.Exists(HTMLDataPath))
            //{
            //    Directory.CreateDirectory(HTMLDataPath);
            //    Directory.CreateDirectory(JSDataPath);
            //    Directory.CreateDirectory(CSSDataPath);
            //    Directory.CreateDirectory(ImageDataPath);
            //}

            listener = new HttpListener();

            listener.Prefixes.Add("http://*:" + port + "/");
            Plugins = new Dictionary<string, Plugin>();
            Controllers = new Dictionary<string, PageController>();
        }

        public void Start()
        {
            listener.Start();
            Task.Queue(Listen);
        }

        public void Stop()
        {
            Listening = false;
            listener.Stop();
        }

        public bool TryAddController(PageController controller)
        {
            string name = controller.GetType().Name;
            if (Controllers.ContainsKey(name))
                return false;

            Controllers.Add(name, controller);

            return false;
        }

        public void TryAddPlugin(string name, IWebserverPlugin plugin, string description = "", string example = "", params string[] parameterInfos)
        {
            Plugins.Add(name.ToLower(), new Plugin
            {
                plugin = plugin,
                help = new HelpInfo(description, parameterInfos, example)
            });
        }

        public bool TryGetPluginByName<T>(string name, out T plugin) where T : IWebserverPlugin
        {
            Plugin foundPlugin;

            if (!Plugins.TryGetValue(name, out foundPlugin))
            {
                plugin = default(T);
                return false;
            }

            if (!(foundPlugin.plugin is T))
            {
                plugin = default(T);
                return false;
            }

            plugin = (T)foundPlugin.plugin;
            return true;

        }

        private List<string> SplitArgs(string args)
        {
            List<string> matchList = new List<string>();
            Regex.Matches(args, @"(?<match>\w+)|\""(?<match>[\w\s]*)""").Cast<Match>().Select(m => m.Groups["match"].Value).ToList().ForEach(s => matchList.Add(s));
            return matchList;
        }

        private void Listen()
        {
            Listening = true;
            while (Listening)
            {
                // Thread blocking
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                // Obtain a response object
                HttpListenerResponse response = context.Response;
                Stream output = null;

                string responseString = string.Empty;
                // Construct a response

                string controllerName = Uri.UnescapeDataString(request.RawUrl);
                if (controllerName == "/")
                    controllerName = "/index";

                controllerName = controllerName.TrimStart('/');

                if (controllerName.EndsWith(".html"))
                    controllerName = controllerName.Remove(0, controllerName.LastIndexOf(".html"));
                else if (controllerName.EndsWith(".js"))
                    controllerName = controllerName.Remove(0, controllerName.LastIndexOf(".js"));
                else if (controllerName.EndsWith(".cs"))
                    controllerName = controllerName.Remove(0, controllerName.LastIndexOf(".css"));
                else if (controllerName.EndsWith(".jpg") || controllerName.EndsWith(".jpeg") || controllerName.EndsWith(".gif") || controllerName.EndsWith(".png") || controllerName.EndsWith(".ico"))
                {
                    if (controllerName.EndsWith(".jpg") || controllerName.EndsWith(".jpeg"))
                        response.ContentType = "image/jpeg";
                    else if (controllerName.EndsWith(".gif"))
                        response.ContentType = "image/gif";
                    else if (controllerName.EndsWith(".png"))
                        response.ContentType = "image/png";
                    else if (controllerName.EndsWith(".ico"))
                        response.ContentType = "image/x-icon";

                    // TODO:  Support image types via DataPath
                    output = response.OutputStream;
                    output.Write(new byte[] { 1 }, 0, 1);

                    // Close the write stream
                    output.Close();
                    continue;
                }

                controllerName = char.ToUpper(controllerName[0]) + controllerName.Substring(1);

                List<string> urlParts = SplitArgs(controllerName.Replace("/", " "));
                controllerName = urlParts[0];

                string action = string.Empty;
                if (urlParts.Count > 1)
                {
                    action = urlParts[1];
                    action = char.ToUpper(action[0]) + action.Substring(1);
                    urlParts.RemoveAt(0);
                }

                urlParts.RemoveAt(0);

                PageController controller = null;

                try
                {
                    Type controllerType = Type.GetType(GetType().Namespace + ".Controllers." + controllerName);
                    // Find the controller that is to be used to execute page logic
                    controller = Activator.CreateInstance(controllerType, mainNetworker, this)/*.Unwrap()*/ as PageController;
                }
                catch
                {
                    try
                    {
                        if (!Controllers.TryGetValue(controllerName, out controller))
                            throw;
                    }
                    catch
                    {
                        // If the specified controllerName was not found, present the 404 page
                        controller = new Controllers.PageNotFound(mainNetworker, this);
                    }
                }

                if (controller is Controllers.Console)
                    response.ContentType = "application/json";

                if (!string.IsNullOrEmpty(action))
                {
                    try
                    {
                        responseString = (string)controller.GetType().GetMethod(action).Invoke(controller, new object[] { urlParts.ToArray() });
                    }
                    catch
                    {
                        if (controller is Controllers.Console)
                        {
                            // The built in command might not exist
                            try
                            {
                                responseString = ((Controllers.Console)controller).ProcessPluginCommand(action, urlParts.ToArray());
                            }
                            catch
                            {
                                responseString = ((Controllers.Console)controller).Error("The requested command '" + action + "' was not found");
                            }
                        }
                        else
                        {
                            controller = new Controllers.PageNotFound(mainNetworker, this);
                            responseString = controller.Render();
                        }
                    }
                }
                else
                    responseString = controller.Render();

                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                output = response.OutputStream;

                // Get a response stream and write the response to it
                response.ContentLength64 = buffer.Length;
                output.Write(buffer, 0, buffer.Length);

                // Close the write stream
                output.Close();
            }
        }

        public static string RenderHTML(PageController controller, string fileName)
        {
            if (!Pages.ContainsKey(fileName))
                throw new ForgeWebServerException("The file named " + fileName + ".html could not be found");

            string page = Pages[fileName];

            foreach (string key in controller.variables.Keys)
            {
                Regex rgx = new Regex("<% " + key + " %>");
                page = rgx.Replace(page, controller.GetVariable(key));
            }

            return page;
        }
    }
}