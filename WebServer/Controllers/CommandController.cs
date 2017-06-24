using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.WebServer.Commands;

namespace BeardedManStudios.WebServer.Controllers
{

    class CommandController : ApiController
    {
        protected override string GetResponse()
        {
            string command_name = context.Request.Headers["commandName"];
            Type command = Type.GetType(typeof(ConsoleCommand).Namespace + "." + command_name + "Command, " + typeof(ConsoleCommand).Assembly.GetName().Name);
            if (command_name == "command")
            {
                return "cannot get command of name command, it is a reserved name";
            }
            else if (command == null)
            {
                return string.Format("Attempted to run command: {0} but command class was not found", typeof(ConsoleCommand).Namespace + "." + command_name + "Controller, " + typeof(ConsoleCommand).Assembly.GetName().Name);
            }
            else if (!command.IsSubclassOf(typeof(ConsoleCommand)))
            {
                return string.Format("Attempted to run command: {0} but command class was not child of CommandController", typeof(ConsoleCommand).Namespace + "." + command_name + "Controller, " + typeof(ConsoleCommand).Assembly.GetName().Name);
            }
            ConsoleCommand command_object = (ConsoleCommand)Activator.CreateInstance(command);

            if(command_object.constructCommand(context.Request.Headers["commandInput"].Split(new []{" "}, StringSplitOptions.None)))
                return command_object.runCommand();
            else
                return "Command not provided input.";
        }
    }
}
