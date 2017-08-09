using BeardedManStudios.Forge.MVCWebServer.Plugins;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.SimpleJSON;
using System;
using System.Linq;
using System.Reflection;

namespace BeardedManStudios.Forge.MVCWebServer.Controllers
{
    public class Console : PageController
    {
        #region Fields
        private JSONNode response = new JSONClass();
        #endregion

        #region Default Methods
        public Console(NetWorker socket, ForgeWebServer webserver)
            : base(socket, webserver)
        {
            response = JSONNode.Parse("{\"data\":{}}");
            response["time"] = DateTime.Now.ToShortTimeString();
        }

        private void AddData(string key, JSONData val)
        {
            response["data"][key] = val;
        }

        private void AddData(string val)
        {
            AddData("message", new JSONData(val));
        }

        public override string Render()
        {
            return response.ToString();
        }

        public string Error(string message)
        {
            AddData("error", new JSONData(message));
            return Render();
        }

        public string Help(params string[] args)
        {
            MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            string data = "";
            foreach (MethodInfo method in methods)
            {
                foreach (HelpInfo help in method.GetCustomAttributes(typeof(HelpInfo), true))
                {
                    data += "<br /><br /><strong>" + method.Name + "</strong>: " + help.ToString();
                }
            }

            foreach (var kv in Webserver.Plugins)
            {
                data += "<br /><br /><strong>" + kv.Key + "</strong>: " + kv.Value.help.ToString();
            }

            AddData("Help" + data);

            return Render();
        }
        #endregion

        [HelpInfo("Shows the current player count", null, "playerCount")]
        public string PlayerCount(params string[] args)
        {
            AddData("There are currently " + Networker.Players.Count + " players online");
            return Render();
        }

        [HelpInfo("Kicks a specified player from the server", new string[] { "The numerical id of the client", "The string message to be sent to the client" }, "kick 1 \"Teleporting everywhere\"")]
        public string Kick(params string[] args)
        {
            ulong playerId = 0;
            string reason = "Server has kicked you";

            if (args.Length == 0)
                return Error("An id is required to process this command");

            if (!ulong.TryParse(args[0], out playerId))
                return Error("The id " + args[0] + " is an invalid player id");

            if (args.Length > 1)
                reason = args[1];

            try
            {
                NetworkingPlayer player = Networker.Players.First(p => p.NetworkId == playerId);

                ((IServer)Networker).Disconnect(player, true);

                AddData("You have kicked player with id " + playerId + " has been kicked");
            }
            catch
            {
                return Error("The player with the id of " + playerId + " was not found");
            }

            return Render();
        }

        [HelpInfo("Shows the time since the server started as well as the current machine time", null, "time")]
        public string Time(params string[] args)
        {
            AddData("Since start (" + Networker.Time.Timestep + ") date time (" + DateTime.Now.ToString() + ") UTC (" + DateTime.UtcNow.ToString() + ")");
            return Render();
        }

        [HelpInfo("This will ban a currently active player from the server", new string[] { "The id for the player", "The amount of time in minutes" }, "ban 1 15")]
        public string Ban(params string[] args)
        {
            if (args.Length < 2)
                return Error("No arguments were supplied, both network player id and minute amount are required");

            ulong playerId = 0;
            if (!ulong.TryParse(args[0], out playerId))
                return Error("The id " + args[0] + " is an invalid player id");

            int minutes = 0;
            if (!int.TryParse(args[1], out minutes))
                return Error("An invalid minute amount (" + args[1] + ") was input");

            try
            {
                ((IServer)Networker).BanPlayer(playerId, minutes);
                AddData("You have banned the player with id " + playerId + " for " + minutes + " minutes");
            }
            catch
            {
                return Error("The player with the id of " + playerId + " was not found");
            }

            return Render();
        }

        public string ProcessPluginCommand(string action, params string[] args)
        {
            ICommandPlugin plugin;
            if (!Webserver.TryGetPluginByName(action.ToLower(), out plugin))
                throw new System.Collections.Generic.KeyNotFoundException();

            AddData(plugin.Execute(args));
            return Render();
        }

        //[HelpInfo("This will spawn a networked object on the network", new string[] { "The name of the object you wish to spawn" }, "spawn \"CubeGuy\"")]
        //public string Spawn(params string[] args)
        //{
        //    if (args.Length == 0)
        //        return Error("An object name is required to call this action");

        //    string objectName = args[0];


        //    Unity.MainThreadManager.Run(() =>
        //    {
        //        Networking.Instantiate(objectName, NetworkReceivers.All);
        //    });

        //    AddData("Attempting to spawn the object named " + objectName);

        //    return Render();
        //}

        //[HelpInfo("This will destroy an object with the specified id", new string[] { "The network id of the object to destroy" }, "destroy 3")]
        //public string Destroy(params string[] args)
        //{
        //    if (args.Length == 0)
        //        return Error("A network id is required to call this action");

        //    ulong objectId = 0;
        //    if (!ulong.TryParse(args[0], out objectId))
        //        return Error("The specified id " + args[0] + " is not a valid network id");

        //    SimpleNetworkedMonoBehavior behavior = SimpleNetworkedMonoBehavior.Locate(objectId);

        //    Networking.Destroy(behavior);

        //    AddData("The object with id " + behavior.NetworkedId + " has been destroyed");

        //    return Render();
        //}

        //[HelpInfo("This will destroy all objects of the specified type", new string[] { "The string name of the type (class) of object to destroy" }, "destroyAll ForgeZombie")]
        //public string DestroyAll(params string[] args)
        //{
        //    if (args.Length == 0)
        //        return Error("A type name (class name) is required to call this action");

        //    string className = args[0];

        //    Unity.MainThreadManager.Run(() =>
        //    {
        //        UnityEngine.MonoBehaviour[] behaviors = UnityEngine.GameObject.FindObjectsOfType(Type.GetType(className)) as UnityEngine.MonoBehaviour[];

        //        for (int i = behaviors.Length - 1; i >= 0; --i)
        //            Networking.Destroy((SimpleNetworkedMonoBehavior)behaviors[i]);
        //    });

        //    AddData("Attempting to destroy all objects of the type " + className);

        //    return Render();
        //}

        [HelpInfo("This will print a generic string to the command window and to unity console", new string[] { "String that will be echoed out" }, "echo \"Hello World!\"")]
        public string Echo(params string[] args)
        {
            if (args.Length != 1)
                return Error("There needs to be exactly 1 argument for this command");

            Logging.BMSLog.Log("Echo " + args[0] + " from web server");
            AddData("Echo " + args[0] + " from web server");

            return Render();
        }
    }
}