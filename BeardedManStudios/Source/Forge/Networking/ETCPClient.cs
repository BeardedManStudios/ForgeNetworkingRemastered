namespace BeardedManStudios.Forge.Networking
{
    public class ETCPClient : ETCPClientBase
    {
        protected override void Initialize(string host, ushort port)
        {
            base.Initialize(host, port);
            InitializeTCPClient(host, port);
        }

        protected void InitializeTCPClient(string host, ushort port)
        {
            //Set the port
            SetPort(port);
        }
    }
}
