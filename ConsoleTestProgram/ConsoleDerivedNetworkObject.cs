using BeardedManStudios.Forge.Networking;
using System;

namespace ConsoleTestProgram
{
    public class ConsoleDerivedNetworkObject : ConsoleBehavior
    {
        public override void HelloWorld(RpcArgs rpcArgs)
        {
            Console.WriteLine(rpcArgs.GetNext<string>());
        }
    }
}