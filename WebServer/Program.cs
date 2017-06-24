using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeardedManStudios.WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new ForgeWebServer(4880, null).Start();
        }
    }
}
