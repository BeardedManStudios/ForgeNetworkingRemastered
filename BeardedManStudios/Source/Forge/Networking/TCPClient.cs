namespace BeardedManStudios.Forge.Networking
{
    // new TCPClient as of September 2018
    public class TCPClient : TCPClientBase
    {
        protected override void Initialize(string host, ushort port, bool pendCreates = true)
        {
            base.Initialize(host, port, pendCreates);
            InitializeTCPClient(host, port);
        }

        protected void InitializeTCPClient(string host, ushort port)
        {
            //Set the port
            SetPort(port);
        }
    }
}
