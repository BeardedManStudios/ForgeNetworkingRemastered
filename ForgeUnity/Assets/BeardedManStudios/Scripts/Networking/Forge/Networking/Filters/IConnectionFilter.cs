namespace BeardedManStudios.Forge.Networking.Filters
{
    interface IConnectionFilter
    {
        bool CanConnect(string host, ushort port);
        bool CanAccept(NetworkingPlayer player);
    }
}
