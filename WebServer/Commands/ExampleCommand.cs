using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeardedManStudios.WebServer.Commands
{
    class ExampleCommand : ConsoleCommand
    {
        public string test_input;
        public bool test_bool;
        public override string runCommand()
        {
            return "works " + test_input + "test" + test_bool;
        }
    }
}
